using MyMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace MyMod.Content.ModProj.General
{
    /// <summary>
    /// 通用挥舞类
    /// </summary>
    public class SwingHelper_GeneralSwing : ProjSkill_Instantiation
    {
        public class PreAtk
        {
            /// <summary>
            /// 前摇时间
            /// </summary>
            public int PreTime = 15;
            /// <summary>
            /// 开始时候调用
            /// </summary>
            public Action<SwingHelper_GeneralSwing> OnStart;
            /// <summary>
            /// 发射在改变时候
            /// </summary>
            public Action<SwingHelper_GeneralSwing> OnChange;
        }
        public class OnAtk
        {
            public delegate void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers);
            /// <summary>
            /// 挥舞所需要时间
            /// </summary>
            public float SwingTime = 30;
            /// <summary>
            /// 挥舞变化方式
            /// </summary>
            public Func<float, float> TimeChange = (time) => time;
            /// <summary>
            /// 在这里使用特定AI
            /// </summary>
            public Action<SwingHelper_GeneralSwing> OnUse;
            /// <summary>
            /// 命中时候
            /// </summary>
            public Action<NPC, NPC.HitInfo, int> OnHit;
            /// <summary>
            /// 即将命中的时候
            /// </summary>
            public ModifyHitNPC ModifyHit;
        }
        public class PostAtk
        {
            /// <summary>
            /// 后摇最长时间
            /// </summary>
            public int PostMaxTime = 30;
            /// <summary>
            /// 后摇什么时候可以跳出
            /// </summary>
            public int PostAtkTime = 5;
            /// <summary>
            /// 结束时候调用
            /// </summary>
            public Action<SwingHelper_GeneralSwing> OnEnd;
        }
        public class Setting
        {
            /// <summary>
            /// 玩家朝向为1的时候,从哪个速度开始的向量(-1会自动调整)
            /// </summary>
            public Vector2 StartVel = Vector2.UnitX;
            /// <summary>
            /// 速度压缩变化
            /// </summary>
            public Vector2 VelScale = Vector2.One;
            /// <summary>
            /// 旋转的弧度
            /// </summary>
            public float SwingRot = MathHelper.Pi;
            /// <summary>
            /// 通过旋转实现透视
            /// </summary>
            public float VisualRotation;
            /// <summary>
            /// 挥舞的朝向
            /// </summary>
            public bool SwingDirectionChange = true;
            /// <summary>
            /// 如何使用这个技能
            /// </summary>
            public Func<bool> ChangeCondition = () => true;
            /// <summary>
            /// 动作值
            /// </summary>
            public float ActionDmg = 1;
            /// <summary>
            /// 挥舞长度
            /// </summary>
            public float SwingLenght = 10;
            public delegate bool PreDraw(SpriteBatch spriteBatch, Color drawColor);
            /// <summary>
            /// 必须设置的绘制
            /// </summary>
            public PreDraw preDraw = (_, _) => false;
        }
        public Setting setting;
        public PreAtk preAtk;
        public PostAtk postAtk;
        public OnAtk onAtk;
        public SwingHelper SwingHelper;
        public Player Player;
        public SoundStyle playSound = SoundID.Item1;
        /// <summary>
        /// 抖屏开关
        /// </summary>
        public bool CanMoveScreen = false;

        public SwingHelper_GeneralSwing(ModProjectile modProjectile, Setting setting, PreAtk preAtk, PostAtk postAtk, OnAtk onAtk, SwingHelper swingHelper, Player player) : base(modProjectile)
        {
            this.setting = setting;
            this.preAtk = preAtk;
            this.postAtk = postAtk;
            this.onAtk = onAtk;
            SwingHelper = swingHelper;
            Player = player;
        }
        public override void AI()
        {
            Projectile.spriteDirection = Player.direction * setting.SwingDirectionChange.ToDirectionInt();
            Player.heldProj = Projectile.whoAmI;

            switch ((int)Projectile.ai[0])
            {
                case 0: // 渐变
                    Projectile.ai[1]++;
                    float time = Projectile.ai[1] / preAtk.PreTime;
                    SwingHelper.Change_Lerp(setting.StartVel, time, setting.VelScale, time * 2, setting.VisualRotation, time);
                    SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                    SwingHelper.SwingAI(setting.SwingLenght, Player.direction, 0);
                    preAtk.OnStart?.Invoke(this);
                    if (time > 1)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0]++;
                        preAtk.OnChange?.Invoke(this);
                        SoundEngine.PlaySound(playSound, Player.position);
                    }
                    break;
                case 1: // 挥舞
                    SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                    Projectile.extraUpdates = 4;
                    Projectile.ai[1]++;
                    float swingTime = Projectile.ai[1] / (onAtk.SwingTime * (Projectile.extraUpdates + 1));
                    if (swingTime > 1)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0]++;
                        break;
                    }
                    onAtk.OnUse?.Invoke(this);
                    swingTime = onAtk.TimeChange.Invoke(swingTime);

                    SwingHelper.SwingAI(setting.SwingLenght, Player.direction, swingTime * setting.SwingRot * setting.SwingDirectionChange.ToDirectionInt());
                    break;
                case 2: // 超时
                    Projectile.ai[1]++;
                    Projectile.extraUpdates = 0;
                    if (Projectile.ai[1] > postAtk.PostMaxTime)
                    {
                        SkillTimeOut = true;
                        break;
                    }
                    SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                    SwingHelper.SetNotSaveOldVel();
                    postAtk.OnEnd?.Invoke(this);
                    SwingHelper.SwingAI(setting.SwingLenght, Player.direction, setting.SwingRot * setting.SwingDirectionChange.ToDirectionInt() * (1 + Projectile.ai[1] * 0.001f));
                    break;
            }
        }
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            SwingHelper.SetRotVel(0);
            TheUtility.ResetProjHit(Projectile);
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            Projectile.damage = Projectile.originalDamage;
            SwingHelper.SetRotVel(0);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => SwingHelper.GetColliding(targetHitbox);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CanMoveScreen)
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 3, 2, 2));
            onAtk.OnHit?.Invoke(target, hit, damageDone);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            onAtk.ModifyHit?.Invoke(target, ref modifiers);
            modifiers.SourceDamage += setting.ActionDmg - 1;
        }
        public override bool? CanDamage()
        {
            return (int)Projectile.ai[0] == 1;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[0] >= 2 && Projectile.ai[1] > postAtk.PostAtkTime;
        }
        public override bool ActivationCondition()
        {
            return setting.ChangeCondition.Invoke();
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor) => setting.preDraw?.Invoke(sb, lightColor) == true;
    }
}
