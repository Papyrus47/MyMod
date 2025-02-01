using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Items
{
	public class SecondGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("第2把枪"); //为你的武器命名
			// Tooltip.SetDefault("这是一个进阶武器的示例。");//这是武器的提示语，利用\n进行换行
		}
		public override void SetDefaults()
		{
			Item.useTime = 8;Item.useAnimation = 40;//为了制作发条枪类武器，使用时间要小于动画时间，使得
													//动画持续过程中可以多次使用,这里连续发射五发子弹
			Item.reuseDelay = 25;//下一次使用前的间隔,达到间歇性连续开火的效果
			Item.consumeAmmoOnFirstShotOnly = true;//这个属性代表只有第一次开火消耗弹药
												   										  
			//以下是武器物品的基本属性
			Item.damage = 45;//物品的基础伤害
			Item.crit = 20;//物品的暴击率
			Item.DamageType = DamageClass.Ranged;//物品的伤害类型
			Item.width = 40;//物品以掉落物形式存在的碰撞箱宽度
			Item.height = 40;//物品以掉落物形式存在的碰撞箱高度
			
			Item.shoot = ProjectileID.BlackBolt;//物品发射的弹幕ID(玛瑙炮)
			Item.shootSpeed = 24f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）
			
			Item.useStyle = ItemUseStyleID.Shoot;//使用动作 swing为挥舞 shoot为射击
			Item.knockBack = 2;//物品击退
			Item.value = Item.buyPrice(1,22,0,0);//价值  buyprice方法可以直接设置成直观的钱币数
			Item.rare = ItemRarityID.Pink;//稀有度
			Item.UseSound = SoundID.Item22;//使用时的声音
			Item.autoReuse = true;//自动连发
			//以下是武器进阶属性
			Item.noUseGraphic = false;//为true时会隐藏物品使用动画
			Item.noMelee = true;//为true时会取消物品近战判定

			Item.useAmmo = ModContent.ItemType<SoulAmmo>();//为其他AmmoID时可以消耗指定弹药

			Item.mana = 0;//为大于零的数时每次使用会消耗魔力值
			Item.scale = 1.2f;//物品使用动画的大小
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			/*Projectile p = Projectile.NewProjectileDirect(source, position, velocity, 
				ProjectileID.Flamelash//烈焰火鞭是魔法伤害弹幕，我们需要把它改成远程
				, damage, knockback,player.whoAmI);//NewProjectileDirect方法可以直接定义生成出来的这个弹幕
			p.DamageType = DamageClass.Ranged;*/
            return true;//返回真，执行原版发射
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();//创建一个配方
			recipe.AddIngredient(ItemID.Torch, 10);//加入材料(10火把)
			recipe.AddIngredient(ItemID.Wood, 10);//添加第二种材料（10木材）
			recipe.AddTile(TileID.Campfire);//加入合成站(这里为了有趣我改成了篝火)
			recipe.Register();
		}
	}
}