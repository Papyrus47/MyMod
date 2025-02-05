using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Buffs//命名空间要和文件夹路径一样
{
    public class NPCdebuff : ModBuff//继承modbuff类
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("NPCdeBUFF");//buff名称
            // Description.SetDefault("npc用的buff一般不用写这个");//buff的提示
            Main.debuff[Type] = true;//为true时就是debuff
            Main.buffNoSave[Type] = false;//为true时退出世界时buff消失
            Main.buffNoTimeDisplay[Type] = false;//为true时不显示剩余时间

            //以下为debuff不可被护士去除
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            //以下为专家模式Debuff持续时间是否延长
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
        //我们做一个让NPC扣血的debuff
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen = -50;//每秒扣除25血量
            base.Update(npc, ref buffIndex);
        }
    }
}