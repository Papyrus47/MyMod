
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
    public class TempleProj : ModProjectile //这是个子弹弹幕
    {
        public override void SetStaticDefaults()//本函数每次加载模组时执行一次，用于分配静态属性
        {

            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;//长宽为4，因为子弹碰撞箱尽可能要小。
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置 因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕                                          
            Projectile.tileCollide = false;//false就能让他穿墙
            Projectile.timeLeft = 300;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;//表示能穿透几次
            Projectile.ignoreWater = true;//无视液体
            Projectile.extraUpdates = 2;//每帧多更新2次，这样速度可以翻2倍的同时不让粒子稀疏,也可以保证高速经过NPC或者墙体时有判断
            base.SetDefaults();
        }
        public override void AI()
        {
            //这个弹幕会生成一些粒子来营造尾迹。
            var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Torch, 0, 0, 0, default, 2);//生成一个粒子,设为d
            d.noGravity = true;//粒子无重力
            int index = Projectile.FindTargetWithLineOfSight(1000);//寻找1000范围内最近敌人
            if (index >= 0)//如果找不到，index会是-1
            {
                NPC npc = Main.npc[index];//定义这个NPC
                Projectile.velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10f;//以30像素每帧的速度进行追击(因为额外更新)
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)//击中NPC时产生一些效果
        {
            if (Main.rand.NextBool(25))//25分之一的几率
            {
                int strike = (int)(target.lifeMax * 0.01f);//目标最大生命1%的额外伤害
                NPC.HitInfo hitInfo = new NPC.HitInfo();
                hitInfo.SourceDamage = strike;
                target.StrikeNPC(hitInfo);//造成伤害
                //StrikeNPC方法是一个可以对NPC造成一次伤害的方法，参数为伤害、击退、击退方向、暴击、是否不造成受伤效果、是否来自联机网络
                //不过不能给玩家DPS
            }
            //所以这个子弹可以在击中NPC时有4%概率削血1%
            //NPC(target, damage, knockback, crit);
        }
        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            //不想绘制东西了，摆烂了。
            return false;//return false阻止自动绘制
        }
    }

}