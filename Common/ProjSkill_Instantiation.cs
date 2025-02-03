using System.Collections.Generic;

namespace MyMod.Common
{
    public abstract class ProjSkill_Instantiation
    {
        public ModProjectile modProjectile;
        public Projectile Projectile;
        public List<ProjSkill_Instantiation> switchToSkill;
        public bool SkillTimeOut;
        public ProjSkill_Instantiation() { }
        public ProjSkill_Instantiation(ModProjectile modProjectile)
        {
            this.modProjectile = modProjectile;
            Projectile = modProjectile.Projectile;
            switchToSkill = new();
        }
        public void AddBySkill(params ProjSkill_Instantiation[] skill)
        {
            foreach (var skillItem in skill)
            {
                skillItem.AddSkill(this);
            }
        }
        public ProjSkill_Instantiation AddSkill(ProjSkill_Instantiation projSkill)
        {
            switchToSkill.Add(projSkill);
            return projSkill;
        }
        public List<ProjSkill_Instantiation> AddSkilles(params ProjSkill_Instantiation[] projSkill)
        {
            switchToSkill.AddRange(projSkill);
            return new List<ProjSkill_Instantiation>(projSkill);
        }
        public ProjSkill_Instantiation this[ProjSkill_Instantiation projSkill] => switchToSkill.Find(x => x == projSkill);
        public virtual void AI() { }
        public virtual bool PreDraw(SpriteBatch sb, ref Color lightColor) => true;
        public virtual void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) { }
        public virtual bool? CanDamage() => null;
        public virtual bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        /// <summary>
        /// 激活这个技能的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool ActivationCondition() => true;
        /// <summary>
        /// 技能切换的条件
        /// </summary>
        /// <returns>返回true 则可以切换技能</returns>
        public virtual bool SwitchCondition() => true;
        /// <summary>
        /// 强制切换到这个技能,无视前者的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => false;
        /// <summary>
        /// 技能切换后调用
        /// </summary>
        public virtual void OnSkillDeactivate() { }
        /// <summary>
        /// 技能激活时调用
        /// </summary>
        public virtual void OnSkillActive() { }
    }
}
