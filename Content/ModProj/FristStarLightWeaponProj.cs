using System;
using MyMod.Content.Items;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace MyMod.Content.ModProj
{
    public class FristStarLightWeaponProj : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.None}";
        public SoundStyle Sound => SoundID.Item1 with
        {
            MaxInstances = 10 //最大实例数
        }; // 使用时的声音
        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.Size = new(42);
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ownerHitCheck = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            float num3 = 0f;
            int num2 = 9;
            int num = 0;
            Projectile.scale = 0.5f;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 8f)
                Projectile.ai[0] = 0f;
            num3 = Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.05f;
            Projectile.soundDelay--;
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                Projectile.soundDelay = 6;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float num46 = 1f;
                    if (player.inventory[player.selectedItem].shoot == Projectile.type)
                        num46 = player.inventory[player.selectedItem].shootSpeed;

                    Vector2 vec3 = Main.MouseWorld - vector;
                    vec3.Normalize();
                    if (vec3.HasNaNs())
                        vec3 = Vector2.UnitX * player.direction;

                    vec3 *= num46;
                    if (vec3.X != Projectile.velocity.X || vec3.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    Projectile.velocity = vec3;
                }
                else
                {
                    Projectile.Kill();
                }
            }

            DelegateMethods.v3_1 = new Vector3(0.5f, 0.5f, 0.5f);
            Utils.PlotTileLine(Projectile.Center - Projectile.velocity, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 80f, 16f, DelegateMethods.CastLightOpen);

            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(num2);
            player.itemRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction) + num3);
            player.itemAnimation = num2 - (int)Projectile.ai[0];

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (float num8 = 0f; num8 <= 1f; num8 += 0.05f)
            {
                float num9 = Utils.Remap(num8, 0f, 1f, 1f, 5f);
                Rectangle rectangle = projHitbox;
                Vector2 vector5 = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.width * num9 * Projectile.scale;
                rectangle.Offset((int)vector5.X, (int)vector5.Y);
                if (rectangle.Intersects(targetHitbox))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        #region 绘制部分造着抄
        public override bool PreDraw(ref Color lightColor)
        {
            float num = Projectile.scale;
            if (num == 0f)
                num = 1f;

            int num2 = (int)Math.Ceiling(3f * num);
            Main.instance.LoadProjectile(Projectile.type);
            Main.instance.LoadItem(ModContent.ItemType<FristStarLightWeapon>());
            int num3 = 2;
            Vector2 vector = Projectile.Center - Projectile.rotation.ToRotationVector2() * num3;
            for (int i = 0; i < 1; i++)
            {
                float num4 = Main.rand.NextFloat();
                float num5 = Utils.GetLerpValue(0f, 0.3f, num4, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num4, clamped: true);
                Color color = Projectile.GetAlpha(Lighting.GetColor(Projectile.Center.ToTileCoordinates())) * num5;
                Texture2D value = TextureAssets.Item[ModContent.ItemType<FristStarLightWeapon>()].Value; // 获取贴图
                Vector2 origin = value.Size() / 2f;
                float num6 = Main.rand.NextFloatDirection();
                float num7 = 8f + MathHelper.Lerp(0f, 20f, num4) + Main.rand.NextFloat() * 6f;
                float num8 = Projectile.rotation + num6 * ((float)Math.PI * 2f) * 0.04f;
                float num9 = num8 + (float)Math.PI / 4f;
                Vector2 position = vector + num8.ToRotationVector2() * num7 + Main.rand.NextVector2Circular(8f, 8f) - Main.screenPosition;
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.rotation < -(float)Math.PI / 2f || Projectile.rotation > (float)Math.PI / 2f)
                {
                    num9 += (float)Math.PI / 2f;
                    spriteEffects |= SpriteEffects.FlipHorizontally;
                }

                Main.spriteBatch.Draw(value, position, null, color, num9, origin, 1f, spriteEffects, 0f);
            }

            for (int j = 0; j < num2; j++)
            {
                float num10 = Main.rand.NextFloat();
                float num11 = Utils.GetLerpValue(0f, 0.3f, num10, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num10, clamped: true);
                float amount = Utils.GetLerpValue(0f, 0.3f, num10, clamped: true) * Utils.GetLerpValue(1f, 0.5f, num10, clamped: true);
                float num12 = MathHelper.Lerp(0.6f, 1f, amount);
                Color weaponColor = Color.OrangeRed; // 铜的颜色
                Texture2D value2 = TextureAssets.Projectile[927].Value;
                Color color2 = weaponColor;
                color2 *= num11 * 0.5f;
                Vector2 origin2 = value2.Size() / 2f;
                Color color3 = Color.White * num11;
                color3.A /= 2;
                Color color4 = color3 * 0.5f;
                float num13 = 1f;
                float num14 = (num - 1f) / 2f;
                float num15 = Main.rand.NextFloat() * 2f * num;
                num15 += num14;
                float num16 = Main.rand.NextFloatDirection();
                Vector2 vector2 = new Vector2(2.8f + num15 * (1f + num14), 1f) * num13 * num12;
                float value3 = 50f * num;
                Vector2 vector3 = Projectile.rotation.ToRotationVector2() * (j >= 1 ? 56 : 0);
                float num17 = 0.03f - j * 0.012f;
                num17 /= num;
                float num18 = 30f + MathHelper.Lerp(0f, value3, num10) + num15 * 16f;
                float num19 = Projectile.rotation + num16 * ((float)Math.PI * 2f) * num17;
                float rotation = num19;
                Vector2 position2 = vector + num19.ToRotationVector2() * num18 + Main.rand.NextVector2Circular(20f, 20f) + vector3 - Main.screenPosition;
                color2 *= num13;
                color4 *= num13;
                SpriteEffects effects = SpriteEffects.None;
                Main.spriteBatch.Draw(value2, position2, null, color2, rotation, origin2, vector2, effects, 0f);
                Main.spriteBatch.Draw(value2, position2, null, color4, rotation, origin2, vector2 * 0.6f, effects, 0f);
            }
            return false;
        }
        #endregion
    }
}
