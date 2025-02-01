﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Buffs//命名空间要和文件夹路径一样
{
	public class MinionBuff : ModBuff //召唤物所用的BUFF
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("泰拉马桶");
			// Description.SetDefault("泰拉马桶将为你而战！\n注意戴好防毒面具");

			Main.buffNoSave[Type] = true; // 退出世界时BUFF消失
			Main.buffNoTimeDisplay[Type] = true; // 不显示时间
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// 如果召唤物存在，那么延长buff时间，否则删除BUFF
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ModProj.MyMinion>()] > 0)//检测玩家持有的弹幕数量
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}