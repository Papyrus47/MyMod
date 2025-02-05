using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI.Chat;

namespace MyMod.Content.Items.Weapons
{
    /// <summary>
    /// 小天使做的
    /// 雷霆之斧！
    /// </summary>
    public class LightingAxe : ModItem
    {
        /// <summary>
        /// 缓存的顶点数据
        /// </summary>
        public List<CustomVertexInfo> lightingCustomVertexCeche = new();
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 44;
            Item.maxStack = 1;
            Item.damage = 50;
            Item.scale = 2f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(10);
            Item.useTurn = false; //禁止使用转向
            Item.autoReuse = false; // 禁止自动重用
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.shoot = ModContent.ProjectileType<MyLightingProj>();
            Item.shootSpeed = 3f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
            return false;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //if (true) // 物品掉落时间
            //{
            //    lightingCustomVertexCeche.Clear(); // 清空缓存

            //    position = Main.LocalPlayer.Top;
            //    Vector2 Size = Vector2.UnitX.RotatedBy((Main.MouseWorld - position).ToRotation()) * 700 * scale; // 获得大小
            //    float count = 20;
            //    Vector2 pos = new(position.X, position.Y); // 获得文字的起始位置
            //    Vector2 endPos = pos + Size; // 获得文字的结束位置
            //    Vector2 startVel = endPos - pos; // 起始速度
            //    Vector2 vel = startVel / count; // 速度
            //    Vector2 drawPos = pos; // 绘制起始位置
            //    for (int i = 0; i < count; i++)
            //    {
            //        if (Math.Abs((endPos - drawPos).ToRotation() - vel.ToRotation()) > 0.8f)
            //            vel = vel.RotatedBy(((endPos - drawPos).ToRotation() - vel.ToRotation() > 0).ToDirectionInt() * 0.5f);
            //        else
            //            vel = vel.RotatedBy(Main.rand.NextFloat(0.3f, 0.8f) * Main.rand.NextBool().ToDirectionInt());
            //        vel = vel.RotatedBy(((endPos - drawPos).ToRotation() - vel.ToRotation()) * ((i + 1) / count));
            //        lightingCustomVertexCeche.Add(new(drawPos - Main.screenPosition, Color.LightBlue, new Vector3(i / count, 0, 0)));
            //        drawPos += vel;
            //    }
            //}
            //var gd = Main.instance.GraphicsDevice;
            //gd.Textures[0] = TextureAssets.MagicPixel.Value;
            //gd.DrawUserPrimitives(PrimitiveType.LineStrip, lightingCustomVertexCeche.ToArray(), 0, lightingCustomVertexCeche.Count - 1);
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {

        }
    }
}
