using Microsoft.Xna.Framework;
using MyMod.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Items
{
    public class SecondSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("精灵剑"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("进阶弹幕绘制示例以及buff示例");
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.shoot = ModContent.ProjectileType<ModProj.MoveProj>();//发射进阶弹幕
            Item.shootSpeed = 9.8f;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {

            SkyManager.Instance.Activate("BlackSky");
            target.AddBuff(ModContent.BuffType<NPCdebuff>(), 100);//对NPC上Debuff
            //NPC(player, target, damage, knockBack, crit);
        }
    }
}