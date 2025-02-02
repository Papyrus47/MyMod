using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Dusts
{
    public class RenderDust : ModDust //这是一个RT粒子的范例
    {
        public override void OnSpawn(Dust dust)//粒子生成时执行的函数
        {
            dust.noLight = true;//粒子无光
            dust.noGravity = true;//粒子无重力
            base.OnSpawn(dust);
        }
        public override bool Update(Dust dust)//ban了原版的粒子更新，用自己的
        {
            dust.position += dust.velocity;//需要自己更新位置
            //下面的这些运动请你自己调试参数
            dust.velocity = dust.velocity.RotatedByRandom(0.1f);//速度方向每帧随机变换一点点
            dust.scale -= 0.03f;//大小不断减小
            dust.velocity *= 0.99f;//速度不断降低
            if (dust.scale <= 0)//如果大小低于0就杀掉
                dust.active = false;

            return false;
        }
        public override bool PreDraw(Dust dust)//就不用粒子本身的绘制了
        {
            return false;
        }
        public static void DrawAll(SpriteBatch sb)//这是个静态方法，因为我们要在RT那边调用，就不用粒子本身的绘制了
        {
            Texture2D tex = ModContent.Request<Texture2D>("MyMod/Content/Dusts/RenderDust").Value;//别把声明放到遍历里面，不然每循环一次都声明一次
            foreach (var d in Main.dust)//遍历每个粒子
            {
                if (d.type == ModContent.DustType<RenderDust>() && d.active)//如果符合条件就绘制
                {
                    sb.Draw(tex, d.position - Main.screenPosition, null, Color.White, 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}