using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MyMod.Content.Buffs//命名空间要和文件夹路径一样
{
    public class FirstBuff : ModBuff//继承modbuff类
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("第一个BUFF");//buff名称
            /* Description.SetDefault("正面BUFF示例\n生命回复增加5点\n最大生命增加1000点\n最大魔力增加150点\n防御力增加10" +
				"\n所有攻击力和暴击率增加10%\n召唤栏位增加10\n+90%伤害减免\n近战攻击速度+100%"); *///buff的提示
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
            player.lifeRegen += 5;//生命回复+5
            player.statLifeMax2 += 1000;//最大生命+1000，注意，是lifemax2，lifemax是存档生命上限（吃生命水晶的那种）
            player.statManaMax2 += 150;//同理魔法值也是如此
            player.statDefense += 10;//防御力+10
            player.GetDamage(DamageClass.Generic) += 0.1f;//攻击力倍率可以加算也可以乘算，但是乘算容易数值膨胀
            player.GetCritChance(DamageClass.Generic) += 10f;//暴击率是以100为满值
            player.maxMinions += 10;//召唤上限+10
            player.endurance += 0.9f;//伤害减免+90%
            player.GetAttackSpeed(DamageClass.Melee) += 1f;//近战攻速+100%
        }
    }
}