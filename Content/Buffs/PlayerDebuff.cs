using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Buffs//命名空间要和文件夹路径一样
{
    public class PlayerDebuff : ModBuff//继承modbuff类
    {

        public override void SetStaticDefaults()//注意，本函数只会在加载模组时配置一次，所以热重载无效
        {
            // DisplayName.SetDefault("玩家负面DeBUFF");//buff名称
            // Description.SetDefault("负面BUFF示例\n生命缓慢损失");//buff的提示
            Main.debuff[Type] = false;//为true时就是debuff
            Main.buffNoSave[Type] = false;//为true时退出世界时buff消失
            Main.buffNoTimeDisplay[Type] = false;//为true时不显示剩余时间

            //以下为debuff不可被护士去除
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            //以下为专家模式Debuff持续时间是否延长
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)//这个函数是当玩家获得BUFF时每帧执行一次，玩家大多数属性每帧重置
        {
            player.lifeRegen = -115;//扣血BUFF实现原理就是这样,一点liferegen代表每秒恢复或损失1/2生命值
                                    //player.moveSpeed -= 1.5f;//减速，具体数值需要你自己尝试
        }
    }
}