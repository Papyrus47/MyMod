using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI.Chat;

namespace MyMod.Content.Items
{
    /// <summary>
    /// 小天使做的
    /// 雷霆之斧！
    /// </summary>
    public class LightingAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 44;
            Item.maxStack = 1;
            Item.damage = 50;
            Item.scale = 2f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(10);
            Item.useTurn = false; //禁止使用转向
            Item.autoReuse = false; // 禁止自动重用
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.knockBack = 4;
            //Item.shoot = ProjectileID.PurificationPowder;
            //Item.shootSpeed = 16f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            line.Font
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            foreach (DrawableTooltipLine line in lines)
            {
                if (line.Mod == "Terraria" && line.Name == "ItemName")
                {
                    Vector2 Size = ChatManager.GetStringSize(line.Font, line.Text,line.BaseScale); // 获得文字的大小

                    CustomVertexInfo[] customVertexInfos = new CustomVertexInfo[4]; // 4个顶点

                    GraphicsDevice gd = Main.instance.GraphicsDevice; // 获得GraphicsDevice
                    
                    break;
                }
            }
        }
    }
}
