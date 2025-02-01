
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMod.ModProj.BossProjectile;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;

namespace MyMod.NPCs // 命名空间咱们写文件夹路径
{
    //这是一个类似克苏鲁之眼的飞行BOSS，属于最简单制作的一类(无动作无碰撞，飞行自由)
    //本BOSS包含演出效果：AI运动，基础弹幕设计，限制圈，闪光与震屏，天空，预警线，残影，尾杀
    //本BOSS中所写的射弹，天空，系统等类均有注释，请务必查阅以辅助理解
    [AutoloadBossHead]
    public class PrimaryBoss : ModNPC
    {
        bool Phase2 = false;//进入二阶段的判断
        public static int secondStageHeadSlot = -1;//二阶段大头贴索引
        public override void SetStaticDefaults()//加载这个NPC内容时执行（只有模组加载时执行一次）
        {
            // DisplayName.SetDefault("教程之魔眼");
            Main.npcFrameCount[Type] = 6;//将本NPC的帧图数量注册为6
            NPCID.Sets.TrailingMode[Type] = 3;//拖尾模式设置为3，可以获得oldpos和oldrot
            NPCID.Sets.TrailCacheLength[Type] = 10;//将拖尾数组长度设为10
        }
        public override void Load()//加载这个NPC内容时执行（只有模组加载时执行一次）这个函数内无法使用ModContent
        {
            // 注册二阶段大头贴
            string texture = BossHeadTexture + "2"; // 也就是 "PrimaryBoss_Head_Boss2"
            secondStageHeadSlot = Mod.AddBossHeadTexture(texture);//addbossheadtex可以注册图标然后拿到这个图标的slot
        }

        public override void BossHeadSlot(ref int index)
        {
            int slot = secondStageHeadSlot;
            if (Phase2 && slot != -1)
            {
                // 二阶段使用另一个大头贴
                index = slot;
            }
        }
        public override void SetDefaults()
        {
            NPC.width = 112;//碰撞箱宽7格物块 (7*16) = 112
            NPC.height = 112;//碰撞箱高7格物块(7*16) = 112
            NPC.damage = 25;//普通模式下的碰撞攻击力
            NPC.defense = 16;//防御力(2防=1点免疫)
            NPC.lifeMax = 40000;//最大生命值
            NPC.HitSound = SoundID.NPCHit1;//受伤音效
            NPC.DeathSound = SoundID.NPCDeath2;//死亡音效
            NPC.value = Item.buyPrice(1, 0, 0, 0);//掉落金钱（普通模式为基准）
            NPC.knockBackResist = 0;//击退接受性（0为彻底免疫击退）
            NPC.noGravity = true;//无重力
            NPC.noTileCollide = true;//穿墙
            NPC.aiStyle = -1; // 不使用原版aiStyle
            NPC.boss = true;//令其为boss
            NPC.npcSlots = 10f; //占用10个NPC槽位，阻止战斗期间生成NPC
            NPC.lavaImmune = true;//免疫岩浆伤害
            NPC.friendly = false;//敌对关系
            Music = MusicID.Boss5;//这里可以找原版的音乐，如何找自己的音乐请看二阶段
            NPC.scale = Main.getGoodWorld ? 2 : 1;//FTW世界两倍大小，当然这里只是展示如何判断FTW,其实我不推荐你在ftw做较多改动(随意吧，想改也行)
        }
        void Move(Vector2 targetPos, float MaxSpeed = 20f)//之前教学的惯性追击方法
        {
            float accSpeed = 0.5f;//设定横纵向加速度
            if (NPC.Center.X - targetPos.X < 0f)
                NPC.velocity.X += NPC.velocity.X < 0 ? 2 * accSpeed : accSpeed;
            else
                NPC.velocity.X -= NPC.velocity.X > 0 ? 2 * accSpeed : accSpeed;

            if (NPC.Center.Y - targetPos.Y < 0f)
                NPC.velocity.Y += NPC.velocity.Y < 0 ? 2 * accSpeed : accSpeed;
            else
                NPC.velocity.Y -= NPC.velocity.Y > 0 ? 2 * accSpeed : accSpeed;
            if (Math.Abs(NPC.velocity.X) > MaxSpeed)//如果横向速度超越最大值，则回到最大值
                NPC.velocity.X = MaxSpeed * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > MaxSpeed)//如果纵向速度超越最大值，则回到最大值
                NPC.velocity.Y = MaxSpeed * Math.Sign(NPC.velocity.Y);
        }
        public override void AI() //AI是NPC每游戏刻执行一次的，用于进行各种操作
        {
            //NPC自带4个AI参数用作全局变量：NPC.ai[0],NPC.localAI[0],NPC.ai[1],NPC.localAI[1]
            //那么在本BOSS里，我想拿ai0当计时器，ai1当作阶段计数器，localAI0和1用于其他效果
            //于是我们可以有这么一个基本思路：
            //①：ai0从0开始累加，因为AI()每游戏刻执行一次，所以计时器可以用于控制什么时候执行什么事
            //②：不同的状态有不同的行为，所以用ai1去区分不同的状态
            //因此我们可以把整个大行为分解成许多个状态，每个状态分别执行不同的小行为
            //例如我令ai[1]的0为BOSS出场行为，1，2，3为半血前的一阶段的行为。4为1阶段转换2阶段的过场动作
            //5，6，7为半血后的二阶段的行为。8为2阶段转换到3阶段的动作
            //9,10,11为残血三阶段，12为尾杀

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }//为了保证boss有目标，我们必须在AI开头首先进行一次索敌（像我这样写就好）
             //现在就可以开始写AI了

            Player player = Main.player[NPC.target];//声明目标玩家方便使用
            NPC.ai[0]++;//计时器只需要一直加就行了,然后我们在切换状态的时候顺便清零计时器就行

