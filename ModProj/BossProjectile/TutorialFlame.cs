
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

namespace MyMod.ModProj.BossProjectile
{
    public class TutorialFlame : ModProjectile // 仿制魔焰眼的喷火效果
    {
        public override string Texture => "Terraria/Images/Item_0";//0是透明贴图
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("教程之火");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire);//克隆对应ID的弹幕属性
            Projectile.aiStyle = -1;//但是不使用原版弹幕的行为
            Projectile.timeLeft = 60;//弹幕持续一秒
            Projectile.width = Projectile.height = 48;//增加碰撞箱
            Projectile.extraUpdates = 4;//每帧更新5次
        }
        public override void AI()
        {
            int random = Main.rand.Next(3);//生成一个0，1，2这三个之一的随机整数
            
            switch (random)
            {
                case 0:
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 0, default, 3f);
                    d.noGravity = true; d.velocity = Main.rand.NextVector2Circular(5, 5);d.velocity += Projectile.velocity;
                    break;
                case 1:
                    d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0, 0, 0, default,3);
                    d.noGravity = true; d.velocity = Main.rand.NextVector2Circular(5, 5); d.velocity += Projectile.velocity;
                    break;
                case 2:
                    d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, 0, default, 3);
                    d.noGravity = true; d.velocity = Main.rand.NextVector2Circular(5, 5); d.velocity += Projectile.velocity;
                    break;
                //这样我们就可以随机生成火焰 暗影焰 霜焰 粒子了
            }
        

        }
        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            //纯粒子的弹幕无需绘制图片
            return false;//return false阻止自动绘制
        }
    }
   
}