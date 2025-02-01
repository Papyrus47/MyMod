
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MyMod.ModProj.BossProjectile
{
    public class RoundLight : ModProjectile //一种圆球状视觉效果弹幕
    {   
        public override void SetStaticDefaults()//本函数每次加载模组时执行一次，用于分配静态属性
        {
            Main.projFrames[Type] = 1;//你的帧图有多少帧就填多少
          //  ProjectileID.Sets.TrailingMode[Type] = 2;//这一项赋值2可以记录运动轨迹和方向（用于制作拖尾）
          //  ProjectileID.Sets.TrailCacheLength[Type] = 10;//这一项代表记录的轨迹最多能追溯到多少帧以前(注意最大值取不到)
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//这一项代表弹幕超过屏幕外多少距离以内可以绘制
                                                                //用于长条形弹幕绘制
                                                                //激光弹幕建议4000左右
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽无所谓
            Projectile.friendly = false;
            Projectile.tileCollide = false;//false就能让他穿墙,就算是不穿墙激光也不要设置不穿墙
            Projectile.timeLeft = 60;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.2f;//一个不断增加的计时器，用以控制光效大小

            //不使用scale是怕出现其他模组修改弹幕大小的情况
        }

        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            var tex = TextureAssets.Projectile[Type].Value;
            Color red = new Color(255, 0, 0, 0);//还是用A=0的办法去掉黑底
            Color white = new Color(255, 255, 255, 0);//还是用A=0的办法去掉黑底
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, red, 0, tex.Size() / 2, 
                Projectile.ai[0], SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null,white, 0, tex.Size() / 2,
               Projectile.ai[0], SpriteEffects.None, 0);
            //为什么我要绘制一次红的一次白的？因为在A=0的情况下，颜色是可以叠加的，同时绘制一次红和一次白可以起到增强光效的感觉
            //
          
            return false;//return false阻止自动绘制
        }
    }
   
}