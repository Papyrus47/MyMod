﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using MyMod.Content.ModProj;

namespace MyMod.Content.Items.Weapons
{
    public class RayWeapon : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("激光弹幕法杖"); //为你的武器命名
            // Tooltip.SetDefault("这是一个激光弹幕的示例");//这是武器的提示语，利用\n进行换行
        }
        public override void SetDefaults()
        {
            //以下是武器物品的基本属性
            Item.damage = 78;//物品的基础伤害
            Item.crit = 7;//物品的暴击率
            Item.DamageType = DamageClass.Magic;//物品的伤害类型
            Item.width = 40;//物品以掉落物形式存在的碰撞箱宽度
            Item.height = 40;//物品以掉落物形式存在的碰撞箱高度
            Item.useTime = 5;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)

            //这里用我们制作的手持弹幕ID
            Item.shoot = ModContent.ProjectileType<DeathRay>();//物品发射的弹幕ID

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
            Item.scale = 1.5f;//物品使用动画的大小
            Item.staff[Type] = true;
            //以下是手持弹幕必须要的属性
            Item.channel = true;//只有他为真，才能让弹幕判断玩家是否按住
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//创建一个配方

            recipe.Register();//空手合成
        }
    }
}