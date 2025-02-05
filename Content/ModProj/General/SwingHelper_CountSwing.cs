using MyMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace MyMod.Content.ModProj.General
{
    /// <summary>
    /// 多次挥舞
    /// </summary>
    public class SwingHelper_CountSwing : SwingHelper_GeneralSwing
    {
        public SwingHelper_CountSwing(ModProjectile modProjectile, Setting setting, PreAtk preAtk, PostAtk postAtk, OnAtk onAtk, SwingHelper swingHelper, Player player,int SwingCount) : base(modProjectile, setting, preAtk, postAtk, onAtk, swingHelper, player)
        {
            this.SwingCount = SwingCount;
        }
        public int SwingCount = 1;
        public override void AI()
        {
            Projectile.spriteDirection = Player.direction * setting.SwingDirectionChange.ToDirectionInt();
            Player.heldProj = Projectile.whoAmI;
            Projectile.hide = true;

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
                        Projectile.ai[2] = SwingCount; // 剩余挥击次数
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
                        if (--Projectile.ai[2] <= 0) // 剩余挥击次数为0
                            Projectile.ai[0]++; // 切换状态
                        else
                        {
                            SwingHelper.SetNotSaveOldVel();
                            TheUtility.ResetProjHit(Projectile);
                        }
                        onAtk.OnChange?.Invoke(this);
                        break;
                    }
                    onAtk.OnUse?.Invoke(this);
                    swingTime = onAtk.TimeChange.Invoke(swingTime);

                    SwingHelper.SwingAI(setting.SwingLenght, Player.direction, swingTime * setting.SwingRot * setting.SwingDirectionChange.ToDirectionInt());
                    break;
                case 2: // 超时
                    base.AI();
                    break;
            }
        }
    }
}
