using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;

namespace MyMod.Items.Accessories
{
    /// <summary>
    /// 这是第一个饰品
    /// 拥有增加玩家最大速度，提升玩家伤害，增加玩家生命值，提升玩家护甲的饰品
    /// </summary>
    public class FirstAccessories : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(); // 格式化参数
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1; // 旅途模式解锁数量
        }
        public override void SetDefaults()
        {
            Item.accessory = true; // 这是饰品
            Item.width = 24; // 宽24像素
            Item.height = 24; // 高24像素
            Item.maxStack = 1; // 最大堆叠数量

            Item.rare = ItemRarityID.Green; // 这是绿色的稀有度
            Item.value = Item.sellPrice(0, 10); //  10 金币
        }
        public override void AddRecipes()
        {
            CreateRecipe().Register(); // 注册配方 空手合成
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.1f; // 提升玩家伤害10%
            player.statLifeMax2 += 100; // 增加玩家最大生命值100
            player.statDefense.AdditiveBonus += 0.1f; // 提升玩家护甲10%
            player.accRunSpeed *= 1.2f; // 增加玩家最大速度20%
            if(hideVisual)
            {
                // 隐藏时候，额外增加10%伤害，增加100生命值，提升10%护甲，最大速度100%
                player.GetDamage(DamageClass.Generic) += 0.1f;
                player.statLifeMax2 += 100;
                player.statDefense.AdditiveBonus += 0.1f;
                player.accRunSpeed *= 2f;
            }
            else
            {
                // 显示时候，额外增加20%伤害，增加200生命值，提升20%护甲，最大速度200%
                player.GetDamage(DamageClass.Generic) += 0.2f;
                player.statLifeMax2 += 200;
                player.statDefense.AdditiveBonus += 0.2f;
                player.accRunSpeed *= 3f;
            }
        }
    }
}