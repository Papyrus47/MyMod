using System.Collections.Generic;
using System.IO;

namespace MyMod.Common
{
    public interface IBasicSkillProj
    {
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill
        {
            get
            {
                return SkillsParis[ID];
            }
            set
            {
                if (!IDParis.ContainsKey(value))
                {
                    string name = value.GetType().Name;
                    int i = 0;
                    while (true)
                    {
                        i++;
                        if (!IDParis.ContainsValue(name) || !SkillsParis.ContainsKey(name))
                        {
                            name += i.ToString();
                            break;
                        }
                    }
                    IDParis.Add(value, name);
                    SkillsParis.Add(name, value);
                }
                ID = IDParis[value];
            }
        }
        public string ID { get; set; }
        public Dictionary<ProjSkill_Instantiation,string> IDParis { get; set; }
        public Dictionary<string, ProjSkill_Instantiation> SkillsParis { get; set; }
        public void SendData(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(ID);
        }
        public void ReceiveData(BinaryReader binaryReader)
        {
            ID = binaryReader.ReadString();
        }
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
