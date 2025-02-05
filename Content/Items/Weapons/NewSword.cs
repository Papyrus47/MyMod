using Microsoft.Xna.Framework;
using MyMod.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Items.Weapons
{
    public class NewSword : ModItem//进阶剑,本剑旨在教学制作一把更自由的近战武器（弹幕）(有刀光教学)
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.shoot = ModContent.ProjectileType<ModProj.MySwordProj>();//发射进阶弹幕
            Item.shootSpeed = 1f;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Rapier;//rapier是必须的
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Pink;
            Item.scale = 3;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override void ModifyItemScale(Player player, ref float scale)
        {
            base.ModifyItemScale(player, ref scale);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float maxRotate = 3.5f;//定义最大挥舞弧度
            if (velocity.X >= 0)//如果你向右攻击的话
            {
                var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
                  type, damage, knockback, player.whoAmI, 1, maxRotate / 20f * 0.75f * player.GetAttackSpeed(DamageClass.Melee));
                p.scale = Item.scale;
                p.rotation = velocity.ToRotation() - maxRotate / 2f;
            }
            else//反之
            {
                var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
                  type, damage, knockback, player.whoAmI, -1, maxRotate / 20f * 0.75f * player.GetAttackSpeed(DamageClass.Melee));
                p.scale = Item.scale;
                p.rotation = velocity.ToRotation() - maxRotate / 2f;
            }
            return false;
        }
    }
}