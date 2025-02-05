using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace MyMod.Content.Items
{
    /// <summary>
    /// 我的雷电弹幕
    /// </summary>
    public class MyLightingProj : ModProjectile
    {
        public List<CustomVertexInfo> customVertexInfos = new();
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.None}";
        public override void SetDefaults()
        {
            Projectile.width = 3;
            Projectile.height = 3;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 10; // 额外的更新次数
            Projectile.timeLeft = 200; // 弹幕持续时间
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000000;
        }
        public override void AI()
        {
            if(Projectile.timeLeft % 20 == 0) // 每20次更新添加两个顶点
            {
                float factor = Projectile.timeLeft / 200f;
                Vector2 pos = new(Projectile.ai[0], Projectile.ai[1]);
                float rot = (pos - Projectile.Center).ToRotation();
                Projectile.velocity = rot.ToRotationVector2() * Projectile.velocity.Length();
                Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4);
                if (Projectile.timeLeft % 40 == 0 && Projectile.ai[2] < 2) // 生成自己的节点
                {
                    Vector2 vel = Projectile.velocity.RotatedByRandom(0.1);
                    var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.Center.X + vel.X * 200, Projectile.Center.Y + vel.Y * 200, ++Projectile.ai[2]);
                    proj.timeLeft = Projectile.timeLeft;
                }
                //Projectile.velocity = Projectile.velocity.RotatedBy(rot * (1f - factor)); // 对雷电进行修正
                //if (customVertexInfos.Count > 30)
                //{
                //    customVertexInfos.RemoveAt(0);
                //    customVertexInfos.RemoveAt(0);
                //}
                customVertexInfos.Add(new(Projectile.Center + (Projectile.velocity.SafeNormalize(default).RotatedBy(MathHelper.PiOver2) * Projectile.width * factor) - Main.screenPosition, Color.LightBlue with { A = 0 }, new Vector3(factor, 0, 0)));
                customVertexInfos.Add(new(Projectile.Center + (Projectile.velocity.SafeNormalize(default).RotatedBy(-MathHelper.PiOver2) * Projectile.width * factor) - Main.screenPosition, Color.LightBlue with { A = 0 }, new Vector3(factor, 1, 0)));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if(customVertexInfos.Count > 2)
            {
                var gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = TextureAssets.MagicPixel.Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, customVertexInfos.ToArray(), 0, customVertexInfos.Count - 2);
            }
            return false;
        }
    }
}
