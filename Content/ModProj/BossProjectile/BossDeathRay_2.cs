
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

namespace MyMod.Content.ModProj.BossProjectile
{
    public class BossDeathRay_2 : ModProjectile //敌对双方向激光弹幕(不需要头尾了)
    {
        float LaserLength = 0;//设定一个长度字段
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
            LaserLength = 2500;//2500的长度
            Projectile.width = Projectile.height = 32;//长宽无所谓，我们需要改写碰撞箱了
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = false;//友方弹幕
            Projectile.hostile = true;//敌对弹幕
            Projectile.tileCollide = false;//false就能让他穿墙,就算是不穿墙激光也不要设置不穿墙
            Projectile.timeLeft = 120;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.penetrate = -1;//表示能穿透几次怪物。-1是无限制
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        public override bool ShouldUpdatePosition()//决定这个弹幕的速度是否控制他的位置变化
        {

            return false;
            //注意，激光类弹幕要返回false,速度只是用来赋予激光方向和击退力的，要修改位置请直接动center
        }
        public override void AI()//激光AI主要是控制方向和源点位置
        {
            //这一段是为了视觉效果设置的AI,localai0将被用来控制激光宽度
            if (Projectile.localAI[0] < 15 && Projectile.timeLeft > 16)//弹幕出现时增加
                Projectile.localAI[0]++;
            if (Projectile.timeLeft < 16) Projectile.localAI[0]--;//弹幕快要消失时减少

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float randomLength = Main.rand.NextFloat(36, 1500);//在36~1500这一段上随机生成粒子，营造效果
            Dust.NewDust(Projectile.Center + direction * randomLength, 0, 0, DustID.RedTorch, 0, 0, 0, default, 3);

            if (Projectile.timeLeft == 114)//激光出现后发射垂直的一排弹幕
            {

                for (int i = -10; i <= 10; i++)//用嵌套循环来完成
                {
                    //第一层循环决定弹幕在激光上的位置
                    float inter = 150;//每两个位置的间隔
                    Vector2 p = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * i * inter;
                    for (int j = -1; j <= 1; j += 2)
                    {
                        float rotation = j * MathHelper.Pi / 2 + Projectile.velocity.ToRotation();//要么顺时针偏移90度，要么逆时针偏移90度，这样正好是相反方向
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), p, rotation.ToRotationVector2() * 0.09f,
                            ModContent.ProjectileType<HostileProj_1>(), 19, 1);
                    }
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.localAI[0] < 15) return false;//激光不成形时不判定
            int Length = (int)LaserLength;//定义激光长度
            //这个函数用于控制弹幕碰撞判断，符合你的碰撞条件时返回真即可
            float point = 0f;//这个照抄就行
            Vector2 startPoint = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * Length;//双方向的激光,起点设在反方向远端
            Vector2 endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Length;//双方向的激光，终点在正方向远端
            bool K =
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), //对方碰撞箱的位置
                targetHitbox.Size(),//对方碰撞箱的大小 
                startPoint,//线形碰撞箱起始点 
                endPoint,//结束点
                50//线的宽度
                , ref point);
            if (K) return true;//如果满足这个碰撞判断，返回真，也就是进行碰撞伤害
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            var tex = TextureAssets.Projectile[Type].Value;
            Color colorA = new Color(255, 255, 255, 0);//
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, colorA, Projectile.velocity.ToRotation(),
                tex.Size() / 2, new Vector2(50, Projectile.localAI[0] / 30f), SpriteEffects.None, 0);
            return false;//return false阻止自动绘制
        }
    }

}