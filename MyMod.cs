global using Terraria.Graphics.Effects;
global using Terraria.ModLoader;
global using Terraria;
global using Microsoft.Xna.Framework.Graphics;//该命名空间包含了使用XNA框架进行图形渲染所需的类和方法。通过导入这个命名空间，可以在代码中使用XNA框架提供的图形渲染功能
global using Microsoft.Xna.Framework;//使用向量/颜色等都需要这个
using Terraria.Graphics.Effects;//写shader和rt都得using这个
using Terraria.GameContent;

namespace MyMod
{
	public class MyMod : Mod
	{

        public override void Load()//模组加载时执行
        {
            SkyManager.Instance["NeonSky"] = new NeonSky();//加载我们写的天空

            //在介绍RT2D之前，必须先介绍一下On 
            //On是一种可以插入原版函数上下文的钩子，具体如下
            //打出On_XXX.AAA（XXX是你要修改的函数所在的类,AAA是你要修改的函数，可以查tml最新版源码了解）然后打+= ,vs会自动给你一个钩子，按TAB获取
            On_Player.Heal += On_Player_Heal;//例如我这里修改Player的Heal函数（弹出钩子见后文 "On的介绍" ）
            //那么On介绍就到这里，其实On可以做的事非常多，但是也不要滥用
          
            //接下来讲RT2D的实现
            On_Main.LoadWorlds += On_Main_LoadWorlds;//把创建render挂到加载世界的钩子去
            Main.OnResolutionChanged += Main_OnResolutionChanged;//这里是分辨率更改时的钩子
            On_FilterManager.EndCapture += On_FilterManager_EndCapture;//重点在这，这里是原版所有滤镜处理结束后时的函数，我们把RT绘制插在这里

            //load时读取shader
            RTShader = ModContent.Request<Effect>("MyMod/Effects/RTShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;//拿一下这个shader
            DistortShader = ModContent.Request<Effect>("MyMod/Effects/DistortShader",
                ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            base.Load();
        }
        #region RT2D
        Effect RTShader;
        Effect DistortShader;

        RenderTarget2D render;//定义一个RT2D实例，我们到时候就用这个

        private void CreateRender()//定义一个创建rt画布的函数，因为我们的render一开始是null，需要进行赋值，这段会在后面用到
        {
            if (render == null)
            {
                GraphicsDevice g = Main.instance.GraphicsDevice;
                render = new RenderTarget2D(Main.graphics.GraphicsDevice, g.PresentationParameters.BackBufferWidth,
                    g.PresentationParameters.BackBufferHeight, false, g.PresentationParameters.BackBufferFormat, 0);
            }
        }
        public static int GapEffectProj = 0;//定义一个弹幕的索引，我们的特效弹幕(RenderGap弹幕)把索引传入这里
        private void On_FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {  
            //RT2D的绘制都在这里面写
            GraphicsDevice gd = Main.instance.GraphicsDevice;//把这一长串用gd代替，方便写
            SpriteBatch sb = Main.spriteBatch;//这个同理

            //在介绍具体写法之前，我想先讲一下RT2D的基本概念。
            //1.RenderTarget本身可以被当作图片，就相当于你在画一个屏幕，同时也可以传入Shader
            //2.RenderTarget和屏幕滤镜一样会被照明模式干碎
            //GraphicsDevice.SetRenderTarget可以理解为切换到指定的RenderTarget上面，之后开始绘制就会在这上面绘制了
            //原版自带一个备用RenderTarget(也就是Main.screenTargetSwap)，用于储存场景
            //Main.screenTarget就是最终要画在屏幕上的Render,除非你已经进行完所有的处理要把最终的图像画在上面，否则不要在这画
            //当你SetRenderTarget(XXX)后，绘制的东西就全在这个“XXX”的RenderTarget上面
            //之后你在别的地方绘制这个RT，就会画出绘制了东西的这个RT
            //举个例子，gd.SetRenderTarget(Main.screenTargetSwap),这时候就切换到另一个RT上
            //你绘制了一些东西，然后end，这些东西是保留在这个RT上面的，可以被整体当成一个图片绘制出来

            //下面简单讲几个RT2D配合Shader制作
            #region RT粒子特效
            //利用RT+粒子绘制+shader制作一个"岩浆"的效果
            gd.SetRenderTarget(Main.screenTargetSwap);//切换到原版的备用RT上去
            gd.Clear(Color.Transparent);//把这个RT清空成透明
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//开始绘制
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//把现在的屏幕画到备用画布上
            sb.End();//结束，这时候备用RT上记录了目前屏幕的样子

            gd.SetRenderTarget(render);//切换到我们自己的RT
            gd.Clear(Color.Transparent);//先把RT清空成透明
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Dusts.RenderDust.DrawAll(sb);//调用我们在那个粒子里写的静态方法，把所有的粒子绘制到我们的RT上面来
            sb.End();//结束，这时候我们有了一张只画了粒子在上面,其余部分全是透明的"屏幕"
                     //切记，如果你想做很多个这样的粒子，不要像我这样遍历粒子来绘制，而是用List去存储粒子的索引，遍历list去绘制粒子

            gd.SetRenderTarget(Main.screenTarget);//接下来我们切换到screenTarget这个Render，要做最终的绘制了
            gd.Clear(Color.Transparent);//照样先清空掉这个Render
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//开始绘制
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//把之前我们保存的原本的屏幕画上来
            sb.End();//结束，然后我们要把之前的画了一堆粒子的那个rt用shader处理，处理完画到屏幕上
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//开始绘制
            gd.Textures[1] = ModContent.Request<Texture2D>("MyMod/Images/Lava").Value;//让Tex1赋值为我们的Lava图像
            RTShader.CurrentTechnique.Passes[0].Apply();//应用我们的shader,将粒子深色部分换成Lava图片,具体实现原理请参见对应fx文件
            RTShader.Parameters["m"].SetValue(0.62f);//参数作用请你查看对应的fx文件
            RTShader.Parameters["n"].SetValue(0.01f);//参数作用请你查看对应的fx文件
            RTShader.Parameters["OffsetX"].SetValue((float)((Main.GlobalTimeWrappedHourly) * 0.001f));//沿着X轴移动
            sb.Draw(render, Vector2.Zero, Color.White);//画出我们处理过的粒子屏幕
            sb.End();//结束！大功告成
            #endregion
            #region 扭曲特效
            //利用RT+shader制作一个"震波"的效果
            //原理是利用素材图的颜色代替向量，在shader里结合画了素材图的空屏幕对原屏幕进行处理
            gd.SetRenderTarget(Main.screenTargetSwap);//切换到原版的备用RT上去
            gd.Clear(Color.Transparent);//把这个RT清空成透明
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//开始绘制
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//把现在的屏幕画到备用画布上
            sb.End();//结束，这时候备用RT上记录了目前屏幕的样子

            gd.SetRenderTarget(render);//切换到我们自己的RT
            gd.Clear(Color.Transparent);//先把RT清空成透明
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None,Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            ModProj.RenderDistort.DrawAllDistortProjectile(sb);//把所有的扭曲球弹幕绘制到我们的RT上面来
            sb.End();//结束，这时候我们有了一张只画了扭曲素材图在上面,其余部分全是透明的"屏幕"
          

            gd.SetRenderTarget(Main.screenTarget);//接下来我们切换到screenTarget这个Render，要做最终的绘制了
            gd.Clear(Color.Transparent);//照样先清空掉这个Render
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//开始绘制
            gd.Textures[1] = render;//让Tex1赋值为我们的render，也就是只绘制了一堆扭曲图片弹幕其余什么也没有的屏幕
            DistortShader.Parameters["Length"].SetValue(0.03f);//扭曲的程度

            //赋值好参数后再应用shader
            DistortShader.CurrentTechnique.Passes[0].Apply();//应用我们的shader,将扭曲图片转化成扭曲,具体实现原理请参见对应fx文件
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//再在shader的加持下绘制原本的屏幕，就可以将屏幕进行扭曲处理了
           // ModProj.RenderDistort.DrawAllDistortProjectile(sb);//把所有的扭曲球弹幕绘制到我们的RT上面来
            sb.End();//结束！大功告成
            #endregion
            #region 空间割裂
            //有时候我们只需要一个特效弹幕，而非多个，就不要老是遍历了，直接用索引
            Projectile projectile = Main.projectile[GapEffectProj];
            if (projectile.type == ModContent.ProjectileType<ModProj.RenderGap>() && projectile.active)//取得这个弹幕符合条件就绘制
            {
                //利用RT+shader制作一个"空间割裂"的效果,这次我们还可以利用上一个扭曲用的shader
                //原理还是和扭曲差不多，与扭曲不同，这次我们真的要将屏幕“劈开”，也就是说我们要将整个屏幕都按照素材图进行扭曲
                //所以我们本次要绘制两大块图片，这两块图片颜色不同(例如红绿)，分别代表相反方向的偏移方向
                gd.SetRenderTarget(Main.screenTargetSwap);//切换到原版的备用RT上去
                gd.Clear(Color.Transparent);//把这个RT清空成透明
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);//开始绘制
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);//把现在的屏幕画到备用画布上
                sb.End();//结束，这时候备用RT上记录了目前屏幕的样子

                gd.SetRenderTarget(render);//切换到我们自己的RT
                gd.Clear(Color.Transparent);//先把RT清空成透明
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                //屏幕对角线长2202，也就是说边长2250的正方形铁定能覆盖完,能将屏幕切成两份！
                sb.Draw(TextureAssets.MagicPixel.Value, projectile.Center - Main.screenPosition,
                        new Rectangle(0, 0, 2250, 2250), new Color(128, 255, 0, 255), projectile.rotation, new Vector2(0, 1000),
                        1, SpriteEffects.None, 0);//画一个半红色+全绿色的方形出来(因为我们shader写的是满红色一圈，所以半红色就是半圈)
                sb.Draw(TextureAssets.MagicPixel.Value, projectile.Center - Main.screenPosition,
                        new Rectangle(0, 0, 2250, 2250), Color.Green, projectile.rotation + 3.1416f, new Vector2(0, 1000),
              1, SpriteEffects.None, 0);//往反方向画一个绿色的方形

                sb.End();//结束，这时候我们有了一张只画了扭曲素材图在上面,其余部分全是透明的"屏幕"

                gd.SetRenderTarget(Main.screenTarget);//接下来我们切换到screenTarget这个Render，要做最终的绘制了
                gd.Clear(Color.Transparent);//照样先清空掉这个Render
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);//开始绘制
                gd.Textures[1] = render;//让Tex1赋值为我们的render，也就是只绘制了扭曲图片弹幕其余什么也没有的屏幕
                DistortShader.Parameters["Length"].SetValue(0.05f * projectile.ai[0]);//扭曲的程度
                DistortShader.Parameters["Rot"].SetValue(projectile.rotation + 1.5707f);//扭曲的方向
                                                                                        //赋值好参数后再应用我们的shader
                DistortShader.CurrentTechnique.Passes[0].Apply();//应用我们的shader,将扭曲图片转化成扭曲,具体实现原理请参见对应fx文件
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);//再在shader的加持下绘制原本的屏幕，就可以将屏幕进行扭曲处理了
                sb.End();//结束
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                //我们还需要给裂开的这一段加上遮挡，来让屏幕像真的裂开了一样,其实我这里留一个小练习，怎么让这个遮挡的渐变光效变成岩浆呢？
                Texture2D tex = ModContent.Request<Texture2D>("MyMod/Images/Ex1").Value;
                sb.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation + 1.5707f, tex.Size() / 2, 
                    new Vector2(1000, 14
                    * projectile.ai[0]), SpriteEffects.None, 0);
                sb.End();
            }
            #endregion


            //别忘了在最后要执行原本的函数！不然你屏幕黑屏
            orig(self,finalTexture,screenTarget1,screenTarget2,clearColor);
        }

        private void Main_OnResolutionChanged(Vector2 obj)//在分辨率更改时，重建render防止某些bug
        {
            CreateRender();
        }

        private void On_Main_LoadWorlds(On_Main.orig_LoadWorlds orig)//加载世界时创建render,保证进游后不会是null
        {
            CreateRender();
            orig.Invoke();
        }
        #endregion
        public override void Unload()//模组卸载时执行
        {
            On_Player.Heal -= On_Player_Heal;//卸载时要把钩子卸载了
            On_Main.LoadWorlds -= On_Main_LoadWorlds;
            Main.OnResolutionChanged -= Main_OnResolutionChanged;
            base.Unload();
        }
        #region On的介绍

        //下面这个就是你打完+=，按了TAB拿到的钩子，删掉自带的throw什么什么的这个破玩意
        private void On_Player_Heal(On_Player.orig_Heal orig, Player self, int amount)//简单介绍一下用On实现给玩家的任何治疗效果都翻3倍(前提是治疗效果用的是heal)
        {
            //orig就是这个原版函数本身
            //self是执行这个方法的实例
            //要保持原版函数执行，就要将orig(self)写进来
            //下面我们在原版的Heal执行之前，先让amount 翻个三倍
            amount *= 3;
            orig(self, amount);
        }
        #endregion
    }
}