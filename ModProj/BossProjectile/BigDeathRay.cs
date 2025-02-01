
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
    public class BigDeathRay : ModProjectile //尾杀大激光
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
            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeTimer = 2;
            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeScale = 10;
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if(npc.active && npc.type == ModContent.NPCType<NPCs.PrimaryBoss>() && npc.ai[0] < 950)
            {
                if (Projectile.timeLeft < 17)Projectile.timeLeft = 17;//保证NPC在维持激光发射状态时，弹幕倒计时维持在进入消失动画的前1帧
                Projectile.velocity = npc.rotation.ToRotationVector2();//让弹幕的速度方向跟随着NPC
                //生成粒子
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                float randomLength = Main.rand.NextFloat(36, 1500);//在36~1500这一段上随机生成粒子，营造效果
                var d = Dust.NewDustDirect(Projectile.Center + direction * randomLength, 0, 0, DustID.SolarFlare, 0, 0, 0,Color.Red,3f);
                d.noGravity = true;
            }
            //这一段是为了视觉效果设置的AI,localai0将被用来控制激光宽度
            if(Projectile.localAI[0] < 15 && Projectile.timeLeft > 16)//弹幕出现时增加
            Projectile.localAI[0]++;
            if (Projectile.timeLeft < 16) Projectile.localAI[0]--;//弹幕快要消失时减少
            
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.localAI[0] < 15) return false;//激光不成形时不判定
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
                150//线的宽度
                , ref point);
            if (K) return true;//如果满足这个碰撞判断，返回真，也就是进行碰撞伤害
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            float Multiply = 150f / 36f;//用我们需要的宽度(150)除以激光图片宽度(36)得到从原图放大到规定宽度所需要的倍率
            int Length = (int)LaserLength;//定义激光长度
            //黑色背景的图片如果不对A值赋予0，或者启动Additive模式的话，画出来是黑色，效果很差
            //接下来是简单的延长绘制
            //下面是激光头部的绘制
            Texture2D head = ModContent.Request<Texture2D>("MyMod/ModProj/BossProjectile/BigDeathRay_Head").Value;//获取头部材质
            Main.EntitySpriteDraw(head, Projectile.Center - Main.screenPosition,null,//不需要选框
            Color.White,
            Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
            new Vector2(0, head.Height / 2),//参考原点选择图片左边中点
            new Vector2(Multiply,Multiply * Projectile.localAI[0]/25f),//为使得激光更加自然，调整激光宽度
            SpriteEffects.None, 0);

            //下面是激光身体的绘制
            Texture2D tex = TextureAssets.Projectile[Type].Value;//获取材质，这是激光中部
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition 
                + Projectile.velocity.SafeNormalize(Vector2.Zero) * head.Width * Multiply,//接在头部后面，所以加上头部长度的方向向量
                new Rectangle(0, 0, Length, tex.Height),//在高度不变的基础上，X轴延长到length
                Color.White,
                Projectile.velocity.ToRotation(),//让图片朝向为弹幕速度方向
                new Vector2(0, tex.Height / 2),//参考原点选择图片左边中点
                new Vector2(1, Projectile.localAI[0] / 25f * Multiply),//为使得激光更加自然，调整激光宽度
                SpriteEffects.None, 0);
            return false;//return false阻止自动绘制
        }
    }
   
}