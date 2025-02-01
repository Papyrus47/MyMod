
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System.Collections.Generic;
using MyMod.ModProj;

namespace MyMod.Items
{
	public class TempleBullet : ModItem
	{
		public override void SetStaticDefaults() {
		
			// DisplayName.SetDefault("神庙子弹");
			// Tooltip.SetDefault("在击中NPC时有4%概率额外造成对方最大生命1%的伤害");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;//旅途模式复制条件
		}

		public override void SetDefaults() {
			Item.damage = 12; // 弹药伤害是和武器叠加作用的，不要设置的太高
			Item.DamageType = DamageClass.Ranged;
			Item.width = 8;
			Item.height = 8;
			Item.maxStack = 2048;//最大堆叠数量
			Item.consumable = true; // 标记为可消耗
			Item.knockBack = 1.5f;//击退力
			Item.value = 10;//价值
			Item.rare = ItemRarityID.Green;//稀有度
			Item.shoot = ModContent.ProjectileType<TempleProj>(); // 代表使用这个弹药会发射什么弹幕
			Item.shootSpeed = 10f; // 子弹速度直接取决于弹药
			Item.ammo = AmmoID.Bullet; // 这个弹药属于哪类弹药，这里我们写子弹类。
			//原版的弹药类有：
			List<int> list = new List<int>() 
			{
				AmmoID.None,//不使用弹药
				AmmoID.Bullet,//子弹
				AmmoID.Arrow,//箭矢
				AmmoID.Rocket,//火箭弹
				AmmoID.Dart,//飞镖（吹管）
				AmmoID.CandyCorn,//玉米糖
				AmmoID.NailFriendly, //钉枪
				AmmoID.Gel,//凝胶（喷火器使用）
				AmmoID.JackOLantern,//南瓜灯
				AmmoID.Sand,//沙枪
				AmmoID.FallenStar,//落星
				AmmoID.Stake,//尖桩
			    AmmoID.Flare,//信号弹
				AmmoID.Snowball,//雪球
				AmmoID.Solution,//溶液（环境改造
				AmmoID.StyngerBolt//毒刺（石巨人的那把武器）
			};
			
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.EmptyBullet,200).AddIngredient(ItemID.LunarTabletFragment,1)
				//200空心子弹加一个日耀碑牌碎片制作
				.Register();//注册进去
		}
	}
}
