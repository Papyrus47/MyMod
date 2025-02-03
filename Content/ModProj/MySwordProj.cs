
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MyMod.Content.ModProj
{
    public class MySwordProj : ModProjectile
    {
        Player player => Main.player[Projectile.owner];
        //定义生成弹幕时传入的owner参数对应的玩家
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;//拖尾长度
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;//拖尾模式
            Projectile.width = Projectile.height = 32;//长宽为两格物块长度
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕                                          
            Projectile.tileCollide = false;//false就能让他穿墙
            Projectile.timeLeft = 60;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.DamageType = DamageClass.Melee;//近战伤害
            Projectile.penetrate = -1;//表示能穿透几次,-1代表无限
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float Length = 100 * Projectile.scale * Projectile.ai[0];//定义剑的长度
            //这个函数用于控制弹幕碰撞判断，符合你的碰撞条件时返回真即可
            float point = 0f;//这个照抄就行
            Vector2 startPoint = player.Center;//判定从玩家中心开始延伸
            Vector2 endPoint = player.Center + Projectile.rotation.ToRotationVector2() * Length;//从玩家处延伸到指定长度的末端
            bool K =
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), //对方碰撞箱的位置
                targetHitbox.Size(),//对方碰撞箱的大小 
                startPoint,//线形碰撞箱起始点 
                endPoint,//结束点
                10 * Projectile.scale//线的宽度
                , ref point);
            if (K) return true;//如果满足这个碰撞判断，返回真，也就是进行碰撞伤害
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;//我们不以timeleft作为弹幕消失要求,因此需要始终维持timeleft
            float rotatespeed = Projectile.ai[1];//我们生成这个弹幕时，传入ai1作为挥舞速度
                                                 //  player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;//维持住玩家的使用
            //我们在发射这个弹幕时给ai0传入-1或1（玩家朝着右边挥舞就传入1，朝着左边就传入-1，因为这是不对称的武器）
            Projectile.rotation += Projectile.ai[0] * rotatespeed;//让这个弹幕顺时针/逆时针转，取决于弹幕生成出来时向左还是向右
                                                                    // Main.NewText(Projectile.localAI[0]);
            Projectile.localAI[0] += rotatespeed;//让弹幕的一个变量每帧加上这个角速度，用以判断武器挥舞到什么程度了
            float maxRotate = 3.5f;//定义武器最大能挥舞到什么程度
            if (Math.Abs(Projectile.localAI[0]) > maxRotate)//如果挥了这么多，就kill掉
            {
                Projectile.Kill();
            }
            player.itemRotation = Projectile.rotation;//控制玩家手臂
            Projectile.Center = player.Center;//弹幕固定在玩家中心
            base.AI();
        }
        public override bool ShouldUpdatePosition()//禁止弹幕因为速度更新位置，说白了就是禁用速度
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)//重写predraw，不使用原版绘制，自己写绘制非常自由
        {
            float rangeFix = 52 * Projectile.scale;
            float rotationFix = 0.785f;//你剑的贴图方向和水平向右差多少弧度就填多少
            Texture2D weapon = TextureAssets.Projectile[Type].Value;
            //draw的九个参数分别的意思请参照“AdvancedProj”中的predraw，本文件不予赘述
            if (Projectile.ai[0] == 1)
            {
                Main.EntitySpriteDraw(weapon, Projectile.Center - Main.screenPosition + rangeFix * Projectile.rotation.ToRotationVector2(),//修正距离请你自己调试
             null, lightColor,
             //lightcolor是受到光照影响的，不喜欢可以直接用Color.White
             Projectile.rotation + rotationFix * Projectile.ai[0],//武器贴图只要不是水平向右都需要加上修正角度，并且由于我们还要有顺时针逆时针之差，所以修正角度也要乘
             weapon.Size() / 2, Projectile.scale, SpriteEffects.None,//判断弹幕ai0以此翻转贴图
             0);
            }
            else
            {
                Main.EntitySpriteDraw(weapon, Projectile.Center - Main.screenPosition - rangeFix * Projectile.rotation.ToRotationVector2(),//修正距离请你自己调试
         null, lightColor,
         //lightcolor是受到光照影响的，不喜欢可以直接用Color.White
         Projectile.rotation + rotationFix * Projectile.ai[0],//武器贴图只要不是水平向右都需要加上修正角度，并且由于我们还要有顺时针逆时针之差，所以修正角度也要乘
         weapon.Size() / 2, Projectile.scale, SpriteEffects.FlipHorizontally,//判断弹幕ai0以此翻转贴图
         0);
            }
            //下面是残影绘制（可选）
            //for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            //{
            //    if (Projectile.oldPos[i] == Vector2.Zero) continue;
            //    if (Projectile.ai[0] == 1)
            //    {
            //        Main.EntitySpriteDraw(weapon, Projectile.Center - Main.screenPosition + rangeFix * Projectile.oldRot[i].ToRotationVector2(),//修正距离请你自己调试
            //     null, lightColor * 0.14f * (1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type]),
            //     //lightcolor是受到光照影响的，不喜欢可以直接用Color.White
            //     Projectile.oldRot[i] + rotationFix * Projectile.ai[0],//武器贴图只要不是水平向右都需要加上修正角度，并且由于我们还要有顺时针逆时针之差，所以修正角度也要乘
            //     weapon.Size() / 2, Projectile.scale, SpriteEffects.None,//判断弹幕ai0以此翻转贴图
            //     0);
            //    }
            //    else
            //    {
            //        Main.EntitySpriteDraw(weapon, Projectile.Center - Main.screenPosition - rangeFix * Projectile.oldRot[i].ToRotationVector2(),//修正距离请你自己调试
            // null, lightColor * 0.14f * (1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type]),
            // //lightcolor是受到光照影响的，不喜欢可以直接用Color.White
            // Projectile.oldRot[i] + rotationFix * Projectile.ai[0],//武器贴图只要不是水平向右都需要加上修正角度，并且由于我们还要有顺时针逆时针之差，所以修正角度也要乘
            // weapon.Size() / 2, Projectile.scale, SpriteEffects.FlipHorizontally,//判断弹幕ai0以此翻转贴图
            // 0);
            //    }
            //}
            //下面是顶点绘制拖尾（进阶，可以不学习）,在学习这个之前请你先看完绿群教程中的“顶点绘制入门”教程
            List<CustomVertexInfo> vertices = new List<CustomVertexInfo>();//声明一个顶点结构体的list，顶点结构体需要自己写，本mod自带一份，可以拿来用
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                if (Projectile.ai[0] == 1)
                {
                    Color coordColor = Main.hslToRgb(i / 20f * 1f, 0.5f, 0.5f);//这是利用hsl的色相来使得每个纹理处的颜色都不一样，形成彩虹色，你可以用别的颜色
                    vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + rangeFix * Projectile.oldRot[i].ToRotationVector2() * 1.9f,
                      new Vector3((float)i / ProjectileID.Sets.TrailCacheLength[Type], 1, 1), coordColor));//上底
                    vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition + rangeFix * Projectile.oldRot[i].ToRotationVector2() * 0.25f,
                        new Vector3((float)i / ProjectileID.Sets.TrailCacheLength[Type], 0, 1), coordColor));//下底

                }
                else
                {
                    vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition - rangeFix * Projectile.oldRot[i].ToRotationVector2() * 1.9f,
                                       new Vector3((float)i / ProjectileID.Sets.TrailCacheLength[Type], 1, 1), Main.hslToRgb(i / 20f * 1f, 0.5f, 0.5f)));//上底
                    vertices.Add(new CustomVertexInfo(Projectile.Center - Main.screenPosition - rangeFix * Projectile.oldRot[i].ToRotationVector2() * 0.25f,
                        new Vector3((float)i / ProjectileID.Sets.TrailCacheLength[Type], 0, 1), Main.hslToRgb(i / 20f * 1f, 0.5f, 0.5f)));//下底

                }
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("MyMod/Images/SlashTex").Value;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //为了让弹幕具有打击感，我们还要在击中函数中写一次生成爆炸的弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromAI()//生成源一般不知道填什么的时候就这么写，反正没用
                , target.Center, Vector2.Zero,//因为爆炸弹幕是静止的所以速度为0
                ProjectileID.DaybreakExplosion,//借用一下破晓的爆炸效果
                0, //伤害为0因为我们不想让他造成伤害
                0, player.whoAmI);

        }
    }
}