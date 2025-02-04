using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;

namespace MyMod.Content.Items
{
    /// <summary>
    /// 第三把教程枪
    /// </summary>
    public class ThirdGun : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.CoinGun}"; // 材质路径，使用硬币枪的材质
        public override void SetDefaults()
        {
            Item.damage = 100; // 伤害
            Item.useTime = 10; // 攻击持续时间
            Item.useAnimation = 10; // 动画持续时间
            Item.useStyle = ItemUseStyleID.Shoot; // 武器使用方式
            Item.noMelee = true; // 无法近战,也就是说武器本体啊，不能造成伤害
            Item.knockBack = 2; // 击退
            Item.value = Item.sellPrice(1); // 价值
            Item.rare = ItemRarityID.Red; // 稀有度
            Item.shoot = ProjectileID.Bullet; // 射出去的弹幕
            Item.shootSpeed = 16f; // 射出去的弹幕的速度
            Item.useAmmo = AmmoID.Bullet; // 消耗的弹药类型
            Item.width = 20; // 掉落物的宽度
            Item.height = 20;
            Item.noUseGraphic = false; // 物品使用时是否显示动画
            Item.autoReuse = true; // 自动重用
        }
        public override Vector2? HoldoutOffset()
        {
            // 允许你确定玩家在使用投射物武器时，手握住的武器精灵图上的偏移量。
            // 这仅用于使用样式（useStyle）为5的物品。返回null以使用默认的偏移量；默认返回null。
            return new Vector2(-10,0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 射击函数，这里是重写的，主要是为了添加一些额外的功能
            // 这里我们射出去三连发
            // 等会调试时候我们会改为散弹效果
            for(int i = 1; i <= 3; i++) // 循环三次
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy((i - 2) * 0.2f).RotatedByRandom(0.2), type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
