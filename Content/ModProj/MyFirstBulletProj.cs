using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace MyMod.Content.ModProj
{
    public class MyFirstBulletProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Bullet}"; // 设置贴图
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true; // 友好
            Projectile.penetrate = -1; // 无穷穿透
            Projectile.tileCollide = true; // 不与地面碰撞
            Projectile.timeLeft = 100; // 持续时间
            #region 本地免疫(独立无敌帧)
            // 本地免疫它会被NPC的无敌帧骗伤,但是它自己不会对NPC造成无敌帧
            Projectile.usesLocalNPCImmunity = true; // 本地免疫
            Projectile.localNPCHitCooldown = -1; // 本地免疫冷却时间
            // 0 为每一帧都有伤害
            // 1 为每间隔1帧时间有一次伤害
            // -1 只能造成一次伤害，但可以重置
            // -2 只能造成一次伤害? // 待验证
            #endregion
        }
        public override void AI()
        {
            if(Projectile.velocity.Length() > 1)
                Projectile.velocity *= 0.9f; // 减速
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // 朝向与速度相同
        }
        //public override void ModifyDamageHitbox(ref Rectangle hitbox) // 修改伤害范围，也就是碰撞箱
        //{
        //    base.ModifyDamageHitbox(ref hitbox);
        //}
        public override bool PreDraw(ref Color lightColor) // 重写绘制
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
