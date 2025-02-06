using rail;
using System;
using Terraria.GameContent;
using static MyMod.Common.SpurtHelper;

namespace MyMod.Common
{
    /// <summary>
    /// 突刺帮助类
    /// </summary>
    public class SpurtHelper : ICloneable
    {
        public delegate void DrawSpurt(SpurtHelper spurt, SpriteBatch sb);
        public static void ASpurtDraw_Proj(SpurtHelper spurt, SpriteBatch sb)
        {
            if (spurt.Owner is not Projectile)
                return;
            Projectile projectile = spurt.Owner as Projectile;
            Vector2 drawPos = spurt.SpurtPos - Main.screenPosition;
            Vector2 drawSize = new Vector2(spurt.Size.X, spurt.Size.Y);
            Main.instance.LoadProjectile(927);
            Main.instance.LoadProjectile(projectile.type);
            #region 绘制突刺
            Texture2D tex2 = TextureAssets.Projectile[927].Value;
            Vector2 scale = new Vector2(spurt.SpurtLenght / tex2.Width, 1f) * projectile.scale;
            sb.Draw(tex2, drawPos, null, projectile.GetAlpha(Color.White), projectile.rotation - MathHelper.PiOver4, new Vector2(tex2.Width / 4f, tex2.Height / 2f), scale, SpriteEffects.None, 0f);
            #endregion
            #region 绘制弹幕
            Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
            Rectangle drawRect = new Rectangle(0,(int)((float)projectile.frame / Main.projFrames[projectile.type] * tex.Height),tex.Width,tex.Height);
            sb.Draw(tex, drawPos,drawRect, Lighting.GetColor((spurt.SpurtPos / 16).ToPoint()), projectile.rotation, drawSize / 2f, projectile.scale, SpriteEffects.None, 0f);
            #endregion
        }
        public static void MoreSpurtDraw_Proj(SpurtHelper spurt, SpriteBatch sb)
        {
            if (spurt.Owner is not Projectile)
                return;
            Projectile projectile = spurt.Owner as Projectile;
            Vector2 drawPos = spurt.SpurtPos - Main.screenPosition;
            Vector2 drawSize = new Vector2(spurt.Size.X, spurt.Size.Y);
            Main.instance.LoadProjectile(927);
            Main.instance.LoadProjectile(projectile.type);
            #region 绘制突刺            
            for (int i = Main.rand.Next(2, 4); i > 0; i--)
            {
                Texture2D tex2 = TextureAssets.Projectile[927].Value;
                Vector2 scale = new Vector2(spurt.SpurtLenght / tex2.Width, 1f) * projectile.scale * Main.rand.NextFloat(0.2f, 1f);
                float rotation = projectile.rotation - MathHelper.PiOver4 + Main.rand.NextFloatDirection() * 0.2f;
                Vector2 position = drawPos + spurt.SpurtVel * 20 + Main.rand.NextVector2Circular(40, 40);
                rotation -= MathHelper.WrapAngle(rotation - (drawPos + spurt.SpurtVel * spurt.SpurtLenght - position).ToRotation()) * 0.8f;
                sb.Draw(tex2, position, null, projectile.GetAlpha(Color.White), rotation, new Vector2(tex2.Width / 4f, tex2.Height / 2f), scale, SpriteEffects.None, 0f);
            }
            #endregion
            #region 绘制弹幕

            Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
            Rectangle drawRect = new Rectangle(0, (int)((float)projectile.frame / Main.projFrames[projectile.type] * tex.Height), tex.Width, tex.Height);
            sb.Draw(tex, drawPos, drawRect, Lighting.GetColor((spurt.SpurtPos / 16).ToPoint()), projectile.rotation, drawSize / 2f, projectile.scale, SpriteEffects.None, 0f);

            #endregion
        }
        public SpurtHelper(object owner)
        {
            Owner = owner;
        }
        /// <summary>
        /// 突刺长度
        /// </summary>
        public float SpurtLenght;
        /// <summary>
        /// 绘制中心
        /// </summary>
        public Vector2 SpurtPos;
        public Vector2 SpurtVel;
        public Vector2 Size;
        public int SpurtDir;
        public int Time;
        public int TimeMax = 20;
        public DrawSpurt drawSpurt;
        /// <summary>
        /// 动画次数与伤害次数
        /// </summary>
        public int DmgCount = 1;
        /// <summary>
        /// 产生者
        /// </summary>
        public object Owner;
        /// <summary>
        /// 改变东西用的函数,建议只调用一次,作为设置
        /// </summary>
        /// <param name="spurtLenght">突刺长度</param>
        /// <param name="vel">速度朝向</param>
        /// <param name="TimeMax">最大持续时间</param>
        /// <param name="size">大小</param>
        /// <param name="dmgCount">动画次数与伤害次数</param>
        public virtual void Change(float spurtLenght, Vector2 vel, int TimeMax, Vector2 size, int dmgCount ,DrawSpurt drawSpurt)
        {
            SpurtVel = vel.SafeNormalize(default);
            SpurtLenght = spurtLenght;
            this.TimeMax = TimeMax;
            DmgCount = dmgCount;
            Size = size;
            this.drawSpurt = drawSpurt;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="dir">朝向</param>
        public virtual void Update(Vector2 pos, int dir,Vector2 vel)
        {
            SpurtPos = pos;
            SpurtVel = vel.SafeNormalize(default);
            SpurtPos += Time % ((float)TimeMax / DmgCount) * SpurtVel * Size.Length() * 0.2f;
            SpurtDir = dir;
            if (Time < TimeMax)
                Time++;
            else
                Time = 0;
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            drawSpurt?.Invoke(this,sb);
        }
        public virtual bool Colliding(Rectangle targetRect)
        {
            float sizeLenght = Size.Length();
            for (float i = 0; i <= 1f; i += sizeLenght / SpurtLenght)
            {
                Rectangle rect = new((int)(SpurtPos.X - Size.X / 2), (int)(SpurtPos.Y - Size.Y / 2), (int)Size.X, (int)Size.Y);
                rect.Offset((int)(SpurtVel.X * SpurtLenght * 0.5f * i), (int)(SpurtVel.Y * SpurtLenght * 0.5f * i));
                if (targetRect.Intersects(rect))
                {
                    return true;
                }
            }
            return false;
        }
        public virtual bool CanDamage() => Time % (TimeMax / DmgCount) == 0;
        public virtual object Clone() => new SpurtHelper(Owner)
        {
            SpurtDir = SpurtDir,
            SpurtLenght = SpurtLenght,
            Time = Time,
            TimeMax = TimeMax,
            SpurtPos = SpurtPos,
            SpurtVel = SpurtVel,
            DmgCount = DmgCount,
            drawSpurt = drawSpurt,
            Size = Size,
            Owner = Owner
        };
    }
}
