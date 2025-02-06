using MyMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace MyMod.Content.ModProj
{
    public class FristSpurtWeaponProj : ModProjectile
    {
        public SpurtHelper spurtHelper;
        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.Size = new(42);
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ownerHitCheck = true;
        }
        public override bool ShouldUpdatePosition() => false;
        #region 绘制缓存系统
        public DrawCecheSystem cecheSystem = new();
        public class MySecondDrawCeche : DrawCecheSystem.Ceche
        {
            public SpurtHelper SpurtHelper;
            public int Time;

            public MySecondDrawCeche(SpurtHelper swingHelper)
            {
                SpurtHelper = swingHelper;
                Time = 5;
            }
            public override void UpdateCeche()
            {
                //#region 保存旧速度
                //for (int i = oldVels.Length - 1; i > 0; i--)
                //{
                //    oldVels[i] = oldVels[i - 1];
                //}
                //#endregion
                
                if (Time-- < 0) // 时间到,移除
                    Remove = true;
            }
            public override void DrawCeche()
            {
                SpurtHelper.Draw(Main.spriteBatch);
            }
        }
        public override void PostAI()
        {
            for (int i = 0; i < cecheSystem.CecheList.Count; i++)
            {
                cecheSystem.CecheList[i].UpdateCeche();
                if (cecheSystem.CecheList[i].Remove)
                {
                    cecheSystem.CecheList.RemoveAt(i);
                    i--;
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            for (int i = 0; i < cecheSystem.CecheList.Count; i++)
            {
                cecheSystem.CecheList[i].DrawCeche();
                if (cecheSystem.CecheList[i].Remove)
                {
                    i--;
                    cecheSystem.CecheList.RemoveAt(i);
                }
            }
        }
        #endregion
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor,Color.LightSkyBlue,0.8f) with { A = 100};
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            Projectile.Center = vector;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Projectile.soundDelay-- <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                Projectile.soundDelay = 6;
            }
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3);
                    Projectile.netUpdate = true;
                    player.itemAnimation = player.itemTime = 2;
                }
                else
                    Projectile.Kill();
            }
            player.itemRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction));
            if (spurtHelper == null)
            {
                spurtHelper = new(Projectile);
                spurtHelper.Change(400, Projectile.velocity,100,Projectile.Size,20,SpurtHelper.MoreSpurtDraw_Proj);
            }
            spurtHelper.Update(Projectile.Center, player.direction,Projectile.velocity);
            if((int)(spurtHelper.Time % ((float)spurtHelper.TimeMax / spurtHelper.DmgCount)) == 0)
            {
                //cecheSystem.CecheList.Add(new MySecondDrawCeche(spurtHelper.Clone() as SpurtHelper));
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false) - Projectile.Size / 2f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => spurtHelper.Colliding(targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            spurtHelper.Draw(Main.spriteBatch);
            return false;
        }
    }
}
