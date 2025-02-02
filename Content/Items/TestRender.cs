using Microsoft.Xna.Framework;
using MyMod.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Items
{
    public class TestRender : ModItem
    {
        public override string Texture => "Terraria/Images/Item_1";
        public override void SetDefaults()
        {
            //以下是武器物品的基本属性
            Item.damage = 155;//物品的基础伤害
            Item.crit = 10;//物品的暴击率
            Item.DamageType = DamageClass.Melee;//物品的伤害类型
            Item.width = 40;//物品以掉落物形式存在的碰撞箱宽度
            Item.height = 40;//物品以掉落物形式存在的碰撞箱高度
            Item.useTime = 20;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)
            Item.shoot = ModContent.ProjectileType<ModProj.RenderGap>();//物品发射的弹幕ID(泰拉剑气)
            Item.shootSpeed = 14f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）
            Item.useAnimation = 20;//物品播放使用动画所经历的时间
            Item.useStyle = ItemUseStyleID.Swing;//使用动作 swing为挥舞
            Item.knockBack = 6;//物品击退
            Item.value = Item.buyPrice(1, 22, 0, 0);//价值  buyprice方法可以直接设置成直观的钱币数
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //本函数用于在武器执行发射弹幕时的操作，返回false可阻止武器原本的发射。true则保留。
            Projectile.NewProjectile(source, position, Vector2.Zero,
                ModContent.ProjectileType<ModProj.RenderGap>(), 0, 0, player.whoAmI);

            return false;
        }
        public override void HoldItem(Player player)
        {
            Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<RenderDust>(), 0, 0, 0, Color.White, 1);
            //生成RT粒子在玩家身上，当然这是范例，你可以自己随意生成
            base.HoldItem(player);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 10);//加入材料(10火把)
            recipe.AddIngredient(ItemID.Wood, 10);//添加第二种材料（10木材）
            recipe.AddTile(TileID.Campfire);//加入合成站(这里为了有趣我改成了篝火)
            recipe.Register();
        }
    }//空间斩测试
    public class TestRender2 : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.CobaltPickaxe;
        public override void SetDefaults()
        {
            //以下是武器物品的基本属性
            Item.damage = 155;//物品的基础伤害
            Item.crit = 10;//物品的暴击率
            Item.DamageType = DamageClass.Melee;//物品的伤害类型
            Item.width = 40;//物品以掉落物形式存在的碰撞箱宽度
            Item.height = 40;//物品以掉落物形式存在的碰撞箱高度
            Item.useTime = 20;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)
            Item.shoot = ModContent.ProjectileType<ModProj.RenderGap>();//物品发射的弹幕ID(泰拉剑气)
            Item.shootSpeed = 14f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）
            Item.useAnimation = 20;//物品播放使用动画所经历的时间
            Item.useStyle = ItemUseStyleID.Swing;//使用动作 swing为挥舞
            Item.knockBack = 6;//物品击退
            Item.value = Item.buyPrice(1, 22, 0, 0);//价值  buyprice方法可以直接设置成直观的钱币数
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //本函数用于在武器执行发射弹幕时的操作，返回false可阻止武器原本的发射。true则保留。
            Projectile.NewProjectile(source, position, Vector2.Zero,
                ModContent.ProjectileType<ModProj.RenderDistort>(), 0, 0, player.whoAmI);

            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 10);//加入材料(10火把)
            recipe.AddIngredient(ItemID.Wood, 10);//添加第二种材料（10木材）
            recipe.AddTile(TileID.Campfire);//加入合成站(这里为了有趣我改成了篝火)
            recipe.Register();
        }
    }//震波测试
}