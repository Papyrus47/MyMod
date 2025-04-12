
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
    public class MyRodProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Terrarian;
        //这行可以将弹幕的材质引用指向原版对应路径的贴图，非常好用，这样就不用准备图片了
        //材质路径格式：Terraria/Images/Item_ 等等，具体请参考解包出来的贴图包名称
        Player player => Main.player[Projectile.owner];
        //定义生成弹幕时传入的owner参数对应的玩家
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽为两格物块长度
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕                                          
            Projectile.tileCollide = false;//false就能让他穿墙
            Projectile.timeLeft = 20;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.DamageType = DamageClass.Magic;//魔法伤害
            Projectile.penetrate = 1;//表示能穿透几次
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        public override void AI()
        {
            #region 风凌的AI
            //if (player.channel && Projectile.ai[0] == 0)//只有玩家持续按住鼠标时并且ai0没被改掉时触发
            //{
            //    Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero)//这是获取弹幕到鼠标的单位向量 
            //        * MathHelper.Min(Vector2.Distance(Projectile.Center, Main.MouseWorld), 15f);
            //    //每帧给予本弹幕一个朝向鼠标的15f每帧的速度,但是由于靠近鼠标时再以这么大的速度走会不停抽搐，
            //    //所以需要用min(两者取最小)方法把速率因子压进距离
            //    player.itemAnimation = player.itemTime = 10; //固定玩家使用时间，这样松开鼠标10帧之后玩家使用完毕
            //    Projectile.timeLeft = 182;//固定弹幕消散倒计时，这样松开鼠标后弹幕会再运行180帧也就是3秒

            //    //下面是使得玩家的法杖方向对着弹幕的效果，由于tr奇妙的手方向判定，我们必须要分情况讨论
            //    if (player.direction == 1)//如果玩家朝着右边
            //    {
            //        player.itemRotation = (Projectile.Center - player.Center).ToRotation();//获取玩家到弹幕向量的方向
            //    }
            //    else
            //    {
            //        player.itemRotation = (Projectile.Center - player.Center).ToRotation() + 3.1415926f;//反之需要+半圈
            //    }
            //}
            //else
            //{
            //    Projectile.ai[0] = 1;//如果玩家松手了，那就改掉ai0不让他继续执行被按住的AI，转而加速直线运动下去
            //    if (Projectile.velocity.Length() < 1) Projectile.velocity = (Projectile.Center - player.Center).SafeNormalize(
            //        Vector2.Zero) * 10;//如果弹幕速度太小，那么就让它直接发射出去好了
            //    Projectile.velocity *= 1.055f;//不断加速
            //}
            #endregion
            #region 小天使的AI
            // ai[0] 状态指示器
            Player owner = Main.player[Projectile.owner];
            if (owner.channel && (int)Projectile.ai[0] == 0) // 只有玩家持续按住鼠标时并且ai0处于0时触发（0表示追踪鼠标的状态）
            {
                Vector2 vel = Main.MouseWorld - Projectile.Center; // 这是获取弹幕到鼠标的向量
                if (vel.Length() > 10) // 如果鼠标距离弹幕不是特别近的状态，就让弹幕跟随鼠标
                    Projectile.velocity = (Projectile.velocity * 10 + vel.SafeNormalize(Vector2.Zero) * 10f) / 11f; // 惯性移动

                owner.itemAnimation = owner.itemTime = 10; // 固定玩家使用时间，这样松开鼠标10帧之后玩家使用完毕
                Projectile.timeLeft = 180; // 固定弹幕消散倒计时，这样松开鼠标后弹幕会再运行180帧也就是3秒
                vel = owner.Center - Projectile.Center; // 这是获取玩家到弹幕的向量
                owner.itemRotation = MathF.Atan2(vel.Y * player.direction, vel.X * player.direction); // 让玩家的法杖方向对着弹幕
            }
            else // 自由移动状态
            {
                Projectile.ai[0] = 1; // ai0改为1，表示不再追踪鼠标
                Projectile.velocity = Projectile.velocity.SafeNormalize(default) * 15f; // 让弹幕以最大速度向前移动
            }
            #endregion
            //以上是手持弹幕的核心,下面是视觉效果的AI,使用for循环让他反复生成粒子
            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch,
                    0, 0, 0, default, 2);
                d.noGravity = true;//禁用粒子重力
                                   //为了让弹幕动起来更好看，生成一些粒子是必要的,那么再来一点紫色粒子吧
                d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch,
                    0, 0, 0, default, 2);
                d.noGravity = true;//禁用粒子重力
                                   //放心，生成粒子的方法会在第一个参数和第二第三个参数组成的一个矩形中随机位置生成粒子，不会重叠的
            }
            base.AI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //为了让弹幕具有打击感，我们还要在击中函数中写一次生成爆炸的弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromAI()//生成源一般不知道填什么的时候就这么写，反正没用
                , Projectile.Center, Vector2.Zero,//因为爆炸弹幕是静止的所以速度为0
                ProjectileID.DaybreakExplosion,//借用一下破晓的爆炸效果
                0, //伤害为0因为我们不想让他造成伤害
                0, player.whoAmI);

        }
    }
}