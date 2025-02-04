
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

namespace MyMod.Content.ModProj
{
    public class DeathRay : ModProjectile //激光弹幕教学
    {
        Player player => Main.player[Projectile.owner];
        float LaserLength = 0;//设定一个长度字段
        //定义生成弹幕时传入的owner参数对应的玩家
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
        #region 不穿墙激光
        void SetLaserPosition()//不穿墙的判断
        {
            LaserLength = 20;
            Vector2 unit = Projectile.velocity.SafeNormalize(Vector2.Zero);
            while (LaserLength <= 1500)//长度还没超过1500时进行循环
            {
                Vector2 range = Projectile.Center + unit * LaserLength;//这是当前激光最远端
                if (!Collision.CanHit(Projectile.Center, 1, 1, range, 1, 1))//如果远端和起点隔着墙
                {
                    LaserLength -= 5;//距离-5
                    return;//跳出该函数
                }
                LaserLength += 2;//距离+2
            }
        }
        #endregion
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽无所谓，我们需要改写碰撞箱了
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕                                          
            Projectile.tileCollide = false;//false就能让他穿墙,就算是不穿墙激光也不要设置不穿墙
            Projectile.timeLeft = 120;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.DamageType = DamageClass.Magic;//魔法伤害
            Projectile.penetrate = -1;//表示能穿透几次怪物。-1是无限制
            Projectile.ignoreWater = true;//无视液体
            Projectile.hide = true; // 隐藏弹幕避免绘制两次 这是重点
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
            if (Projectile.localAI[0] < 25 && Projectile.timeLeft > 26)//弹幕出现时增加
                Projectile.localAI[0]++;
            if (Projectile.timeLeft < 26) Projectile.localAI[0]--;//弹幕快要消失时减少
            SetLaserPosition();//进行碰撞判断
            if (player.channel)
            {
                if (player.direction == 1)//如果玩家朝着右边
                {
                    player.itemRotation = Projectile.velocity.ToRotation();//获取玩家到弹幕向量的方向
                }
                else
                {
                    player.itemRotation = Projectile.velocity.ToRotation() + 3.1415926f;//反之需要+半圈
                }
                player.heldProj = Projectile.whoAmI;//之前漏讲了，手持弹幕要写这个
                player.itemAnimation = player.itemTime = 25;
                if (Projectile.timeLeft < 30)
                    Projectile.timeLeft = 30;//保持激光不衰减
            }
            Projectile.Center = player.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 80;
            //让弹幕的位置保持在距离玩家80的地方，这样能有武器的感觉
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.MouseWorld - player.Center, 0.05f);
            //让激光方向追着鼠标走
            Dust.NewDustDirect(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserLength
                + Main.rand.NextVector2Circular(40, 40), 0, 0, DustID.Phantasmal, 0, 0, 0).scale = 2;
            Dust.NewDustDirect(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserLength
     + Main.rand.NextVector2Circular(40, 40), 0, 0, DustID.Phantasmal, 0, 0, 0).scale = 2;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.localAI[0] < 25) return false;//激光不成形时不判定
            int Length = (int)LaserLength;//定义激光长度
            //这个函数用于控制弹幕碰撞判断，符合你的碰撞条件时返回真即可
            float point = 0f;//这个照抄就行
            Vector2 startPoint = Projectile.Center;
            Vector2 endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Length;
            //结束点在弹幕速度方向上距离1500像素处的位置
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
            int Length = (int)LaserLength;//定义激光长度
            //黑色背景的图片如果不对A值赋予0，或者启动Additive模式的话，画出来是黑色，效果很差
            //接下来是简单的延长绘制
            Color color1 = Color.White;//白色绘制就是图片原色
            color1.A = 0;//A赋予0使得图片颜色变为加算,可以去掉黑色部分
            //下面是激光头部的绘制
            Texture2D head = ModContent.Request<Texture2D>("MyMod/Content/ModProj/DeathRayHead").Value;//获取头部材质
            Main.EntitySpriteDraw(head, Projectile.Center - Main.screenPosition, null,//不需要选框
            color1,//修改后的颜色
            Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
            new Vector2(0, head.Height / 2),//参考原点选择图片左边中点
            new Vector2(1, Projectile.localAI[0] / 25f),//为使得激光更加自然，调整激光宽度
            SpriteEffects.None, 0);
            //下面是激光身体的绘制
            Texture2D tex = TextureAssets.Projectile[Type].Value;//获取材质，这是激光中部
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition
                + Projectile.velocity.SafeNormalize(Vector2.Zero) * head.Width,//接在头部后面，所以加上头部长度的方向向量
                new Rectangle(0, 0, Length, tex.Height),//在高度不变的基础上，X轴延长到length
                color1,//修改后的颜色
                Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
                new Vector2(0, tex.Height / 2),//参考原点选择图片左边中点
                new Vector2(1, Projectile.localAI[0] / 25f),//为使得激光更加自然，调整激光宽度
                SpriteEffects.None, 0);
            //下面是激光尾部的绘制
            Texture2D Tail = ModContent.Request<Texture2D>("MyMod/Content/ModProj/DeathRayTail").Value;//获取头部材质
            Main.EntitySpriteDraw(Tail, Projectile.Center - Main.screenPosition
             + Projectile.velocity.SafeNormalize(Vector2.Zero) * (head.Width + Length),//接在身体末端的后面
            null,//不需要选框
            color1,//修改后的颜色
            Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
            new Vector2(0, Tail.Height / 2),//参考原点选择图片左边中点
           new Vector2(1, Projectile.localAI[0] / 25f),//为使得激光更加自然，调整激光宽度    
            SpriteEffects.None, 0);
            return false;//return false阻止自动绘制
        }
    }

}