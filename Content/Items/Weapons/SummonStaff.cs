using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Items.Weapons
{
    public class SummonStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("泰拉马桶法杖"); //为你的武器命名
            // Tooltip.SetDefault("这是一个召唤武器的示例。");//这是武器的提示语，利用\n进行换行
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // 让玩家可以点击屏幕任何地方
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }
        public override void SetDefaults()
        {
            //以下是武器物品的基本属性  召唤杖的属性和近战武器类似，去掉近战判定，改掉伤害属性就是召唤杖了
            Item.damage = 288;//物品的基础伤害
            Item.crit = 0;//物品的暴击率
            Item.DamageType = DamageClass.Summon;//物品的伤害类型
            Item.width = 40;//物品以掉落物形式存在的碰撞箱宽度
            Item.height = 40;//物品以掉落物形式存在的碰撞箱高度
            Item.useTime = 25;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)
            Item.useAnimation = 25;//物品播放使用动画所经历的时间
            Item.shoot = ModContent.ProjectileType<ModProj.MyMinion>();//发射仆从

            Item.shootSpeed = 1f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）

            Item.useStyle = ItemUseStyleID.Swing;//使用动作 swing为挥舞 shoot为射击
            Item.knockBack = 2;//物品击退
            Item.value = Item.buyPrice(1, 22, 0, 0);//价值  buyprice方法可以直接设置成直观的钱币数
            Item.rare = ItemRarityID.Pink;//稀有度
            Item.UseSound = SoundID.Item44;//使用时的声音
            Item.autoReuse = true;//自动连发
                                  //以下是武器进阶属性
            Item.noUseGraphic = false;//为true时会隐藏物品使用动画
            Item.noMelee = true;//为true时会取消物品近战判定
            Item.useAmmo = AmmoID.None;//为其他AmmoID时可以消耗指定弹药
            Item.mana = 20;//为大于零的数时每次使用会消耗魔力值
            Item.scale = 1.2f;//物品使用动画的大小
            Item.buffType = ModContent.BuffType<Buffs.MinionBuff>();//使用时为玩家赋予召唤物的BUFF

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 给予玩家BUFF保证召唤物存活
            player.AddBuff(Item.buffType, 3);

            // 召唤物需要设置originalDamage
            var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;

            //返回false阻止原版发射
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.TerraToilet);//加入材料
            recipe.AddIngredient(ItemID.Wood, 10);//添加第二种材料（10木材）
            recipe.AddTile(TileID.MythrilAnvil);//加入合成站(秘银和山铜砧共用ID)
            recipe.Register();
        }
    }
}