using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Items
{
    public class TextWeapon_Combat : ModItem //伤害数字类跳字示范
    {
        public override string Texture => "Terraria/Images/Projectile_9";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodenSword);//为了省事可以直接克隆木剑的属性
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("伤害数字类跳字");
            base.SetStaticDefaults();
        }
        public override void HoldItem(Player player)//玩家握持该物品时，每游戏刻（tick）执行一次
        {
            if (Main.GameUpdateCount % 120 == 0)//每120游戏刻(即两秒)执行一次
            {
                CombatText.NewText(player.Hitbox,//跳字生成的矩形范围（这里我填写了玩家的碰撞箱）
                    Color.Cyan,//跳字的颜色
                  "这是一段伤害类跳字示例",//这里是你需要展示的文字
                  false,//dramatic为true可以使得字体闪烁，
                  false //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
                    );
            }
            base.HoldItem(player);
        }
    }
    public class TextWeapon_Popup : ModItem //重铸提示类跳字示范
    {
        public override string Texture => "Terraria/Images/Projectile_9";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodenSword);//为了省事可以直接克隆木剑的属性
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("重铸提示字类跳字");
            base.SetStaticDefaults();
        }
        public override void HoldItem(Player player)//握持该物品时每游戏刻（帧）执行一次
        {
            if (Main.GameUpdateCount % 120 == 0)//每120游戏刻(即两秒)执行一次
            {
                AdvancedPopupRequest Popup = default;//声明一个popup结构体
                Popup.Text = "这是一个提示类跳字示例";
                Popup.DurationInFrames = 60;//持续60游戏刻(一秒)
                Popup.Color = Color.Pink;//跳字颜色
                Popup.Velocity = new Vector2(0, -10);//跳字初速度
                PopupText.NewText(Popup, player.Center);//生成这个跳字
            }
            base.HoldItem(player);
        }
    }
    public class TextWeapon_Draw : ModItem //绘制文字示范
    {
        public override string Texture => "Terraria/Images/Projectile_9";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.CopperWatch);//为了省事可以直接克隆铜表的属性
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("绘制文字示范");
            base.SetStaticDefaults();
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Utils.DrawBorderString(spriteBatch, "这是一个绘制文字的示范", position, Color.White, 1);//这个方法可以绘制文字
            //可以在任何绘制函数中使用该方法
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}