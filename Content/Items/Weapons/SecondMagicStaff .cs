using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Items.Weapons
{
    public class SecondMagicStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("第?把法杖"); //为你的武器命名
            // Tooltip.SetDefault("这是一个魔法进阶武器的示例。");//这是武器的提示语，利用\n进行换行
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
            //本次学习如何从鼠标位置发射弹幕，以及化敌为友

            Vector2 pos = Main.MouseWorld;//先将鼠标在世界中的位置定义出来
            for (int i = 0; i < 4; i++)//我想让他从鼠标位置发射十字弹幕
            {
                float rotation = i * MathHelper.TwoPi / 4;//偏转角的设置
                Projectile p = Projectile.NewProjectileDirect(source
                    , pos //上面定义的位置
                    , new Vector2(10, 0).RotatedBy(rotation)//直接用向量设定初速度(水平向右10码速度)再偏转
                  , ProjectileID.RuneBlast//符文巫师的爆炸弹
                  , damage, knockback, player.whoAmI);
                p.hostile = false;//hostile就是弹幕的敌对属性，false阻止其为敌对弹幕
                p.friendly = true;//friendly是弹幕的友方属性，true使其为友方弹幕
                p.DamageType = DamageClass.Magic;//别忘了设置伤害属性

                //弹幕并不是非友即敌，也有无害的中立弹幕，也就是说这两条都为false
                //相反的，如果两条都为true，那就是无差别打击的弹幕
            }
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