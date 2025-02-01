using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod
{
	public class ParticleWeapon : ModItem //伤害数字类跳字示范
    {
        public override string Texture => "Terraria/Images/Projectile_9";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.TerraBlade);//为了省事可以直接克隆泰拉刃的属性
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("高级粒子发生示例");
            base.SetStaticDefaults();
        }
        public override void HoldItem(Player player)//玩家握持该物品时，每游戏刻（tick）执行一次
        {
            if(Main.GameUpdateCount % 20 == 0)//每20游戏刻(即1/3秒)执行一次
            {
                for (int i = 0; i < 10; i++)//每次生成10个高级粒子
                {
                    ParticleOrchestraSettings p = default(ParticleOrchestraSettings);//声明一个高级粒子生成器
                    p.MovementVector = Main.rand.NextVector2Circular(10, 10);//速度设为横轴与纵轴半径为10的圆内的随机向量
                    p.PositionInWorld = player.Center;//初始位置为玩家中心
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.RainbowRodHit, p);
                    //生成一个彩虹法杖的粒子
                }
            }
            base.HoldItem(player);
        }
    }
}