            //这里是玩家死亡或者天亮后，BOSS脱战
            if(Main.dayTime || player.dead)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -40), 0.035f);//速度渐变到向上40像素每帧
                if(NPC.Distance(player.Center) > 1500)
                {
                    NPC.active = false;
                }
                return;
            }

            //至于NPC的朝向(rotation)，有时候我们需要他一直面向玩家，有时候我又需要它朝着速度方向(比如冲刺时）
            //因此不同状态要对朝向进行不同的赋值,就不统一写了

            switch (NPC.ai[1])//用ai1区分状态
            {
                case 0://选择0作为出场动作的原因是这些变量默认就是0
                       //自然召唤的BOSS一般会离玩家有一段距离，因此我们需要去靠近玩家,等到距离玩家足够近再正式战斗
                    {
                        if (Vector2.Distance(player.Center, NPC.Center) > 1000)//若NPC距离玩家超过1000像素
                        {
                            Move(player.Center);//追击玩家
                        }
                        else
                        {
                            NPC.ai[0] = 0;//清零计时器
                            NPC.ai[1]++;//进入下一个状态（状态1）
                        }
                        NPC.rotation = NPC.AngleTo(player.Center);
                        break;
                    }
                case 1://移动到玩家的左侧或者右侧(具体看BOSS在哪边)并且向玩家发射子弹
                    {
                        
                        float offset = (player.Center.X - NPC.Center.X > 0) ? -500 : 500;
                        //玩家X减去NPC的X大于0说明玩家在NPC右边，反之在左边（这里我把0算作了左边）
                        Vector2 targetPosition = player.Center + new Vector2(offset, 0);//写出目标位置
                        Move(targetPosition);//追击这个目标位置
                        NPC.rotation = NPC.AngleTo(player.Center);
                        //以上是本阶段NPC的行动，接下来要写发射弹幕了
                        if (NPC.ai[0] % 35 == 0 && NPC.ai[0] > 50)//每35帧发射一次，但是前50帧我不想让他发射
                        {
                            Vector2 dir = NPC.DirectionTo(player.Center);//获取npc到玩家的方向向量
                            dir = dir.RotateRandom(0.1f);//进行一次0.1弧度以内的随机偏转,使得弹道更加真实
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                                dir * 10, ProjectileID.DeathLaser, 25, 1.2f, Main.myPlayer);
                            //发射激光(这是个原版弹幕)
                        }
                        if (NPC.ai[0] > 300)//五秒之后进入下一阶段
                        {
                            NPC.ai[0] = 0;//清零计时器
                            NPC.ai[1]++;//进入下一个状态（状态2）
                            if(NPC.life < NPC.lifeMax / 2)
                            {
                                NPC.ai[1] = 4;//如果HP小于一半，则转换到二阶段
                            }
                        }
                        break;
                    }
                case 2://进行5次对着玩家的长程冲刺
                    {
                        if (NPC.ai[0] < 40)//前40帧先不冲刺，营造一下要冲刺的感觉
                        {
                            Move(player.Center, 7f);//追击玩家，但是速度压到7，体现蓄力感
                            NPC.rotation = NPC.AngleTo(player.Center);//方向朝着玩家
                        }

                        //有时候我们想每隔一定的时间重复一个动作(比如冲刺)，但是又不想复制一堆同样的代码，费时费力不划算
                        //所以可以利用一个全局字段，每次执行完这个动作后就加一，累计足够的次数后再跳出
                        if (NPC.localAI[0] < 5)//直到NPC.localAI[0]为5才不会执行这玩意，也就是我会冲刺5次
                        {
                            if (NPC.ai[0] == 40)//第40帧的时候进行第一次冲刺
                            {
                                NPC.velocity = NPC.DirectionTo(player.Center) * 24;//没错就是这么直接,把NPC速度赋值为到玩家的向量
                                NPC.rotation = NPC.velocity.ToRotation();
                                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);//播放吼叫音效
                            }
                            else if (NPC.ai[0] > 70 && NPC.ai[0] < 100)//冲刺半秒后停下
                            {
                                NPC.velocity *= 0.91f;//尽快减速，并且在此期间朝向不断转向玩家，表现掉头效果
                                float targetRotation = NPC.AngleTo(player.Center);
                                NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.1f);
                                //angleLerp是一种角度插值工具
                            }
                            else if (NPC.ai[0] == 100)//计时器到100帧时回到39，这样我们就又可以进行一次完整的冲刺了
                            {
                                NPC.ai[0] = 39;
                                NPC.localAI[0]++;//冲刺统计次数+1
                            }
                        }
                        else
                        {
                            NPC.localAI[0] = 0;//用完了记得归零，我们其他的状态也要用。

                            NPC.ai[0] = 0;//清零计时器
                            NPC.ai[1]++;//进入下一个状态（状态3）
                            if (NPC.life < NPC.lifeMax / 2)
                            {
                                NPC.ai[1] = 4;//如果HP小于一半，则转换到二阶段
                            }
                        }
                        break;
                    }
                case 3://再次接近玩家，但与玩家始终保持一段距离，并且发射偶数狙
                    {
                        Vector2 target = player.Center + player.DirectionTo(NPC.Center) * 500;//将目标位置定在玩家朝着boss500像素处
                        Move(target);//向着目标位置追击
                        NPC.rotation = NPC.AngleTo(player.Center);//朝向向着玩家
                        if(NPC.ai[0] % 5 == 0 && NPC.ai[0] > 45)//45帧以后并且每1/12秒发射一次偶数狙
                        {
                            for(int i = -3; i <= 3; i += 2)//偶数狙意味着弹道相对于玩家与NPC的连线成轴对称
                            {
                                float rot = i * MathHelper.ToRadians(15);//torarians可以直观地从度数转换为弧度
                                //因此这里的发射偏转角就从-45度到-15度到+15度到+45度
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                                    NPC.DirectionTo(player.Center).RotatedBy(rot) * 10f,ModContent.ProjectileType<HostileProj_0>(),
                                    25, 1,player.whoAmI
                                    );
                                //发射邪教徒的火球,因为火球有一定程度的追踪效果，所以偶数狙也可以起到逼迫玩家的作用
                            }
                        }
                        if(NPC.ai[0] > 150)
                        {
                            NPC.ai[0] = 0;//清零计时器
                            NPC.ai[1] = 1;//回到状态1
                            if (NPC.life < NPC.lifeMax / 2)
                            {
                                NPC.ai[1] = 4;//如果HP小于一半，则转换到二阶段
                            }
                        }
                        break;
                    }
                //以上是全部的一阶段流程了，本BOSS选择的是每次切换状态时检测血量，如果血量小于一半就转换到二阶段
                case 4://本状态为过场动作，锁定血量并且旋转，抛出电火花
                    {
                        NPC.dontTakeDamage = true;//我并不想让NPC在转阶段的时候被打.所以给个无敌
                        if(NPC.ai[0] < 30)
                        {
                            NPC.velocity *= 0.9f;//尽快减速,进入旋转模式
                        }
                        else if(NPC.ai[0] < 150)
                        {
                            //旋转两秒，旋转的速度先增加，再减小，符合自然运动规律
                            //我们设第一秒，旋转角速度不断增加，增加到1.2rad
                            NPC.velocity *= 0.9f;//减速
                            if(NPC.ai[0] < 30 + 60)
                            NPC.localAI[0] += 0.8f/60f;//还是用localai0作为旋转角速度,在第一秒内，由0加到了1.2rad
                            else if(NPC.ai[0] < 30 + 120)
                            {
                                NPC.localAI[0] -= 0.8f / 60f; //在第2秒内，由0.8减少到了0rad
                            }
                            if(NPC.ai[0] == 30 + 60)//在旋转一秒之后进入二阶段贴图状态！
                            {
                                Phase2 = true;
                            }
                            NPC.rotation += NPC.localAI[0];//顺时针旋转指定的角速度
                            for(int i = 0; i < 4; i++)//重复执行四次（抛出电火花）
                            {
                                //(由于填写了width和height,粒子生成位置是随机的。所以尽管重复执行，也会有变化)
                                Vector2 vel = NPC.rotation.ToRotationVector2();//设置一个和NPC朝向相同的单位向量
                                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);//生成粒子
                                d.velocity = vel * 10;//令生成的粒子的初速度为这个向量的十倍
                            }
                        }
                        else if(NPC.ai[0] < 200)
                        {
                            float targetRotation = NPC.AngleTo(player.Center);
                            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.1f);//朝向扶正，面对玩家，准备战斗
                        }
                        else
                        {
                            NPC.dontTakeDamage = false;//取消无敌
                            NPC.ai[0] = 0;NPC.ai[1]++;//进入状态5，也就是二阶段的第一个状态
                            LimitCenter = NPC.Center;//令限制圈的中心等于NPC中心，我们要开始加限制圈了
                            LimitRange = 1000;//令限制圈的半径等于1000，相当于屏幕的高

                        }

                        //二阶段之后新增限制圈，因此我们需要在转换阶段的时候做一个逐渐增大的粒子圈
                        if(NPC.ai[0] > 150 && NPC.ai[0] < 200)
                        {
                            float range = (NPC.ai[0] - 150) / 50f;//这样可以让这个东西随着时间从150到200，从0线性增加到1
                            range *= 1000;//一千倍，换算为真实半径
                            for(int i = 0;i < 25; i++)//每帧生成25次粒子，够用了
                            {
                                Dust.NewDust(NPC.Center + Main.rand.NextVector2CircularEdge(range, range),//随机在这个范围的边缘生成
                                    0, 0, DustID.ShadowbeamStaff);
                            }
                        }
                        break;
                    }
                case 5://本状态为喷火
                    {
                        LimitCircle(700, NPC.Center);//收紧限制圈，逼迫玩家旋转躲避
                        //二阶段的AI比一阶段更加复杂，每个状态内包含的行为更多，不过仍然只需要依照时间执行行为
                        //二阶段的第一个状态我想做一个边喷火边发射火球的行为
                        //喷火的调转速度不需要太快，因此我们换一种写法
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.AngleTo(player.Center), 0.04f);//角度渐变
                        NPC.velocity = (NPC.rotation).ToRotationVector2() * 6.7f;
                        if(NPC.ai[0] > 45 && NPC.ai[0] <= 270 && NPC.ai[0] % 5 == 0)//45帧开始喷火，270帧停止喷火，持续180帧也就是3秒
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity.SafeNormalize(Vector2.Zero) * 32,
                              //为了使喷火看上去更自然，生成位置往前增加32码
                              NPC.velocity.SafeNormalize(Vector2.Zero) * 9,//生成弹幕的速度为NPC的速度的单位向量乘以7
                              ModContent.ProjectileType<TutorialFlame>(), 24, 1, Main.myPlayer);
                            //因为喷火属于高密度发射，因此每1/12秒发射一次弹幕
                        }
                        else if(NPC.ai[0] > 270)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1]++;//进入二阶段的第二状态
                            if (NPC.life < NPC.lifeMax * 0.1f && Main.expertMode)
                            {
                                NPC.ai[1] = 8;//如果专家模式以上，并且HP小于10%，则转换到三阶段
                            }
                        }
                        break;
                    }
                case 6://本状态进行5~7（随难度增加）次短程快速冲刺，最后进行一次长程预判冲刺
                    {
                        LimitCircle(1500, NPC.Center);//放大限制圈
                        if(NPC.ai[0] == 1)//第一帧先配置冲刺的数量,还是拿localai0来用
                        {
                            NPC.localAI[0] = Main.masterMode ? 7 : Main.expertMode ? 6 : 5;//大师7专家6普通5
                        }
                        else if(NPC.ai[0] < 40)//还是40帧前进行准备动作
                        {
                            Vector2 targetPos = player.Center + player.DirectionTo(NPC.Center) * 500;
                            Move(targetPos, 15);
                            NPC.rotation = NPC.AngleTo(player.Center) ;//方向朝着玩家
                        }
                        else if (NPC.localAI[0] > 0)//直到NPC.localAI[0]减到0才不会执行这玩意，也就是我会冲刺7/6/5次
                        {
                            if (NPC.ai[0] == 40)//第40帧的时候进行第一次冲刺
                            {
                                //这里有一个需要注意的地方，就是高频率高速冲刺，如果直接瞄准玩家，在玩家机动性不够高时很容易无解
                                //因此我们不妨学一下原版克眼的机制，让这个冲刺只能沿着45度/135度/225度/315度走
                                Vector2 dir = NPC.DirectionTo(player.Center);//先求出到玩家的方向向量
                                dir.X = dir.X >= 0 ? 1 : -1;//将这个方向向量的X单位化
                                dir.Y = dir.Y >= 0 ? 1 : -1;//将这个方向向量的Y单位化
                                //此时你得到了一个45度/135度/225度/315度中，最接近玩家的那个方向的向量，这个向量长为根号二
                                dir = dir.SafeNormalize(Vector2.Zero);//先标准化
                                dir *= 18;//此时我们再把这个向量倍增18倍，就能得到最终的速度
                                NPC.velocity = dir;
                                NPC.rotation = NPC.velocity.ToRotation();
                                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);//播放狂暴吼叫音效
                            }
                            else if (NPC.ai[0] > 14 + 40 && NPC.ai[0] < 65)//冲刺15帧后停下
                            {
                                NPC.velocity *= 0.9f;//减速
                            }
                            else if (NPC.ai[0] == 65)//计时器到70帧时回到39，这样我们就又可以进行一次完整的冲刺了
                            {
                                NPC.ai[0] = 39;
                                NPC.localAI[0]--;//剩余次数-1
                            }
                        }
                        else //这时候说明前几次冲刺结束了，那么我们进行一次预判大冲刺
                        {
                            if(NPC.ai[0] == 40)//先吼叫并且释放一个预警线
                            {
                                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(),NPC.Center,Vector2.Zero,
                                    ModContent.ProjectileType<WarningLine>(),0,0,Main.myPlayer,
                                    NPC.whoAmI,//注意，ai0存入本NPC的索引
                                    0);
                            }
                            if(NPC.ai[0] < 80)//在计时器从39到达80这段时间内先减速，并且瞄准预判玩家。
                            {
                                Move(player.Center + player.DirectionTo(NPC.Center) * 300, 8);//低速向玩家300距离的位置靠近
                                Vector2 offset = player.velocity * 45 + player.Center;//这个向量相当于预判玩家以当前的速度在接下来45帧(3/4秒)内移动到的位置
                                NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.AngleTo(offset), 0.07f);//角度渐变

                            }
                            else if(NPC.ai[0] == 80)//第80帧时进行预判冲刺
                            {
                                Vector2 offset = player.velocity * 45 + player.Center;//这个向量相当于预判玩家以当前的速度在接下来30帧(半秒)内移动到的位置
                                NPC.velocity = NPC.DirectionTo(offset) * 40f;//以40f的速度向着这个offset位置前进！
                                NPC.rotation = NPC.velocity.ToRotation();
                            }
                            else if(NPC.ai[0] > 100 && NPC.ai[0] < 120)//100帧到120帧这段时间减速
                            {
                                NPC.velocity *= 0.925f;//减速
                            }
                            else if(NPC.ai[0] == 120)//120帧时结束吧
                            {
                                NPC.localAI[0] = 0;//用完了记得归零(当然本来就是0了)
                                NPC.ai[0] = 0;//清零计时器
                                NPC.ai[1]++;//进入下一个状态（状态7）
                                if (NPC.life < NPC.lifeMax * 0.1f && Main.expertMode)
                                {
                                    NPC.ai[1] = 8;//如果专家模式以上，并且HP小于10%，则转换到三阶段
                                }
                            }
              
                        }
                        break;
                    }
                case 7://本状态BOSS将在随机的几个位置释放对着玩家的激光，玩家需注意躲避，boss本身会跟着玩家运动
                    {
                        LimitCircle(1000, NPC.Center);
                        Vector2 targetPos = player.Center + player.DirectionTo(NPC.Center) * 400;
                        Move(targetPos, 15);
                        NPC.rotation = NPC.AngleTo(player.Center);
                        if(NPC.ai[0] % 60 == 50 && NPC.ai[0] < 300)//从50帧开始，每一秒在四个随机位置生成对着玩家的激光，持续到300帧
                        {
                            for(int i = 0;i < 4; i++)
                            {
                                Vector2 randomPos = NPC.Center + Main.rand.NextVector2CircularEdge(1200, 1200);//1200的圆周上的随机位置
                                Vector2 vel = player.DirectionFrom(randomPos);//到玩家的单位向量
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), randomPos, vel, ModContent.ProjectileType<WarningLine_2>(),
                                    0, 0, Main.myPlayer);//生成预警线(预警线自己会生成激光)
                            }
                        }
                        if(NPC.ai[0] >= 400)//最后一轮激光会持续一段时间，因此我们留出100帧用来给玩家缓冲
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 5; //回到二阶段第一状态
                            if (NPC.life < NPC.lifeMax * 0.1f && Main.expertMode)
                            {
                                NPC.ai[1] = 8;//如果专家模式以上，并且HP小于10%，则转换到三阶段
                            }
                        }
                        break;
                    }
                case 8://本状态BOSS将进入3阶段(专家以上)，进行咆哮和震屏
                    {
                        LimitCircle(800, NPC.Center);
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.AngleTo(player.Center), 0.05f);
                        if (NPC.ai[0] <= 60)//前一秒，减速，并且蓄积粒子，形成蓄力效果，并且镜头渐渐移动到BOSS上
                        {
                            NPC.velocity *= 0.95f;//每帧速度减少5%
                            NPC.dontTakeDamage = true;//无敌，防止偷伤害
                            for (int i = 0; i < 3; i++)//每帧连续执行三次
                            {
                                Vector2 edge = NPC.Center + Main.rand.NextVector2CircularEdge(100, 100);//在半径100的圆周上随机位置
                                Vector2 velocity = NPC.DirectionFrom(edge);//获取这个位置到NPC的单位向量
                                Dust d = Dust.NewDustDirect(edge, 0, 0, DustID.RedTorch, 0, 0, 0, default, 1);
                                d.velocity = velocity * 24;//赋予粒子向NPC的速度,因为粒子很快就会消失，所以需要尽快移动到NPC中心
                                d.noGravity = true;//粒子无重力
                            }
                            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().TargetScreenPos = NPC.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                            //别忘了减去屏幕长宽一半。不然就是镜头左上角在NPC上了
                        }
                        else if (NPC.ai[0] < 180)
                        {
                            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().TargetScreenPos = NPC.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeTimer = 2;//因为我们是每帧赋值，所以直接赋值2，在这个状态结束后就会立马归零
                            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeScale = 15;//震动幅度设为15
                            if (NPC.ai[0] == 61)//进行吼叫
                            {
                                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);//吼叫
                            }
                            if(NPC.ai[0] % 15 == 0)//每1/4秒
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                                    ModContent.ProjectileType<ModProj.BossProjectile.RoundLight>(), 0, 0, Main.myPlayer);
                                //释放一次光效弹幕
                                int randomHeal = Main.rand.Next(1500, 2000);//1500到1999的随机数
                                randomHeal += NPC.lifeMax / 100;//在这个基础上增加最大生命值的1%
                                if (Main.masterMode) randomHeal += randomHeal/2;//大师难度这个数值将会增加50%
                                NPC.life += randomHeal;//回复这个数值的血量
                                //跳字
                                CombatText.NewText(NPC.Hitbox, CombatText.HealLife,//颜色选择现成的healLife
                                    randomHeal);
                            }
                           
                        }
                        else if(NPC.ai[0] == 180)
                        {
                            NPC.dontTakeDamage = false;NPC.ai[0] = 0;NPC.ai[1]++;//进入下一个阶段
                        }
                        break;
                    }
                case 9://本状态，BOSS狂暴，并且进行更加迅猛的冲刺，持续10~12次,并且这次可以进行六个方向上的冲刺
                    {
                        LimitCircle(1500, NPC.Center);//放大限制圈
                        if (NPC.ai[0] == 1)//第一帧先配置冲刺的数量,还是拿localai0来用
                        {
                            NPC.localAI[0] = Main.masterMode ? 12 : 10;//大师12专家10,普通模式到不了三阶段，所以只要判断大师模式即可
                        }
                        else if (NPC.ai[0] < 40)//还是40帧前进行准备动作
                        {
                            Vector2 targetPos = player.Center + player.DirectionTo(NPC.Center) * 400;
                            Move(targetPos, 18);
                            NPC.rotation = NPC.AngleTo(player.Center);//方向朝着玩家
                        }
                        else if (NPC.localAI[0] > 0)//直到NPC.localAI[0]减到0才不会执行这玩意
                        {
                            if (NPC.ai[0] == 40)//第40帧的时候进行第一次冲刺
                            {
                                Vector2 CurrentDir = NPC.DirectionTo(player.Center);//先求出到玩家的方向向量
                                //正好在这里讲一下简单的的迭代算法
                                float difference = 2;//随便定义一个大于1的差值,这个在下面会用到
                                float rotation2 = 0;//声明一个方向值，这个方向在下面会用到
                                for (int i = 0; i < 6; i++)//首先写一个for循环，0~5，总共六次
                                {
                                    float rotation = i * MathHelper.TwoPi / 6f;//这个是每次循环中的方向值
                                    Vector2 Dir = rotation.ToRotationVector2();//转化成向量
                                    //显然，长度不变的两个向量相减，如果它们之间夹角越大，则相减后得到的新向量的长度越大
                                    if ((CurrentDir - Dir).Length() < difference)//如果到玩家方位的方向与当前方向的向量差值长度小于现在的差值
                                    {
                                        difference = (CurrentDir - Dir).Length();//那么就让差值等于这个新的差值
                                        rotation2 = rotation;//并且让方向2等于这个方向
                                    }
                                    //在经过这6次迭代后，一定能找到这六个方向里最靠近目标方向的那个方向，那个方向便赋值到rotation2
                                    //迭代算法一般用于寻找最近敌人
                                }
                                //在六个方向里寻找最接近玩家的，然后赋值给速度.是一种平衡手段，我只是不想直接冲向玩家，导致机动性不够用
                                NPC.velocity = rotation2.ToRotationVector2() * 24f;
                                NPC.rotation = NPC.velocity.ToRotation();
                                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);//播放狂暴吼叫音效
                            }
                            else if (NPC.ai[0] > 14 + 40 && NPC.ai[0] < 65)//冲刺15帧后停下
                            {
                                NPC.velocity *= 0.9f;//减速
                            }
                            else if (NPC.ai[0] == 65)//计时器到70帧时回到39，这样我们就又可以进行一次完整的冲刺了
                            {
                                NPC.ai[0] = 39;
                                NPC.localAI[0]--;//剩余次数-1
                            }
                        }
                        else//冲刺结束后来到玩家附近，在距离符合标准时(这里就不使用定时进入了)进入下一阶段
                        {
                            Vector2 pos = player.Center + player.DirectionTo(NPC.Center) * 300;
                            Move(pos, 20);
                            if(NPC.Distance(player.Center) < 320)//距离小于320时
                            {
                                NPC.ai[0] = 0;NPC.ai[1]++;//进入下一状态
                            }
                        }
                        break;
                    }
                case 10://本状态，BOSS绕着玩家旋转（一秒转半圈），并且发射自击狙
                    {
                        //BOSS转圈时限制圈不应该跟着动，我们固定一下限制圈
                        if(NPC.ai[0] == 1)//第一帧时，记录玩家到NPC的方向,并且产生一个随机数，为1或-1
                        {
                            LimitCenter = player.Center;
                            NPC.localAI[0] = NPC.AngleFrom(player.Center);
                            NPC.localAI[1] = Main.rand.NextBool(2) ? 1 : -1;//这样做是1和-1各二分之一的概率
                        }
                        else if(NPC.ai[0] < 250)
                        {
                            LimitCircle(675, LimitCenter);
                            float speed = 3.14159f/60f;//一秒转半圈（用pi除以一秒得到每帧角速度
                            speed *= NPC.localAI[1];//再由最开始抽取的随机数决定顺时针或逆时针
                            float Range = 600f;
                            NPC.localAI[0] += speed;
                            Vector2 pos = player.Center + NPC.localAI[0].ToRotationVector2() * Range;
                            //这样就找到了玩家350远的圆周上随时间变动的点位。
                            NPC.velocity = Vector2.Lerp(NPC.Center, pos, 0.09f) - NPC.Center;//渐进运动(运动篇讲过)
                            NPC.rotation = NPC.AngleTo(player.Center);
                            if(NPC.ai[0] % 5 == 0)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(player.Center) * 8f, ProjectileID.DeathLaser, 20, 1, Main.myPlayer);
                            }
                        }
                        else if(NPC.ai[0] == 250)
                        {
                            NPC.ai[0] = 0;NPC.ai[1] ++;//下一个阶段
                        }
                        break;
                    }
                case 11://本状态是三阶段最后一个AI，对玩家进行几次预判激光，并且激光会产生纵向射弹
                    {
                        //https://www.bilibili.com/video/BV1rh411w7bJ AI灵感来源
                        if (NPC.ai[0] == 1)//在第一帧时决定
                        {
                            LimitCenter = player.Center;
                            NPC.localAI[0] = Main.masterMode ? 4 : 3;//大师4次，专家3次
                            NPC.ai[0] = 10;//先把计时器加到10，与后面回来的计时统一
                            return;
                        }
                        else
                        {
                            LimitCircle(750, LimitCenter);
                            if (NPC.localAI[0] > 0)//攻击次数没用完时
                            {
                                if (NPC.ai[0] < 50 && NPC.ai[0] > 10)//前40(10~50)帧跟随玩家，在第11帧(现在状态的)时释放跟随型预警线
                                {
                                    Vector2 pos = player.Center + player.velocity * 25;
                                    NPC.rotation = NPC.AngleTo(pos);
                                    if (NPC.ai[0] == 11)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RoundLight2>(),
                                            0, 0, Main.myPlayer, NPC.whoAmI, 40);//预警持续40tick
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<WarningLine_3>(),
                                            0, 0, Main.myPlayer, NPC.whoAmI);
                                    }
                                    else//追击/游击运动
                                    {
                                        Vector2 targetPos = player.Center + player.DirectionTo(NPC.Center) * 400;
                                        Move(targetPos, 15);
                                    }
                                }
                                else if (NPC.ai[0] >= 50 && NPC.ai[0] < 140)//缓冲一下，因为玩家需要躲避激光产生的垂直射弹
                                {
                                    if(NPC.ai[0] > 52)//等1帧再给方向赋值，防止激光受到影响
                                    NPC.rotation = NPC.AngleTo(player.Center);
                                    if(NPC.ai[0] == 50)//在此区间内第一帧时记录的东西
                                    {
                                        NPC.localAI[1] = player.AngleTo(NPC.Center);//存一下玩家到NPC的方向，方便NPC绕着玩家时参照
                                    }
                                    else
                                    {
                                        NPC.localAI[1] += 3.14f / 120f;
                                        Vector2 target = player.Center + NPC.localAI[1].ToRotationVector2() * 400;
                                        NPC.velocity = Vector2.Lerp(NPC.Center, target, 0.09f) - NPC.Center;//渐进到这个位置
                                    }
                                }
                                else if(NPC.ai[0] == 140)
                                {
                                    NPC.ai[0] = 10;
                                    NPC.localAI[0]--;
                                }
                            }
                            else //攻击结束后
                            {
                                NPC.ai[0] = 0;
                                NPC.ai[1] = 9;//回到三阶段第一状态
                            }
                        }
                        break;
                    }
                case 12://最终尾杀，BOSS释放“魔炮”，并且四周吸收子弹，执行完毕后死亡
                    {
                        if (NPC.life > 0)
                            NPC.life -= NPC.lifeMax / 1050;//尾杀我规定是1050帧结束，那么就让BOSS血量每帧减少总血量/1050
                        LimitCircle(1000, NPC.Center);
                        //https://www.bilibili.com/video/BV1Tw411f7Do AI灵感来源
                        if (NPC.ai[0] == 1)//第一帧震动，并且停止运动
                        {
                            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeScale = 12;//幅度12
                            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeTimer = 50;//持续50帧
                            NPC.velocity *= 0;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<RoundLight2>(),
                                          0, 0, Main.myPlayer, NPC.whoAmI, 40);//预警持续180tick
                        }
                        else if(NPC.ai[0] < 180)//前3s蓄力，给玩家准备的时间，顺便从外向内吸收子弹
                        {
                            if (NPC.ai[0] % 50 == 0)//每50帧发射一次
                            {
                                float Range = 1500;//从1500像素远处开始释放弹幕，弹幕向着NPC走，走到NPC处死亡
                                float speed = 6;//6像素每帧的速度
                                for (int i = 0; i < 12; i++)//将圆周12等分
                                {
                                    float rotation = i * (MathHelper.TwoPi / 12f) + NPC.ai[0] * 0.1f;//加上计时器因子，使得发射角度不断变化
                                    Vector2 position = NPC.Center + Range * rotation.ToRotationVector2();
                                    Vector2 velocity = speed * NPC.DirectionFrom(position);
                                    Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                                   position, velocity, ModContent.ProjectileType<HostileProj_2>(), 19, 1, Main.myPlayer);
                                    p.timeLeft = (int)(Range / speed);//求出抵达所需要的时间并赋值给timeleft
                                }
                            }
                        }
                        else if(NPC.ai[0] == 180)
                        {
                            float rand = Main.rand.NextBool(2) ? 2 : -2;//偏移的弧度
                            NPC.rotation = NPC.AngleTo(player.Center) + rand;//向玩家释放激光，随机左右偏移一段弧度
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2(),
                                ModContent.ProjectileType<BigDeathRay>(), 35, 1, Main.myPlayer,NPC.whoAmI);//发射大激光
                            SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);//别忘播放激光音效
                        }
                        else if(NPC.ai[0] < 950)//如何让BOSS总是向更靠近玩家的方向旋转？
                        {
                            if (NPC.ai[0] % 60 == 0)//每秒发射一次
                            {
                                float Range = 1500;//从1500像素远处开始释放弹幕，弹幕向着NPC走，走到NPC处死亡
                                float speed = 4;//4像素每帧的速度
                                for (int i = 0; i < 12; i++)//将圆周12等分
                                {
                                    float rotation = i * (MathHelper.TwoPi / 12f) + NPC.ai[0] * 0.1f;//加上计时器因子，使得发射角度不断变化
                                    Vector2 position = NPC.Center + Range * rotation.ToRotationVector2();
                                    Vector2 velocity = speed * NPC.DirectionFrom(position);
                                    Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(),
                                   position, velocity, ModContent.ProjectileType<HostileProj_2>(), 19, 1, Main.myPlayer);
                                    p.timeLeft = (int)(Range / speed);//求出抵达所需要的时间并赋值给timeleft
                                }
                            }
                            float rspeed = 0.02f;//先规定旋转角速度
                            if(Math.Abs(NPC.AngleTo(player.Center) - NPC.rotation) < rspeed)//如果npc到玩家的夹角比角速度还小，说明此次旋转一定会过头
                            {
                                NPC.rotation = NPC.AngleTo(player.Center);//那么直接让方向与到玩家方向一样，就不会发生抖动情况了
                                return;
                            }
                            Vector2 direction = NPC.DirectionTo(player.Center);//先写出NPC到玩家的单位向量
                            Vector2 clockwise = (NPC.rotation + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
                            Vector2 anticlockwise = (NPC.rotation - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
                            //显然，要比较两个向量哪个与目标夹角更近，就是比较他们与目标向量相减后的长度
                            if ((clockwise - direction).Length() <= (anticlockwise - direction).Length())//如果顺时针的差值更小
                            {
                                NPC.rotation += rspeed;
                            }
                            else
                            {
                                NPC.rotation -= rspeed;
                            }
                        }
                        else if(NPC.ai[0] < 1050)//暴毙退场动画
                        {
                            NPC.rotation += Main.rand.NextFloat(0.4f,0.7f);//一通乱转
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RedTorch,0,0,0,default,3);
                        }
                        else if(NPC.ai[0] == 1050)
                        {
                            if (NPC.life > 0) NPC.life = 0;
                            NPC.checkDead();//执行一次死亡判定
                        }
                        break;
                    }

            }

            if (Phase2)//二阶段新增霓虹天空效果,并且BOSS自身发光
            {
                Lighting.AddLight(NPC.Center, 1, 1, 1);//后面三个参数分别为rgb，比例影响色相与纯度，大小影响强度
                SkyManager.Instance.Activate("NeonSky");//激活我们注册的霓虹天空
                if (!SkyManager.Instance["NeonSky"].IsActive())//如果这个天空没激活
                    SkyManager.Instance.Activate("NeonSky");
                ((NeonSky)SkyManager.Instance["NeonSky"]).Timeleft = 2;//之后每帧赋予这个倒计时2，如果npc不在了，天空自动关闭
                                                                       //因为我在天空里面写了天空自动关闭
                Music = MusicLoader.GetMusicSlot("MyMod/Music/Sentinel");//二阶段音乐切换为自己的音乐
            }

        }
        Vector2 LimitCenter;
        float LimitRange;
        void LimitCircle(float range,Vector2 targetPos)//自制一个限制圈方法，用于简便生成对应范围的限制圈粒子以及拉回玩家
        {
            //我们需要一个localAI[1]存储限制圈半径，以及一个矢量全局变量存储限制圈圆心
            LimitRange = MathHelper.Lerp(LimitRange, range, 0.05f);//半径渐变到目标长度
            LimitCenter = Vector2.Lerp(LimitCenter, targetPos, 0.07f);//限制圈圆心渐变到目标坐标
            for (int i = 0; i < 25; i++)//每帧生成25次粒子，够用了
            {
                Dust.NewDust(targetPos + Main.rand.NextVector2CircularEdge(range, range),//随机在这个范围的边缘生成
                    0, 0, DustID.ShadowbeamStaff);
            }
            //接下来是拉回玩家的操作
            Player player = Main.player[NPC.target];
            if (player.active && !player.dead && !player.ghost)
            {
                float distance = player.Distance(LimitCenter);
                if (distance > LimitRange)//如果玩家离中心超过指定范围
                {
                    //禁用玩家的所有可移动方式
                    player.controlLeft = false;
                    player.controlRight = false;
                    player.controlUp = false;
                    player.controlDown = false;
                    player.controlUseItem = false;
                    player.controlUseTile = false;
                    player.controlJump = false;
                    player.controlHook = false;

                    //禁用狗爪和坐骑
                    if (player.grapCount > 0)
                        player.RemoveAllGrapplingHooks();
                    if (player.mount.Active)
                        player.mount.Dismount(player);
                    //重置玩家速度
                    player.velocity.X = 0f;
                    player.velocity.Y = -0.4f;

                    //拉回玩家
                    Vector2 movement = (LimitCenter - player.Center).SafeNormalize(Vector2.Zero);
                    float length = MathHelper.Min(15, (LimitCenter - player.Center).Length());//把长度压进15
                    player.position += movement * length;//每帧拉回这么多向量.这样的话就是限制圈的主要内容了
                }
            }
        }
        public override void FindFrame(int frameHeight)//frameHeight是系统切割完整图之后的单帧高度
        {
            NPC.frameCounter++;//帧图计时器
            if(!Phase2)//一阶段
            {
                NPC.frame.Y = (int)(NPC.frameCounter / 10 % 3) * frameHeight;
                //NPC的帧数以每1/6秒变换一次的速度从第一张不断变换到第三张，再回到第一张
            }
            else //二阶段
            {
                NPC.frame.Y = (int)(NPC.frameCounter / 10 % 3 + 3) * frameHeight;
                //NPC的帧数以每1/6秒变换一次的速度从第4张不断变换到第6张，再回到第4张
            }
            base.FindFrame(frameHeight);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            //请记住，在同一个函数内，代码从上到下依次执行，因此写在下方的绘制会盖在上方的绘制上。
            //所以我们如果要绘制残影，需要写在绘制本体之上
            Color red = new Color(255, 0, 0, 0);
            var tex = TextureAssets.Npc[Type].Value;
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[Type]; i++)//循环上限小于轨迹长度
            {
                float factor = 1 - (float)i / NPCID.Sets.TrailCacheLength[Type];
                //定义一个从新到旧由1逐渐减少到0的变量，比如i = 0时，factor = 1
                Vector2 oldcenter = NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition;
                //由于轨迹只能记录弹幕碰撞箱左上角位置，我们要手动加上宽高一半来获取中心
                Main.EntitySpriteDraw(tex, oldcenter, NPC.frame, drawColor * factor,//颜色逐渐变淡
                    NPC.oldRot[i] - 1.57f,//轨迹上的曾经的方向
                      new Vector2(NPC.frame.Width / 2, NPC.frame.Height / 2 + 16),//加十六是为了让旋转中心更靠近眼睛中心
                     NPC.scale,
                     SpriteEffects.None, 0);
                if (NPC.ai[1] > 8)//三阶段追加红色残影
                {
                    Main.EntitySpriteDraw(tex, oldcenter, NPC.frame, red * factor,//颜色逐渐变淡
      NPC.oldRot[i] - 1.57f,//轨迹上的曾经的方向
        new Vector2(NPC.frame.Width / 2, NPC.frame.Height / 2 + 16),//加十六是为了让旋转中心更靠近眼睛中心
       NPC.scale,
       SpriteEffects.None, 0);
                }
            }

            //默认的绘制并不是很精准，有时候会因为碰撞箱之类的种种原因，不怎么好用.因此我们需要自己手动绘制然后禁掉原版绘制
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame,//矩形选框直接填这个
                drawColor, NPC.rotation - 1.57f, //这块减去1.57是因为贴图朝下而非朝右
                 new Vector2(NPC.frame.Width / 2, NPC.frame.Height / 2 + 16),//加十六是为了让旋转中心更靠近眼睛中心
                NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool CheckDead()//符合死亡条件时，会先执行这个东西，如果返回false，则阻止本次死亡
        {
            if(NPC.ai[1] < 12 && Main.expertMode)//如果没进入尾杀阶段，并且是专家模式以上，则先锁血无敌，然后返回false
            {
                NPC.life = NPC.lifeMax;//你可以自己决定是回复到1血还是满血，我这里回满血然后在尾杀不断减少，有一种倒计时的感觉
                NPC.dontTakeDamage = true;//无敌，防止被打
                NPC.ai[1] = 12;//切换到尾杀阶段
                NPC.ai[0] = 0;//归零计时器
                return false;
            }
            return base.CheckDead();
        }
        public override void OnKill()//在确认被杀后执行的一系列操作，例如推进世界进度，开启某些条件等,死亡效果不是用这个完成的
        {
            if (!MyWorldSystem.DownedTutorialEye) MyWorldSystem.DownedTutorialEye = true;//记录击杀

            base.OnKill();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if(NPC.life <= 0 && NPC.ai[1] >= 12)//确认死亡状态后，执行死亡效果
            {
                //被杀时产生爆炸和烟雾
                SoundEngine.PlaySound(new SoundStyle("MyMod/Sounds/Explosion"), NPC.Center);//调用自己的音效就是这样了
                Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeScale = 12;//震动12
                Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeTimer = 40;//持续40帧
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                                      ModContent.ProjectileType<ModProj.BossProjectile.RoundLight>(), 0, 0, Main.myPlayer);
                //释放一次光效弹幕
            }
            base.HitEffect(hit);
        }
        public override void SendExtraAI(BinaryWriter writer)//联机同步我不懂，摆了
        {

        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {

        }


    }
   
}