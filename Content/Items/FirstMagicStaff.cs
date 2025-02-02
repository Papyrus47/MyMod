using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Items
{
    public class FirstMagicStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("第一把法杖"); //为你的武器命名
            // Tooltip.SetDefault("这是你的第一把法杖.\n这是一个魔法武器的示例。");//这是武器的提示语，利用\n进行换行
        }
        public override void SetDefaults()
        {
            //以下是武器物品的基本属性
            Item.damage = 88;//物品的基础伤害
            Item.crit = 7;//物品的暴击率
            Item.DamageType = DamageClass.Magic;//物品的伤害类型
            Item.width = 40;//物品以掉落物形式存在的碰撞箱宽度
            Item.height = 40;//物品以掉落物形式存在的碰撞箱高度
            Item.useTime = 5;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)
            Item.shoot = ProjectileID.Blizzard;//物品发射的弹幕ID(暴雪)
            Item.shootSpeed = 24f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）
            Item.useAnimation = 5;//物品播放使用动画所经历的时间
            Item.useStyle = ItemUseStyleID.Shoot;//使用动作 swing为挥舞 shoot为射击
            Item.knockBack = 2;//物品击退
            Item.value = Item.buyPrice(1, 22, 0, 0);//价值  buyprice方法可以直接设置成直观的钱币数
            Item.rare = ItemRarityID.Pink;//稀有度
            Item.UseSound = SoundID.Item4;//使用时的声音
            Item.autoReuse = true;//自动连发
                                  //以下是武器进阶属性
            Item.noUseGraphic = false;//为true时会隐藏物品使用动画
            Item.noMelee = true;//为true时会取消物品近战判定
            Item.useAmmo = AmmoID.None;//为其他AmmoID时可以消耗指定弹药
            Item.mana = 10;//为大于零的数时每次使用会消耗魔力值
            Item.scale = 1.2f;//物品使用动画的大小
            Item.staff[Item.type] = true;//让本武器为法杖类型（因为贴图是45度，如果不这么做发射时就会倾斜45度）
                                         //如果你直接画一个朝右的法杖，就不需要了(0度朝右)
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //为了让弹道更生动，我们需加入随机偏转角让速度偏转
            float rot = Main.rand.NextFloat(-0.1f, 0.1f);//这是tml自带的随机数方法
                                                         //左参数为下限，右参数为上限，上限无法取到
                                                         //本函数用于在武器执行发射弹幕时的操作，返回false可阻止武器原本的发射。true则保留。
            Projectile.NewProjectile(source, position, velocity.RotatedBy(rot), type, damage, knockback, player.whoAmI);
            //生成一个弹幕，source是生成源，直接使用参数即可。第二个参数是生成位置，position在玩家处。
            //第三个参数是速度，决定弹幕的初始速度（二维向量），第四个参数是ID，第五个参数是伤害，第六个参数是鸡腿
            //第七个参数是弹幕所有者的索引，通常有player参数时直接填player.whoami，不填这个参数可能会引发错误。
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.IceBlock, 10);//加入材料(10冰块)
            recipe.AddIngredient(ItemID.Wood, 10);//添加第二种材料（10木材）
            recipe.AddTile(TileID.IceBlock);//加入合成站(这里为了有趣我改成了冰块)
            recipe.Register();
        }
    }
}