using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMod.Content.DamageClasses
{
    /// <summary>
    /// 第一个伤害类型
    /// </summary>
    public class FristDamageClass : DamageClass
    {
        /// <summary> 
        /// 这允许你定义这个 <see cref="DamageClass"/> 将被视为哪些其他类别（除了自身），用于盔甲和饰品效果，例如幽灵盔甲对魔法攻击产生的闪电，或岩浆石对近战攻击触发的地狱之火减益。<br/>
        /// 有关更详细的解释和示例，请参阅 <see href="https://github.com/tModLoader/tModLoader/blob/stable/ExampleMod/Content/DamageClasses/ExampleDamageClass.cs">ExampleMod的ExampleDamageClass.cs</see>
        /// 这个方法仅用于被重写。模组制作者应该调用 <see cref="DamageClass.CountsAsClass"/> 来查询效果继承。
        /// </summary>
        /// <remarks>对于你想要继承的每个 <see cref="DamageClass"/> 返回 <see langword="true"/></remarks>
        /// <param name="damageClass">你想要继承效果的 <see cref="DamageClass"/>。</param>
        /// <returns>默认返回 <see langword="false"/> - 这不允许其他类别的效果触发在这个 <see cref="DamageClass"/> 上。</returns>

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            #region 示例代码
            if(damageClass == Summon) // 猜猜为什么没有召唤师
            {
                return false;
            }
            #endregion
            return true; // 这里返回true表示这个伤害类型可以允许其他伤害类型效果实现
        }
        /// 这允许你定义这个 <see cref="DamageClass"/> 将从哪些其他类别（除了自身）受益，用于统计上的属性增益，例如伤害和暴击率。
        /// 这用于允许详细的指定你的伤害类别可以从哪些其他类别的属性增益中受益或不受益。
        /// </summary>
        /// <param name="damageClass">你希望这个 <see cref="DamageClass"/> 从统计上受益的 <see cref="DamageClass"/>。</param>
        /// <returns>默认情况下，这将返回 <see cref="StatInheritanceData.Full"/> 对于 <see cref="DamageClass.Generic"/>，对于其他类别则返回 <see cref="StatInheritanceData.None"/>。</returns>
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if(damageClass == Melee) // 这里是近战伤害类别
            {
                return new(damageInheritance: 2, // 这里是伤害的增加倍率
                    critChanceInheritance: 10, // 这里是暴击率的增加倍率
                    attackSpeedInheritance: 1, // 这里是攻速的增加倍率
                    armorPenInheritance: 1, // 这里是护甲穿透的增加倍率
                    knockbackInheritance: 1);// 这里是击退的增加倍率
                
            }
            return base.GetModifierInheritance(damageClass);
        }
        /// <summary> 
        /// 这允许你定义这个 <see cref="DamageClass"/> 将被视为哪些其他类别（除了自身），用于前缀的目的。<br/>
        /// 这个方法仅用于被重写。模组制作者应该调用 <see cref="DamageClass.GetsPrefixesFor"/> 来查询前缀继承。
        /// </summary>
        /// <remarks>对于你想要继承的每个 <see cref="DamageClass"/> 返回 <see langword="true"/></remarks>
        /// <param name="damageClass">你想要继承前缀的 <see cref="DamageClass"/>。</param>
        /// <returns>默认返回 <see cref="GetEffectInheritance"/> - 允许从这个类别继承效果的任何类别的前缀生效并保留在这个 <see cref="DamageClass"/> 的物品上。</returns>

        public override bool GetPrefixInheritance(DamageClass damageClass)
        {
            return false;
        }
        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            // 这个方法允许你阻止某些常见统计提示行在与这个 DamageClass 关联的物品上显示。
            // 你可以使用的四个行名是 "Damage", "CritChance", "Speed", 和 "Knockback"。这四个行名默认都为 true，因此会显示。例如...
            if (lineName == "Speed")
                return false;

            return true;
            // 请注意，这个钩子不会永远存在；它只会在即将到来的对提示行整体进行重构之后被移除。
            // 一旦发生这种情况，将会展示一个更好、更灵活的解释说明如何实现这一点，并移除这个钩子。
        }
    }
}
