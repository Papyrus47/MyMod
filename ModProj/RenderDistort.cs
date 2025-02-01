
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

namespace MyMod.ModProj
{
    public class RenderDistort : ModProjectile
    {
        public override string Texture => "MyMod/Images/FlowMap";//将弹幕材质定向到我们的FlowMap图片(用不着但是至少要过编译吧)
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PhantasmalBolt);
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 0;
            Projectile.hostile = false;
            Projectile.timeLeft = 100;
            base.SetDefaults();
        }
        public override void AI()
        {
            Projectile.scale += 0.02f;
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)//不要绘制弹幕，我们要在RT那一起绘制
        {
            return false;
        }
        public static void DrawAllDistortProjectile(SpriteBatch sb)
        {
            Texture2D t = ModContent.Request<Texture2D>("MyMod/Images/FlowMap").Value;
            foreach (var projectile in Main.projectile)//遍历所有的弹幕
            {
                if (projectile.type == ModContent.ProjectileType<RenderDistort>() && projectile.active)//符合条件的就绘制
                {
                    sb.Draw(t, projectile.Center - Main.screenPosition, null, Color.White,0, t.Size() / 2,
                        projectile.scale, SpriteEffects.None, 0);//画出这个弹幕来,大小由弹幕的scale控制
                }
            }
        }
    }
   
}