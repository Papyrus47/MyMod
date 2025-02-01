using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Items
{
	public class FirstGun : ModItem
	{
		public override void SetStaticDefaults()//1.4.4tml删除了所有代码内部的文本命名，转而使用hjson来添加翻译
		{
			// DisplayName.SetDefault("第一把枪"); //为你的武器命名
			// Tooltip.SetDefault("这是你的第一把枪.\n这是一个远程武器的示例。");//这是武器的提示语，利用\n进行换行
		}
		public override void SetDefaults()
		{
			//以下是武器物品的基本属性
			Item.damage = 65;//物品的基础伤害
			Item.crit = 20;//物品的暴击率
			Item.DamageType = DamageClass.Ranged;//物品的伤害类型
			Item.width = 88;//物品以掉落物形式存在的碰撞箱宽度
			Item.height = 38;//物品以掉落物形式存在的碰撞箱高度
			Item.useTime = 5;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)
			Item.shoot = ProjectileID.BlackBolt;//物品发射的弹幕ID(玛瑙炮)
			Item.shootSpeed = 24f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）
			Item.useAnimation = 5;//物品播放使用动画所经历的时间
			Item.useStyle = ItemUseStyleID.Shoot;//使用动作 swing为挥舞 shoot为射击
			Item.knockBack = 2;//物品击退
			Item.value = Item.buyPrice(1,22,0,0);//价值  buyprice方法可以直接设置成直观的钱币数
			Item.rare = ItemRarityID.Pink;//稀有度
			Item.UseSound = SoundID.Item22 with
			{
				MaxInstances = 114 //最大实例数
			};//使用时的声音
			Item.autoReuse = true;//自动连发
			//以下是武器进阶属性
			Item.noUseGraphic = false;//为true时会隐藏物品使用动画
			Item.noMelee = true;//为true时会取消物品近战判定
			Item.useAmmo = AmmoID.Bullet;//为其他AmmoID时可以消耗指定弹药
			Item.mana = 0;//为大于零的数时每次使用会消耗魔力值
			Item.scale = 1.2f;//物品使用动画的大小
		}
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
			//本函数是武器近战挥舞时触发的操作，通常为生成粒子
			//Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Torch, 0, 0, 0, default, 2);
            base.MeleeEffects(player, hitbox);
        }
        public override Vector2? HoldoutOffset()
        {
            // 允许你确定玩家在使用投射物武器时，手握住的武器精灵图上的偏移量。
            // 这仅用于使用样式（useStyle）为5的物品。返回null以使用默认的偏移量；默认返回null。
            return new Vector2(-18,-2);
        }

        public override Vector2? HoldoutOrigin()
        {
            //允许你确定玩家在使用此物品时，手握住的物品精灵图上的点。原点是从精灵图的左下角开始。这仅用于使用样式（useStyle）为5的法杖。返回null以使用默认的原点（零）；默认返回null。
            return base.HoldoutOrigin();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			player.AddBuff(ModContent.BuffType<Buffs.FirstBuff>(), 200);
            //本函数用于在武器执行发射弹幕时的操作，返回false可阻止武器原本的发射。true则保留。
            position += velocity.RotatedBy(0.15).SafeNormalize(default) * 80; // 修改发射子弹的位置,+velocity 是为发射位置修正
            for (int i = -2;i <= 2; i++)
            {
				float rotation = MathHelper.ToRadians(7.5f * i); // 计算子弹的旋转角度,-15~15度为一个子弹的旋转角度

                Projectile.NewProjectile(source, position, velocity.RotatedBy(rotation),
					type == ProjectileID.Bullet ? ProjectileID.BlackBolt : type //若子弹为火枪子弹，则换为黑玛瑙，其他子弹保留
					//type是当前消耗子弹的弹幕ID，不想让他射子弹对应的弹幕，可以改掉它。
					, damage, knockback, player.whoAmI) ;
            }
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
			//远程武器就不需要了
         
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