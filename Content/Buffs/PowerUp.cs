using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace MyMod.Content.Buffs
{
    public class PowerUp : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.MagicPower}";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // 显示是否为负面效果,也就是能不能手动取消
            Main.buffNoTimeDisplay[Type] = true; // 显示是否在Buff图标上显示时间,如果为false,则显示在Buff名字上
            Main.buffNoSave[Type] = false; // 显示是否在保存游戏时不会被保存
        }
        public override void Update(Player player, ref int buffIndex) // 这里是更新玩家的buff
        {
            player.GetDamage(DamageClass.Generic) += 1; // 给玩家增加100%的伤害
        }
        public override void Update(NPC npc, ref int buffIndex) // 这里是更新NPC的buff
        {
            base.Update(npc, ref buffIndex);
        }
    }
}
