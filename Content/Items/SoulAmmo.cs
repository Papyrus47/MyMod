
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyMod.Content.Items
{
    public class SoulAmmo : ModItem
    {
        public override string Texture => "Terraria/Images/Projectile_258";
        public override void SetStaticDefaults()
        {

            // DisplayName.SetDefault("幽灵弹药");
            // Tooltip.SetDefault("使用这个弹药可以发射幽灵凤凰");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;//旅途模式复制条件
        }

        public override void SetDefaults()
        {
            Item.damage = 12; // 弹药伤害是和武器叠加作用的，不要设置的太高
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 2048;//最大堆叠数量
            Item.consumable = true; // 标记为可消耗
            Item.knockBack = 1.5f;//击退力
            Item.value = 10;//价值
            Item.rare = ItemRarityID.Green;//稀有度
            Item.shoot = ProjectileID.DD2PhoenixBowShot;// 代表使用这个弹药会发射什么弹幕
            Item.shootSpeed = 10f; // 子弹速度直接取决于弹药
            Item.ammo = Item.type; // 这个弹药属于哪类弹药，因为这是一种新的弹药类，直接使这个弹药ID等于物品ID即可（如果还想创建其他的同类弹幕，弹药ID请写成这个物品ID）


        }

        // 空手合成
        public override void AddRecipes()
        {
            CreateRecipe()

                .Register();
        }
    }
    public class SoulAmmo2 : ModItem
    {
        public override string Texture => "Terraria/Images/Projectile_258";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("幽灵弹药II");
            // Tooltip.SetDefault("使用这个弹药可以发射强化星云烈焰");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;//旅途模式复制条件
        }

        public override void SetDefaults()
        {
            Item.color = Color.Blue;//随便给物品染个色
            Item.damage = 12; // 弹药伤害是和武器叠加作用的，不要设置的太高
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 2048;//最大堆叠数量
            Item.consumable = true; // 标记为可消耗
            Item.knockBack = 1.5f;//击退力
            Item.value = 10;//价值
            Item.rare = ItemRarityID.Green;//稀有度
            Item.shoot = ProjectileID.NebulaBlaze2;// 代表使用这个弹药会发射什么弹幕
            Item.shootSpeed = 10f; // 子弹速度直接取决于弹药

            Item.ammo = ModContent.ItemType<SoulAmmo>(); // 这个弹药和上面的是一类，所以我们把它的ammo也写成上面的的物品ID


        }

        // 空手合成
        public override void AddRecipes()
        {
            CreateRecipe()

                .Register();
        }
    }
}
