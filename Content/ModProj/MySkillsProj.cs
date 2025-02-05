using MyMod.Common;
using MyMod.Content.ModProj.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace MyMod.Content.ModProj
{
    public class MySkillsProj : ModProjectile, IBasicSkillProj
    {
        #region 机器复制粘贴环节
        public Item SpawnItem;
        public Player Player;
        public SwingHelper SwingHelper;
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
                        if (!SkillsParis.ContainsKey(name))
                        {
                            name += i.ToString();
                            if (IDParis.ContainsValue(name))
                            {
                                name += (i++).ToString();
                                continue;
                            }
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
        public Dictionary<ProjSkill_Instantiation, string> IDParis { get; set; }
        public Dictionary<string, ProjSkill_Instantiation> SkillsParis { get; set; }

        public override void SetDefaults()
        {
            Projectile.ownerHitCheck = true; // 弹幕检查是否隔墙
            Projectile.penetrate = -1; // 弹幕穿透
            Projectile.aiStyle = -1; // 弹幕AI样式
            Projectile.friendly = true; // 弹幕友好为true允许造成伤害
            Projectile.tileCollide = false; // 弹幕不碰撞墙壁
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地NPC伤害（本地无敌帧）
            Projectile.localNPCHitCooldown = -1; // -1表示只能命中一次
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem);
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingHelper.DrawTrailCount = 4; // 绘制拖尾的次数
                IDParis = new();
                SkillsParis = new();
                //SwingLength = Projectile.Size.Length();
                //Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
        }
        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2; // 弹幕不消失
            CurrentSkill.AI();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            SwingHelper.SendData(writer);
            (this as IBasicSkillProj).SendData(writer);
        }
        
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingHelper.RendData(reader);
            (this as IBasicSkillProj).ReceiveData(reader);
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem, Player, target, ref modifiers); // 调用Mod物品的ModifyHitNPC
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone); // 技能命中效果
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone); // Mod物品命中
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone); // 原版物品命中
        }
        #endregion
        #region 绘制缓存系统
        public DrawCecheSystem cecheSystem = new();
        public class MyFristDrawCeche : DrawCecheSystem.Ceche
        {
            public SwingHelper SwingHelper;
            public int Time;

            public MyFristDrawCeche(SwingHelper swingHelper)
            {
                SwingHelper = swingHelper;
                Time = 60;
            }
            public override void UpdateCeche()
            {
                Vector2[] oldVels = SwingHelper.oldVels; // 获得旧速度
                //#region 保存旧速度
                //for (int i = oldVels.Length - 1; i > 0; i--)
                //{
                //    oldVels[i] = oldVels[i - 1];
                //}
                //#endregion
                if(Time-- < 0) // 时间到,移除
                    Remove = true;
            }
            public override void DrawCeche()
            {
                SwingHelper.Swing_Draw_ItemAndTrailling(Color.Transparent, TextureAssets.Extra[201].Value, (factor) => Color.Lerp(Color.LightPink, Color.Purple * (Time / 60f), factor) with { A = 0 } * (Time / 60f));
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
        public override void OnKill(int timeLeft)
        {
            Player.fullRotation = 0;
            Player.legRotation = 0;
        }
        public void Init()
        {
            OldSkills = new();

            NoUse noUse = new(Player, SwingHelper, this)
            {
                Length = Projectile.Size.Length()
            }; // 玩家拿在手上不使用的时候

            SwingHelper_GeneralSwing.Setting.PreDraw drawProj = (sb, drawColor) =>
                            {
                                SwingHelper.Swing_Draw_ItemAndTrailling(drawColor, TextureAssets.Extra[201].Value, (factor) => Color.Lerp(Color.LightPink, Color.Purple, factor) with { A = 0 } * factor * 3);
                                return false;
                            }; // 绘制弹幕
            SwingHelper_GeneralSwing.Setting.PreDraw drawProj2 = (sb, drawColor) =>
            {
                SwingHelper.Swing_Draw_ItemAndTrailling(drawColor, TextureAssets.Extra[201].Value, (factor) => Color.Lerp(Color.LightPink, Color.Purple, factor) with { A = 0});
                return false;
            };
            Func<float, float> swingChange = (time) => MathHelper.SmoothStep(0, 1f, time); // 缓动函数
            Action<NPC, NPC.HitInfo, int> onHitEffect = (target, hit, damage) =>
            {
                for (int i = -30; i <= 30; i++)
                {
                    var dust = Dust.NewDustPerfect(target.Center, DustID.CrystalPulse, Projectile.velocity.RotatedBy(MathHelper.PiOver2 * Player.direction).SafeNormalize(default) * i * 0.5f, 100, Color.Purple,0.8f);
                    dust.noGravity = true;

                    //dust = Dust.NewDustPerfect(target.Center, DustID.CrystalPulse, Projectile.velocity.SafeNormalize(default) * i * 0.5f, 100, Color.Purple, 0.8f);
                    //dust.noGravity = true;
                }
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center,Projectile.velocity.SafeNormalize(default).RotatedByRandom(0.7),3f,2f,2));
            }; // 击中效果

            SwingHelper_GeneralSwing SwingUp = new(this, // 上斩
            setting: new() // 设置
            {
                SwingLenght = Projectile.Size.Length(),// 挥舞长度
                ChangeCondition = () => Player.controlUseItem,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2, // 挥舞角度
                preDraw = drawProj,
                SwingDirectionChange = false, // 挥舞方向变化
                StartVel = Vector2.UnitY.RotatedBy(-0.4f),// 起始速度朝向
                VelScale = new Vector2(1, 1), // 速度缩放
                VisualRotation = 0, // 视觉朝向
            },
            preAtk: new() // 攻击前
            {
                PreTime = 3, // 前摇时间
                OnChange = (_) =>
                {
                    if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI
                        return;
                    Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                    SwingHelper.SetRotVel(Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()); // 朝向
                }
            },
            postAtk: new() // 攻击后
            {
                PostMaxTime = 30, // 后摇最大时间
                PostAtkTime = 3, // 后摇切换时间
            }, onAtk: new() // 攻击时
            {
                SwingTime = 10, // 挥舞时间
                TimeChange = swingChange, // 时间变化函数
                OnHit = (target, hit, damage) =>
                {
                    onHitEffect.Invoke(target, hit, damage);
                    if (target.knockBackResist != 0)
                        target.velocity.Y = -5f; // 击飞
                },
                OnChange = (_) =>
                {
                    Player.fullRotation = 0;
                    Player.legRotation = 0;

                },
                OnUse = (_) =>
                {
                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation, Player.direction * 0.4f, 0.1f);
                    Player.legRotation = -Player.fullRotation;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                }

            }, SwingHelper, Player);

            SwingHelper_GeneralSwing SwingAcross = new(this, // 横斩
            setting: new() // 设置
            {
                SwingLenght = Projectile.Size.Length(),// 挥舞长度
                ChangeCondition = () => Player.controlUseItem,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2, // 挥舞角度
                preDraw = drawProj,
                SwingDirectionChange = false, // 挥舞方向变化
                StartVel = Vector2.UnitY.RotatedBy(0.4f),// 起始速度朝向
                VelScale = new Vector2(1, 0.3f), // 速度缩放
                VisualRotation = 0.7f, // 视觉朝向
            },
            preAtk: new() // 攻击前
            {
                PreTime = 3, // 前摇时间
                OnChange = (_) =>
                {
                    if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI
                        return;
                    Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                    SwingHelper.SetRotVel(Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()); // 朝向
                }
            },
            postAtk: new() // 攻击后
            {
                PostMaxTime = 15, // 后摇最大时间
                PostAtkTime = 3, // 后摇切换时间
            }, onAtk: new() // 攻击时
            {
                SwingTime = 10, // 挥舞时间
                TimeChange = swingChange, // 时间变化函数
                OnHit = onHitEffect,
                OnChange = (_) =>
                {
                    Player.fullRotation = 0;
                    Player.legRotation = 0;

                },
                OnUse = (_) =>
                {
                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation, Player.direction * 0.4f, 0.1f);
                    Player.legRotation = -Player.fullRotation;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                }
            }, SwingHelper, Player);

            SwingHelper_GeneralSwing SwingDown = new(this, // 下挥
            setting: new() // 设置
            {
                SwingLenght = Projectile.Size.Length(),// 挥舞长度
                ChangeCondition = () => Player.controlUseItem,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2, // 挥舞角度
                preDraw = drawProj,
                SwingDirectionChange = true, // 挥舞方向变化
                StartVel = -Vector2.UnitY.RotatedBy(-0.4f),// 起始速度朝向
                VelScale = new Vector2(1, 1f), // 速度缩放
                VisualRotation = 0f, // 视觉朝向
            },
            preAtk: new() // 攻击前
            {
                PreTime = 3, // 前摇时间
                OnChange = (_) =>
                {
                    if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI
                        return;
                    Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                    SwingHelper.SetRotVel(Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()); // 朝向
                }
            },
            postAtk: new() // 攻击后
            {
                PostMaxTime = 20, // 后摇最大时间
                PostAtkTime = 3, // 后摇切换时间
            }, onAtk: new() // 攻击时
            {
                SwingTime = 10, // 挥舞时间
                TimeChange = swingChange, // 时间变化函数
                OnHit = onHitEffect,
                OnChange = (_) =>
                {
                    Player.fullRotation = 0;
                    Player.legRotation = 0;

                },
                OnUse = (_) =>
                {
                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation, Player.direction * 0.4f, 0.1f);
                    Player.legRotation = -Player.fullRotation;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                }
            }, SwingHelper, Player);

            SwingHelper_GeneralSwing Spurt = new(this, // 突刺
            setting: new() // 设置
            {
                SwingLenght = Projectile.Size.Length(),// 挥舞长度
                ChangeCondition = () => Player.controlUseItem,
                SwingRot = MathHelper.Pi, // 挥舞角度
                preDraw = drawProj,
                SwingDirectionChange = false, // 挥舞方向变化
                StartVel = -Vector2.UnitX,// 起始速度朝向
                VelScale = new Vector2(1, 0.0001f), // 速度缩放
                VisualRotation = 0, // 视觉朝向
            },
            preAtk: new() // 攻击前
            {
                PreTime = 3, // 前摇时间
                OnChange = (_) =>
                {
                    if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI
                        return;
                    Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                    SwingHelper.SetRotVel(Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()); // 朝向
                }
            },
            postAtk: new() // 攻击后
            {
                PostMaxTime = 60, // 后摇最大时间
                PostAtkTime = 10, // 后摇切换时间
            }, onAtk: new() // 攻击时
            {
                SwingTime = 5, // 挥舞时间
                TimeChange = swingChange, // 时间变化函数
                OnHit = onHitEffect,
                OnChange = (_) =>
                {
                    Player.fullRotation = 0;
                    Player.legRotation = 0;

                },
                OnUse = (_) =>
                {
                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation, Player.direction * 0.4f, 0.1f);
                    Player.legRotation = -Player.fullRotation;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                }
            }, SwingHelper, Player);

            SwingHelper_GeneralSwing ChangeSlash = new(this, // 蓄力斩
            setting: new() // 设置
            {
                SwingLenght = Projectile.Size.Length(),// 挥舞长度
                ChangeCondition = () => Player.controlUseTile,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2, // 挥舞角度
                preDraw = drawProj2,
                SwingDirectionChange = false, // 挥舞方向变化
                StartVel = -Vector2.UnitX.RotatedBy(-0.4f),// 起始速度朝向
                VelScale = new Vector2(2, 0.3f), // 速度缩放
                VisualRotation = 0.7f, // 视觉朝向
            },
            preAtk: new() // 攻击前
            {
                PreTime = 5, // 前摇时间
                OnStart = (Swing) =>
                {
                    if (Swing.setting.ChangeCondition()) // 蓄力
                    {
                        if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI,多人用
                            return;
                        Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                        SwingHelper.SetRotVel(Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()); // 朝向
                        if (Projectile.ai[1] > 2) // 每蓄力2帧
                        {
                            Projectile.ai[1] = 0;
                            if (Projectile.damage < Projectile.originalDamage * 2)
                                Projectile.damage++; // 增加1点伤害
                        }
                    }
                },
                OnChange = (_) => Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center,Projectile.velocity.SafeNormalize(default),9f,0.2f,2))
            },
            postAtk: new() // 攻击后
            {
                PostMaxTime = 15, // 后摇最大时间
                PostAtkTime = 2, // 后摇切换时间
            }, onAtk: new() // 攻击时
            {
                SwingTime = 10, // 挥舞时间
                TimeChange = swingChange, // 时间变化函数
                OnHit = onHitEffect
            }, SwingHelper, Player);

            SwingHelper_CountSwing SlashAll = new(this, // 蓄力斩
            setting: new() // 设置
            {
                SwingLenght = Projectile.Size.Length(),// 挥舞长度
                ChangeCondition = () => Player.controlUseTile,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2, // 挥舞角度
                preDraw = drawProj2,
                SwingDirectionChange = false, // 挥舞方向变化
                StartVel = -Vector2.UnitX.RotatedBy(-0.4f),// 起始速度朝向
                VelScale = new Vector2(2, 0.3f), // 速度缩放
                VisualRotation = 0.7f, // 视觉朝向
            },
            preAtk: new() // 攻击前
            {
                PreTime = 5, // 前摇时间
                OnStart = (Swing) =>
                {
                    if (Swing.setting.ChangeCondition()) // 蓄力
                    {
                        if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI,多人用
                            return;
                        Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                        SwingHelper.SetRotVel(Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()); // 朝向
                        if (Projectile.ai[1] > 2) // 每蓄力2帧
                        {
                            Projectile.ai[1] = 0;
                            if (Projectile.damage < Projectile.originalDamage * 2)
                                Projectile.damage++; // 增加1点伤害
                        }
                    }
                },
                OnChange = (_) => Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(default), 9f, 0.2f, 2))
            },
            postAtk: new() // 攻击后
            {
                PostMaxTime = 15, // 后摇最大时间
                PostAtkTime = 2, // 后摇切换时间
            }, onAtk: new() // 攻击时
            {
                SwingTime = 5, // 挥舞时间
                TimeChange = swingChange, // 时间变化函数
                OnHit = onHitEffect,
                OnChange = (_) =>
                {
                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation, -Player.direction * 0.5f, 0.9f);
                    if (Projectile.ai[2] > 0)
                    {
                        if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI,多人用
                            return;
                        Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                        SwingHelper.SetRotVel((Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()) + Main.rand.NextFloatDirection() * 0.2f); // 朝向
                        cecheSystem.CecheList.Add(new MyFristDrawCeche(SwingHelper.Clone() as SwingHelper)); // 生成残留
                    }
                    else
                    {
                        Player.fullRotation = 0;
                        Player.legRotation = 0;
                    }
                },
                OnUse = (_) =>
                {
                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation,Player.direction * 0.5f,0.1f);
                    Player.legRotation = -Player.fullRotation;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                }
            }, SwingHelper, Player,15);
            SwingHelper_CountSwing SlashAllTwo = new(this, // 蓄力斩
            setting: new() // 设置
            {
                SwingLenght = Projectile.Size.Length(),// 挥舞长度
                ChangeCondition = () => Player.controlUseTile,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2, // 挥舞角度
                preDraw = drawProj2,
                SwingDirectionChange = false, // 挥舞方向变化
                StartVel = -Vector2.UnitX.RotatedBy(-0.4f),// 起始速度朝向
                VelScale = new Vector2(2, 0.3f), // 速度缩放
                VisualRotation = 0.7f, // 视觉朝向
            },
            preAtk: new() // 攻击前
            {
                PreTime = 5, // 前摇时间
                OnStart = (Swing) =>
                {
                    if (Swing.setting.ChangeCondition()) // 蓄力
                    {
                        if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI,多人用
                            return;
                        Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                        SwingHelper.SetRotVel(Player.direction == 1 ? (Main.MouseWorld - Player.Center).ToRotation() : -(Player.Center - Main.MouseWorld).ToRotation()); // 朝向
                        if (Projectile.ai[1] > 2) // 每蓄力2帧
                        {
                            Projectile.ai[1] = 0;
                            if (Projectile.damage < Projectile.originalDamage * 2)
                                Projectile.damage++; // 增加1点伤害
                        }
                    }
                },
                OnChange = (_) => Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(default), 9f, 0.2f, 2))
            },
            postAtk: new() // 攻击后
            {
                PostMaxTime = 15, // 后摇最大时间
                PostAtkTime = 2, // 后摇切换时间
            }, onAtk: new() // 攻击时
            {
                SwingTime = 5, // 挥舞时间
                TimeChange = swingChange, // 时间变化函数
                OnHit = onHitEffect,
                OnChange = (_) =>
                {
                    if (Projectile.ai[2] > 0)
                    {
                        Player.fullRotation = MathHelper.Lerp(Player.fullRotation, Player.direction * -0.2f, 0.6f);
                        if (Player.whoAmI != Main.myPlayer) // 其他玩家不处理这个AI,多人用
                            return;
                        Player.ChangeDir((Main.MouseWorld.X - Player.Center.X > 0).ToDirectionInt());
                        SwingHelper.SetRotVel(Main.rand.NextFloat(6.28f)); // 朝向
                        cecheSystem.CecheList.Add(new MyFristDrawCeche(SwingHelper.Clone() as SwingHelper)
                        {
                            Time = 40,
                        }); // 生成残留
                    }
                    else
                    {
                        Player.fullRotation = 0;
                        Player.legRotation = 0;
                    }
                },
                OnUse = (_) =>
                {
                    Player.fullRotation = MathHelper.Lerp(Player.fullRotation, Player.direction * 0.2f, 0.1f);
                    Player.legRotation = -Player.fullRotation;
                    Player.fullRotationOrigin = Player.Size * 0.5f;
                }

            }, SwingHelper, Player, 60);

            noUse.AddSkill(ChangeSlash).AddSkill(SlashAll).AddSkill(SlashAllTwo); // 蓄力技能
            noUse.AddSkill(SwingUp).AddSkill(SwingAcross).AddSkill(SwingDown).AddSkill(Spurt).AddSkill(SwingUp); // 挥舞连段
            CurrentSkill = noUse; // 切换技能为不使用时候的技能
        }
    }
}
