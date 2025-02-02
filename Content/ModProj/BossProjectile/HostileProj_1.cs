
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

namespace MyMod.Content.ModProj.BossProjectile
{
    public class HostileProj_1 : ModProjectile // 
    {
        public override string Texture => "MyMod/TuPian/Trail1";//这里教一下直接引用本模组内的图片
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;//这一项赋值2可以记录运动轨迹和方向（用于制作拖尾）
            ProjectileID.Sets.TrailCacheLength[Type] = 10;//这一项代表记录的轨迹最多能追溯到多少帧以前(注意最大值取不到)
            // DisplayName.SetDefault("魔弹II");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire);//克隆对应ID的弹幕属性
            Projectile.aiStyle = -1;//但是不使用原版弹幕的行为
            Projectile.timeLeft = 360;//弹幕持续6秒
            Projectile.width = Projectile.height = 16;//
        }
        public override void AI()
        {
            Projectile.velocity *= 1.022f;//不断加速
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            //同时，需要进行的绘制在这里面写就好

            Texture2D texture = TextureAssets.Projectile[Type].Value;//声明本弹幕的材质
            //要制作拖尾，首先要建立一个for循环语句，从0一直走到轨迹末端
            //这里我们介绍一个能产生高亮叠加绘制的办法（A=0）
            Color MyColor = Main.DiscoColor; MyColor.A = 0;//让A=0是为了能直接叠加颜色
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)//循环上限小于轨迹长度
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                //定义一个从新到旧由1逐渐减少到0的变量，比如i = 0时，factor = 1
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                //由于轨迹只能记录弹幕碰撞箱左上角位置，我们要手动加上弹幕宽高一半来获取中心
                Main.EntitySpriteDraw(texture, oldcenter, null, MyColor * factor,//颜色逐渐变淡
                    Projectile.oldRot[i] - 1.57f,//弹幕轨迹上的曾经的方向
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                     new Vector2(1, 1) * factor,//大小逐渐变小
                    SpriteEffects.None, 0);
            }
            //由于tr绘制是先执行的先绘制，所以要想残影不覆盖到本体上面，就要先写残影绘制

            Main.EntitySpriteDraw(  //entityspritedraw是弹幕，NPC等常用的绘制方法
                texture,//第一个参数是材质
                Projectile.Center - Main.screenPosition,//注意，绘制时的位置是以屏幕左上角为0点
                                                        //因此要用弹幕世界坐标减去屏幕左上角的坐标
                null,//第三个参数就是帧图选框了
                MyColor,//第四个参数是颜色，这里我们用自带的lightcolor，可以受到自然光照影响
                Projectile.rotation - 1.57f,//第五个参数是贴图旋转方向
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                //第六个参数是贴图参照原点的坐标，这里写为贴图单帧的中心坐标，这样旋转和缩放都是围绕中心
                new Vector2(1, 1),//第七个参数是缩放，X是水平倍率，Y是竖直倍率
                SpriteEffects.None,
                0//第九个参数是绘制层级，但填0就行了，不太好使
                );

            return false;//return false阻止自动绘制
        }
    }

}