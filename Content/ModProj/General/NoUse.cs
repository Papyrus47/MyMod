using MyMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMod.Content.ModProj.General
{
    public class NoUse : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        public float Length = 30;
        public NoUse(Player player, SwingHelper swingHelper,ModProjectile proj) : base(proj)
        {
            this.player = player;
            this.swingHelper = swingHelper;
        }
        public override void AI()
        {
            //Projectile.localAI[0] += player.velocity.X / 10;
            //Projectile.localAI[0] %= 6.28f;
            //float rotation = Projectile.localAI[0] % (MathHelper.PiOver4 * 2) - MathHelper.PiOver4;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.velocity.X / 10f); // 设置玩家的前臂为合成状态，并设置其伸展程度为Full
            player.itemRotation = player.compositeFrontArm.rotation; // 设置玩家的手臂角度为玩家的前臂的旋转角度

            player.heldProj = Projectile.whoAmI;
            swingHelper.Change(Vector2.UnitY, Vector2.One, 0); // 起始位置,缩放
            swingHelper.ProjFixedPos(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, player.velocity.X / 10f),-Length * 0.02f);
            swingHelper.SetSwingActive(); // 激活挥舞
            Projectile.spriteDirection = player.direction; // 弹幕贴图朝向与玩家一致
            swingHelper.SwingAI(Length, player.direction, (player.velocity.X / 10f) * player.direction - MathHelper.PiOver2);
        }
        public override bool ActivationCondition() => true; // 无条件激活
        public override bool SwitchCondition() => true; // 无条件切换
        public override bool? CanDamage() => false; // 无法造成伤害
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, null, null); // 用这个只绘制弹幕本体
            // 禁用绘制
            return false;
        }
    }
}
