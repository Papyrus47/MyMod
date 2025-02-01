using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Items
{
	public class FirstSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("第一把剑"); //为你的武器命名
			// Tooltip.SetDefault("这是你的第一把剑.\n这是一个近战武器的示例。");//这是武器的提示语，利用\n进行换行
		}
		public override void SetDefaults()
		{
			//以下是武器物品的基本属性
			Item.damage = 155;//物品的基础伤害
			Item.crit = 10;//物品的暴击率
			Item.DamageType = DamageClass.Melee;//物品的伤害类型
			Item.width = 40;//物品以掉落物形式存在的碰撞箱宽度
			Item.height = 40;//物品以掉落物形式存在的碰撞箱高度
			Item.useTime = 20;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)
			Item.shoot = ProjectileID.TerraBeam;//物品发射的弹幕ID(泰拉剑气)
			Item.shootSpeed = 14f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）
			Item.useAnimation = 20;//物品播放使用动画所经历的时间
			Item.useStyle = ItemUseStyleID.Swing;//使用动作 swing为挥舞
			Item.knockBack = 6;//物品击退
			Item.value = Item.buyPrice(1,22,0,0);//价值  buyprice方法可以直接设置成直观的钱币数
			Item.rare = ItemRarityID.Pink;//稀有度
			Item.UseSound = SoundID.Item1;//使用时的声音
			Item.autoReuse = true;//自动连发
			//以下是武器进阶属性
			Item.noUseGraphic = false;//为true时会隐藏物品使用动画
			Item.noMelee = false;//为true时会取消物品近战判定
			Item.useAmmo = AmmoID.None;//为其他AmmoID时可以消耗指定弹药
			Item.mana = 0;//为大于零的数时每次使用会消耗魔力值
			Item.scale = 2.5f;//物品作为近战武器时的判定大小
		}
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
			//本函数是武器近战挥舞时触发的操作，通常为生成粒子
			Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Torch, 0, 0, 0, default, 2);
            base.MeleeEffects(player, hitbox);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			//本函数用于在武器执行发射弹幕时的操作，返回false可阻止武器原本的发射。true则保留。
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback,player.whoAmI);
			Projectile.NewProjectile(source, position,velocity.RotatedBy(0.2f),type,damage,knockback,player.whoAmI);
			Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.2f), type, damage, knockback, player.whoAmI);
			//这里我额外生成两个散射剑气,注意rotatedby是将向量偏转指定弧度，（6.28也就是2PI为一圈）
			//生成一个弹幕，source是生成源，直接使用参数即可。第二个参数是生成位置，position在玩家处。
			//第三个参数是速度，决定弹幕的初始速度（二维向量），第四个参数是ID，第五个参数是伤害，第六个参数是鸡腿
			//第七个参数是弹幕所有者的索引，通常有player参数时直接填player.whoami，不填这个参数可能会引发错误。
			return false;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
			//本函数为近战攻击到NPC时进行的操作，通常为产生弹幕或者BUFF
			target.AddBuff(BuffID.OnFire, 120);//addbuff方法第一个参数为要上的BUFFID，第二个为持续时间（帧）
			player.AddBuff(BuffID.NebulaUpLife3, 30);//为玩家添加半秒星云回复BUFF
     
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