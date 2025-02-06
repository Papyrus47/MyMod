using MyMod.Content.ModProj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace MyMod.Content.Items.Weapons
{
    public class FristSpurtWeapon : ModItem
    {
        public override void SetDefaults()
        {
            const int USETIME = 10;
            //以下是武器物品的基本属性
            Item.damage = 65;//物品的基础伤害
            Item.crit = 20;//物品的暴击率
            Item.DamageType = DamageClass.MeleeNoSpeed;//物品的伤害类型,近战无攻速加成类
            Item.width = 42;//物品以掉落物形式存在的碰撞箱宽度
            Item.height = 42;//物品以掉落物形式存在的碰撞箱高度
            Item.useTime = USETIME;//物品一次使用所经历的时间（以帧为单位）(正常情况1秒60帧)
            Item.shoot = ModContent.ProjectileType<FristSpurtWeaponProj>();//物品发射的弹幕ID(玛瑙炮)
            Item.shootSpeed = 24f;//物品发射的弹幕速度（像素/帧）（一个物块长16像素）
            Item.useAnimation = USETIME;//物品播放使用动画所经历的时间
            Item.useStyle = ItemUseStyleID.Rapier; // 物品的使用方式,这里是短剑类
            Item.knockBack = 2;//物品击退
            Item.value = Item.buyPrice(1, 22, 0, 0);//价值  buyprice方法可以直接设置成直观的钱币数
            Item.rare = ItemRarityID.Pink;//稀有度
            Item.autoReuse = true;//自动连发
                                  //以下是武器进阶属性
            Item.noUseGraphic = true;//为true时会隐藏物品使用动画
            Item.noMelee = true;//为true时会取消物品近战判定
            Item.mana = 0;//为大于零的数时每次使用会消耗魔力值
            Item.scale = 1.2f;//物品使用动画的大小
            Item.channel = true;
        }
    }
}
