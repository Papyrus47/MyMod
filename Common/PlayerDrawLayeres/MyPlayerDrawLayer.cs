using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace MyMod.Common.PlayerDrawLayeres
{
    // 我的玩家绘制层教学
    public class MyPlayerDrawLayer : PlayerDrawLayer
    {
        // 这里获取绘制层的位置
        // BeforeParent(PlayerDrawLayers.HeadBack) 表示在头部后面
        // AfterParent是前面，BeforeParent是后面
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if(drawInfo.drawPlayer.HeldItem != null && !drawInfo.drawPlayer.HeldItem.IsAir)
                return false; // 手持物品隐藏
            return true; // 正常默认绘制
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            // 这里添加绘制到drawInfo里面
            Texture2D value = TextureAssets.Item[ItemID.Zenith].Value;
            drawInfo.DrawDataCache.Add(new DrawData(
                // 按绘制一样的格式添加绘制数据
                value, // 绘制的物品贴图
                drawInfo.drawPlayer.Top - new Vector2(0,value.Height) - Main.screenPosition, // 绘制的位置
                null, // 绘制帧图
                Color.White, // 绘制颜色
                Main.GlobalTimeWrappedHourly, // 旋转角
                value.Size() * 0.5f, // 绘制起点位置
                1f, // 缩放比例
                SpriteEffects.None // 绘制翻转
                ));
        }
    }
}
