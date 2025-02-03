using System.Collections.Generic;

namespace MyMod.Common
{
    public interface IBasicSkillProj
    {
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public void Init();
        public void SwitchSkill()
        {
            if (CurrentSkill.SkillTimeOut)
            {
                if (PreSkillTimeOut())
                {
                    var targetSkill = OldSkills[0];
                    CurrentSkill.OnSkillDeactivate();
                    targetSkill.OnSkillActive();
                    CurrentSkill = targetSkill;
                    OldSkills.Clear();
                }
                return;
            }
            foreach (var targetSkill in CurrentSkill.switchToSkill)
            {
                if (CurrentSkill.SwitchCondition() && targetSkill.ActivationCondition() || targetSkill.CompulsionSwitchSkill(CurrentSkill))
                {
                    CurrentSkill.OnSkillDeactivate();
                    targetSkill.OnSkillActive();
                    OldSkills.Add(CurrentSkill);
                    CurrentSkill = targetSkill;
                    return;
                }
            }
        }
        /// <summary>
        /// 返回false阻止直接调用回起点
        /// </summary>
        /// <returns></returns>
        public bool PreSkillTimeOut() => true;
    }
}
