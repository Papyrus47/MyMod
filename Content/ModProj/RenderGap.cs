
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
    public class RenderGap : ModProjectile //空间撕裂
    {
        public override string Texture => "MyMod/Images/FlowMap";//将弹幕材质定向到我们的FlowMap图片(用不着但是至少要过编译吧)
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PhantasmalBolt);
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 0;
            Projectile.hostile = false; Projectile.timeLeft = 40;
            base.SetDefaults();
        }
        public override void AI()
        {
            Projectile.ai[0] = (float)Math.Sin(Projectile.timeLeft / 40f * 3.14159265f);
            MyMod.GapEffectProj = Projectile.whoAmI;//将这个弹幕的序号传进主类的静态字段，方便我们在rt用
            base.AI();
        }
        public override bool PreDraw(ref Color lightColor)//不要绘制弹幕，我们要在RT那一起绘制
        {
            return false;
        }
    }

}