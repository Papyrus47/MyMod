using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config;

namespace MyMod.Common
{
    /// <summary>
    /// 挥舞用的类
    /// <para>注意 rotation 依然发挥作用</para>
    /// </summary>
    public class SwingHelper : ICloneable
    {
        /// <summary>
        /// 请设置为Projectile
        /// </summary>
        public object SpawnEntity;
        /// <summary>
        /// 起点向量
        /// </summary>
        public Vector2 StartVel;
        public Vector2 VelScale;
        public Vector2[] oldVels;
        public float VisualRotation;
        public int[] oldFrames;
        /// <summary>
        /// 使用Shader的索引
        /// </summary>
        public byte UseShaderPass = 0;
        /// <summary>
        /// 重复绘制拖尾的次数
        /// </summary>
        public int DrawTrailCount = 1;
        /// <summary>
        /// 挥舞的启用
        /// </summary>
        public bool _acitveSwing;
        public bool _changeLerpInvoke;
        public bool _canDrawTrailing;
        public bool _drawCorrections;
        public bool _oldVelsSave;
        public float _halfSizeLength;
        public float _velLerp;
        public float _changeHeldLength;
        public float _velRotBy;
        public Asset<Texture2D> SwingItemTex;
        public Projectile projectile => SpawnEntity as Projectile;
        public virtual Vector2 velocity { get => projectile.velocity; set => projectile.velocity = value; }
        public virtual Vector2 Center { get => projectile.Center; set => projectile.Center = value; }
        public virtual float rotation { get => projectile.rotation; set => projectile.rotation = value; }
        public virtual int spriteDirection { get => projectile.spriteDirection; set => projectile.spriteDirection = value; }
        public virtual int frame { get => projectile.frame; set => projectile.frame = value; }
        public virtual int frameMax { get => Main.projFrames[projectile.type]; set => Main.projFrames[projectile.type] = value; }
        public virtual int width { get => projectile.width; set => projectile.width = value; }
        public virtual Vector2 Size { get => projectile.Size; set => projectile.Size = value; }
        public SwingHelper(object spawnEntity, int oldVelLength, Asset<Texture2D> swingItemTex = null)
        {
            SpawnEntity = spawnEntity;
            _halfSizeLength = 0;
            oldVels = new Vector2[oldVelLength];
            _oldVelsSave = true;
            SwingItemTex = swingItemTex;
        }
        public virtual void SendData(BinaryWriter writer)
        {
            //for (int i = 0; i < oldVels.Length; i++)
            //{
            //    writer.WriteVector2(oldVels[i]);
            //}
            writer.Write(_velRotBy);
        }
        public virtual void RendData(BinaryReader reader)
        {
            //for (int i = 0; i < oldVels.Length; i++)
            //{
            //    oldVels[i] = reader.ReadVector2();
            //}
            _velRotBy = reader.ReadSingle();
        }
        public virtual void SetRotVel(float rotVel)
        {
            _velRotBy = rotVel;
        }
        public virtual void Change(Vector2 startVel, Vector2 velScale, float visualRotation = 0f)
        {
            StartVel = startVel;
            StartVel.Normalize();
            VisualRotation = visualRotation;
            VelScale = velScale;
            _halfSizeLength = Size.Length() * 0.5f;
        }
        public virtual void Change_Lerp(Vector2 startVel, float velLerp, Vector2 velScale, float scaleAmount, float visualRotation = 0f, float visualRotAmount = 0.1f)
        {
            startVel.Normalize();
            StartVel = startVel;
            _velLerp = velLerp;
            VelScale = Vector2.Lerp(VelScale, velScale, scaleAmount);
            _halfSizeLength = MathHelper.Lerp(_halfSizeLength, Size.Length() * 0.5f, scaleAmount);
            VisualRotation = MathHelper.Lerp(VisualRotation, visualRotation, visualRotAmount);
            _changeLerpInvoke = true;
        }
        public virtual void VelRotBy(float rot) => _velRotBy = rot;
        /// <summary>
        /// 挥舞函数
        /// </summary>
        /// <param name="velLength">弹幕长度</param>
        /// <param name="dir">朝向</param>
        /// <param name="Rot">旋转弧度</param>
        public virtual void SwingAI(float velLength, int dir, float Rot)
        {
            if (_acitveSwing)
            {
                _canDrawTrailing = true;
                Vector2 start = StartVel;
                start.X *= dir;

                Vector2 vel = start;

                if (_changeLerpInvoke)
                {
                    //velocity = Vector2.Lerp(velocity, vel * velLength, _velLerp);
                    vel *= VelScale;
                    if (velocity == default) velocity = Vector2.One;
                    velocity = velocity.
                        RotatedBy(MathHelper.WrapAngle(vel.ToRotation() - velocity.ToRotation()) * _velLerp).
                        SafeNormalize(default) * MathHelper.Lerp(velocity.Length(), velLength, _velLerp);
                }
                else
                {
                    vel = vel.RotatedBy(Rot * dir);
                    vel *= VelScale;
                    vel = vel.RotatedBy(_velRotBy * dir);
                    velocity = vel * velLength;
                }
            }
            oldFrames ??= new int[oldVels.Length];

            #region 保存oldXXX
            for (int i = oldVels.Length - 1; i >= 0; i--)
            {
                if (_acitveSwing && !_changeLerpInvoke)
                {
                    if (i == 0)
                    {
                        oldVels[0] = _oldVelsSave ? velocity : default;
                        oldFrames[0] = frame;
                    }
                    else
                    {
                        oldVels[i] = oldVels[i - 1];
                        oldFrames[i] = oldFrames[i - 1];
                    }
                }
                else
                {
                    oldVels[i] = default;
                    oldFrames[i] = 0;
                }
            }
            #endregion

            if (projectile != null) projectile.timeLeft = 2;
            _oldVelsSave = true;
            _acitveSwing = _changeLerpInvoke = false;

            //if (_drawCorrections)
            //{
            //    Center -= velocity.SafeNormalize(default) * _changeHeldLength;
            //}
        }
        /// <summary>
        /// 将弹幕锁定位置在玩家身上   <para>当<paramref name="isUseSwing"/>为ture的时候,
        /// 则会根据<code> MathF.Atan2(velocity.Y * player.direction, velocity.X * player.direction)</code>
        /// 决定玩家手臂方向,同时设置使用时间和挥舞时间为2</para>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="length">根据弹幕速度,决定最终位置  就是位置加上速度的单位向量乘以的这个<paramref name="length"/></param>
        /// <param name="isUseSwing">如果开始挥舞,请设置这个为true,当这个为true的时候,调用挥舞函数才能使挥舞继续</param>
        /// <param name="drawCorrections">绘制修正</param>
        public virtual void ProjFixedPlayerCenter(Player player, float length = 0f, bool isUseSwing = false, bool drawCorrections = false)
        {
            Center = player.RotatedRelativePoint(player.MountedCenter);
            _drawCorrections = drawCorrections;
            if (!_drawCorrections) Center += velocity.SafeNormalize(default) * length;
            _changeHeldLength = length;
            if (isUseSwing)
            {
                SetSwingActive();
                player.itemAnimation = player.itemTime = 2;
                player.itemRotation = MathF.Atan2(velocity.Y * player.direction, velocity.X * player.direction);
            }
        }
        /// <summary>
        /// 将弹幕锁定位置在对应位置上
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="length">根据弹幕速度,决定最终位置  就是位置加上速度的单位向量乘以的这个<paramref name="length"/></param>
        public virtual void ProjFixedPos(Vector2 pos, float length = 0f, bool drawCorrections = false)
        {
            Center = pos;
            _drawCorrections = drawCorrections;
            if (!_drawCorrections) Center += velocity.SafeNormalize(default) * length;
            _changeHeldLength = length;
        }
        public virtual void SetSwingActive() => _acitveSwing = true;
        /// <summary>
        /// 使用这个函数会使拖尾不保存现有的速度
        /// </summary>
        public virtual void SetNotSaveOldVel() => _oldVelsSave = false;
        #region 绘制 
        /// <summary>
        /// 绘制剑与拖尾
        /// </summary>
        /// <param name="drawColor"></param>
        /// <param name="tex"></param>
        /// <param name="colorFunc"></param>
        /// <param name="effect"></param>
        public virtual void Swing_Draw_ItemAndTrailling(Color drawColor, Texture2D tex, Func<float, Color> colorFunc, Effect effect = null, Func<float, float> SetZ = null)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            DrawTrailing(tex, colorFunc, effect, SetZ);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            DrawSwingItem(drawColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
        /// <summary>
        /// 绘制剑与残影
        /// </summary>
        /// <param name="drawColor"></param>
        /// <param name="colorFunc"></param>
        /// <param name="drawCount"></param>
        public virtual void Swing_Draw_ItemAndAfterimage(Color drawColor, Func<float, Color> colorFunc, int drawCount = -1)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

            Swing_Draw_Afterimage(colorFunc, drawCount);
            DrawSwingItem(drawColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
        /// <summary>
        /// 绘制拖尾 残影 剑
        /// </summary>
        /// <param name="drawColor"></param>
        /// <param name="tex"></param>
        /// <param name="AfterimageColorFunc"></param>
        /// <param name="TrailingColorFunc"></param>
        /// <param name="drawAfterimageCount"></param>
        /// <param name="effect"></param>
        public virtual void Swing_Draw_All(Color drawColor, Texture2D tex, Func<float, Color> AfterimageColorFunc, Func<float, Color> TrailingColorFunc, int drawAfterimageCount = -1, Effect effect = null, Func<float, float> SetZ = null)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            DrawTrailing(tex, TrailingColorFunc, effect, SetZ);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Swing_Draw_Afterimage(AfterimageColorFunc, drawAfterimageCount);
            DrawSwingItem(drawColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
        /// <summary>
        /// 绘制残影
        /// </summary>
        /// <param name="drawColorFunc"></param>
        /// <param name="drawCount"></param>
        public virtual void Swing_Draw_Afterimage(Func<float, Color> drawColorFunc, int drawCount = -1)
        {
            if (!_canDrawTrailing) return;

            GraphicsDevice gd = Main.graphics.GraphicsDevice;

            CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
            int velLength = oldVels.Length;
            List<CustomVertexInfo> customVertexInfos = new List<CustomVertexInfo>();
            for (int i = 0; i < velLength; i++)
            {
                if (i > drawCount && drawCount != -1) break;

                float factor = (oldFrames[i] + 1f) / frameMax;
                Vector2 velocity = GetOldVel(i);
                Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(VisualRotation * spriteDirection).SafeNormalize(default)
                    * _halfSizeLength * spriteDirection;
                Vector2 center = GetDrawCenter(i);
                if (_drawCorrections)
                {
                    center = Center + (center - Center);
                }
                Vector2 halfVelPos = center + velocity * 0.5f;
                Vector2[] pos = new Vector2[4]
                {
                    center - Main.screenPosition,
                    halfVelPos - halfLength - Main.screenPosition,
                    center + velocity - Main.screenPosition,
                    halfVelPos + halfLength  - Main.screenPosition
                };
                Color drawColor = drawColorFunc.Invoke((float)i / velLength);
                customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0f, factor, 0)); // 柄
                customVertices[1] = new(pos[1], drawColor, new Vector3(0f, factor - 1f, 0)); // 左上角
                customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1f, factor - 1f, 0)); // 头
                customVertices[4] = new(pos[3], drawColor, new Vector3(1f, factor, 0)); // 右下角

                customVertexInfos.AddRange(customVertices);
            }
            gd.Textures[0] = SwingItemTex.Value;
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertexInfos.ToArray(), 0, customVertexInfos.Count / 3);
        }
        public virtual void DrawSwingItem(Color drawColor)
        {
            //if (projectile.localAI[0]++ > 2)
            //    Main.NewText(true);
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (projectile != null)
            {
                SwingItemTex ??= TextureAssets.Projectile[projectile.type];
            }
            //var origin = gd.RasterizerState;
            //RasterizerState rasterizerState = new()
            //{
            //    CullMode = CullMode.None,
            //    FillMode = FillMode.WireFrame
            //};
            //gd.RasterizerState = rasterizerState;

            Vector2 velocity = GetOldVel(-1, true);
            Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(VisualRotation * spriteDirection).SafeNormalize(default)
                * _halfSizeLength * spriteDirection;

            Vector2 center = GetDrawCenter();
            if (_drawCorrections)
            {
                center = Center + (center - Center);
            }
            Vector2 halfVelPos = center + velocity * 0.5f;
            Vector2[] pos = new Vector2[4]
            {
                 center - Main.screenPosition,
                 halfVelPos - halfLength - Main.screenPosition,
                 center + velocity - Main.screenPosition,
                 halfVelPos + halfLength  - Main.screenPosition
            };

            float factor = (frame + 1f) / frameMax;
            CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
            customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0, factor, 0)); // 柄
            customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
            customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1, factor - 1f, 0)); // 头
            customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor, 0)); // 右下角

            gd.Textures[0] = SwingItemTex.Value;
            //gd.Textures[0] = TextureAssets.MagicPixel.Value;
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
            //gd.RasterizerState = origin;
        }

        /// <summary>
        /// 绘制拖尾
        /// </summary>
        /// <param name="tex">拖尾贴图</param>
        /// <param name="colorFunc">拖尾颜色</param>
        /// <param name="effect">调用的shader 默认不选择调用,如果调用,则需要让shader处理顶点信息转换到屏幕上</param>
        public virtual void Swing_TrailingDraw(Texture2D tex, Func<float, Color> colorFunc, Effect effect = null)
        {
            if (tex == null || colorFunc == null || !_canDrawTrailing)
            {
                return;
            }
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            DrawTrailing(tex, colorFunc, effect);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public virtual void DrawTrailing(Texture2D tex, Func<float, Color> colorFunc, Effect effect, Func<float, float> SetZ = null)
        {
            List<CustomVertexInfo> customVertices = new();
            if (colorFunc == null)
                return;
            int length = oldVels.Length;

            for (int j = 0; j < DrawTrailCount; j++) // 重复绘制拖尾的次数
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    Vector2 vel = GetOldVel(i, true);
                    if (vel == default)
                    {
                        break;
                    }

                    //float factor2 = EaseFunction.EaseOutQuint(i, 0f, length - 1, 0, 0.2f);
                    float factor = (float)i / length;
                    Color drawColor = colorFunc.Invoke(factor); // 获取绘制颜色
                                                                //Main.NewText(drawColor);
                    Vector2 pos = GetDrawCenter(i);
                    if (_drawCorrections)
                    {
                        pos = Center + (pos - Center);
                    }
                    if (effect == null || UseShaderPass == 1)
                    {
                        pos -= Main.screenPosition;
                    }
                    float z = 0;
                    if (SetZ != null) z = SetZ(factor);


                    customVertices.Add(new(pos, drawColor, new Vector3(factor, 0, z)));
                    customVertices.Add(new(pos + vel, drawColor, new Vector3(factor, 1, z)));

                }
                if (customVertices.Count > 4 * DrawTrailCount)
                {
                    List<CustomVertexInfo> vertices = TheUtility.GenerateTriangle(customVertices);
                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    //var origin = gd.RasterizerState;
                    //RasterizerState rasterizerState = new()
                    //{
                    //    CullMode = CullMode.None,
                    //    FillMode = FillMode.WireFrame
                    //};
                    //gd.RasterizerState = rasterizerState;
                    //gd.Textures[0] = tex;

                    gd.Textures[0] = tex;
                    //gd.Textures[0] = TextureAssets.MagicPixel.Value;
                    effect?.CurrentTechnique.Passes[UseShaderPass].Apply();
                    gd.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
                    customVertices.Clear();
                    //gd.RasterizerState = origin;
                }
            }
        }

        #endregion
        public virtual Vector2 GetDrawCenter(int index = 0)
        {
            Vector2 pos = Center;
            if (_drawCorrections)
            {
                pos += oldVels[index].SafeNormalize(default) * _changeHeldLength;
            }
            return pos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="notFilp">将旋转角度反方向修正</param>
        /// <returns></returns>
        public virtual Vector2 GetOldVel(int i, bool notFilp = true)
        {
            if (i < 0)
            {
                return velocity.RotatedBy(notFilp.ToDirectionInt() * rotation * spriteDirection);
            }
            return oldVels[i].RotatedBy(notFilp.ToDirectionInt() * rotation * spriteDirection);
        }

        public virtual bool GetColliding(Rectangle targetHitBox)
        {
            float r = 0;
            if (_drawCorrections)
            {
                return Collision.CheckAABBvLineCollision(targetHitBox.TopLeft(), targetHitBox.Size(), Center, Center + velocity.RotatedBy(rotation * spriteDirection),
                width / 2, ref r);
            }
            return Collision.CheckAABBvLineCollision(targetHitBox.TopLeft(), targetHitBox.Size(), Center, Center + velocity.RotatedBy(rotation * spriteDirection),
                width / 2, ref r);
        }

        public object Clone()
        {
            SwingHelper sh = new(SpawnEntity, oldVels.Length, SwingItemTex)
            {
                oldVels = oldVels.Clone() as Vector2[],
                velocity = velocity,
                rotation = rotation,
                spriteDirection = spriteDirection,
                Size = Size,
                StartVel = StartVel,
                oldFrames = oldFrames.Clone() as int[],
                UseShaderPass = UseShaderPass,
                _acitveSwing = _acitveSwing,
                _canDrawTrailing = _canDrawTrailing,
                _changeHeldLength = _changeHeldLength,
                _halfSizeLength = _halfSizeLength,
                _oldVelsSave = _oldVelsSave,
                _velLerp = _velLerp,
                _velRotBy = _velRotBy,
                _drawCorrections = _drawCorrections,
                VelScale = VelScale,
                VisualRotation = VisualRotation,
                DrawTrailCount = DrawTrailCount
            };
            return sh;
        }
    }
}
