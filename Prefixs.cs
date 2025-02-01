using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod
{
	// 这个类作为声明物品“前缀”或”修饰词条”的例子
	public class FirstPrefix : ModPrefix //这是一个通用武器前缀示例
	{
        public override string Name => "FirstPrefix";//这是前缀的内部名称，不代表显示名称，请你去hjson修改显示名称
        // 修改该前缀的类别，默认为 PrefixCategory.Custom。影响哪些物品可以获得此前缀
        public override PrefixCategory Category => PrefixCategory.AnyWeapon;//任何武器都可以使用
		List<PrefixCategory> categories = new List<PrefixCategory>() { 
		PrefixCategory.Melee,//近战武器
		PrefixCategory.Magic,//魔法武器
		PrefixCategory.Ranged,//远程武器
		PrefixCategory.AnyWeapon,//任何武器
		PrefixCategory.Custom,//自定义（需要自己写chooseprefix）
		PrefixCategory.Accessory//饰品
		};//tml本身并未设置给召唤武器的专属词条，因此没有指定召唤武器。
		  //但也不是做不到，那需要设置globalitem的allowprefix

		// 原版前缀的权重和更多信息参见tML文档
		// 当多个前缀有相似的作用时，可以与 switch 或 case 使用以为不同的前缀提供不同的概率
		// 注意：即使权重是0f，也有可能被抽到。排除前缀请参见 CanRoll（就在下面）
		// 注意：如果前缀的类别是 PrefixCategory.Custom，请在ModItem的ChoosePrefix函数进行筛选
		public override float RollChance(Item item)
		{
			return 5f;
		}

		// 决定该前缀是否能在重铸时随机到
		// 设为 true 就是能，false 就是不能
		public override bool CanRoll(Item item)
		{
			return true;
		}

		// 用这个方法来修改拥有此前缀的物品的属性：
		// damageMult 伤害乘数，knockbackMult 击退乘数，useTimeMult 使用时间乘数，scaleMult 大小乘数，
		// shootSpeedMult 弹速（射速，射出的速度）乘数，manaMult 魔力消耗乘数，critBonus 暴击增量
		public override void SetStats(ref float damageMult, ref float knockbackMult, 
			ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
		{
			damageMult *= 1f + 0.20f;
		}

		// 修改获得此前缀的物品的价格，valueMult 为价格乘数
		public override void ModifyValue(ref float valueMult)
		{
			valueMult *= 1f + 0.05f;
		}

		// 这个方法用来修改获得此前缀的物品的其它属性
		public override void Apply(Item item)
		{
			// 开始你的表演
		}
	}
	public class AccPrefix : ModPrefix //这是一个饰品前缀示例
	{
		public override string Name => "AccPrefix";//这是前缀的内部名称，不代表显示名称，请你去hjson修改显示名称
													 // 修改该前缀的类别，默认为 PrefixCategory.Custom。影响哪些物品可以获得此前缀
		public override PrefixCategory Category => PrefixCategory.Accessory;//饰品
		List<PrefixCategory> categories = new List<PrefixCategory>() {
		PrefixCategory.Melee,//近战武器
		PrefixCategory.Magic,//魔法武器
		PrefixCategory.Ranged,//远程武器
		PrefixCategory.AnyWeapon,//任何武器
		PrefixCategory.Custom,//自定义（需要自己写chooseprefix）
		PrefixCategory.Accessory//饰品
		};//tml本身并未设置给召唤武器的专属词条，因此没有指定召唤武器。
		  //但也不是做不到，那需要设置globalitem的allowprefix

		// 原版前缀的权重和更多信息参见tML文档
		// 当多个前缀有相似的作用时，可以与 switch 或 case 使用以为不同的前缀提供不同的概率
		// 注意：即使权重是0f，也有可能被抽到。排除前缀请参见 CanRoll（就在下面）
		// 注意：如果前缀的类别是 PrefixCategory.Custom，请在ModItem的ChoosePrefix函数进行筛选
		public override float RollChance(Item item)
		{
			return 5f;
		}

		// 决定该前缀是否能在重铸时随机到
		// 设为 true 就是能，false 就是不能
		public override bool CanRoll(Item item)
		{
			return true;
		}
		
		// 修改获得此前缀的物品的价格，valueMult 为价格乘数
		public override void ModifyValue(ref float valueMult)
		{
			valueMult *= 1.145f;
		}

		// 这个方法用来修改获得此前缀的物品的其它属性，例如物品防御力
		//然而，要想将这些加成写在物品描述中，你需要额外写tooltip
		public override void Apply(Item item)
		{
			item.defense += 10;//防御力+10
		}
	}
}