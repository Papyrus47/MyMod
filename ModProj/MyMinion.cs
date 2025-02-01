
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MyMod.ModProj
{
    public class MyMinion : ModProjectile //这是一个召唤兽弹幕的示例，为了便于理解，我不会写太复杂的AI
    {
        Player player => Main.player[Projectile.owner];
        //定义生成弹幕时传入的owner参数对应的玩家,非常重要
        public override void SetStaticDefaults()//本函数每次加载模组时执行一次，用于分配静态属性
        {
            Main.projFrames[Type] = 1;//你的帧图有多少帧就填多少
            ProjectileID.Sets.TrailingMode[Type] = 2;//这一项赋值2可以记录运动轨迹和方向（用于制作拖尾）
            ProjectileID.Sets.TrailCacheLength[Type] = 10;//这一项代表记录的轨迹最多能追溯到多少帧以前(注意最大值取不到)
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;//这一项代表弹幕超过屏幕外多少距离以内可以绘制
                                                                //用于长条形弹幕绘制
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;//设置为true可以被其他召唤物顶替掉
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;//右键锁定敌人
            Main.projPet[Projectile.type] = true;//判定为召唤物
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;//弹幕碰撞伤害对邪教徒减伤25%(追踪弹幕常用)
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽为两格物块长度
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕
            Projectile.light = 0.5f;//自带发光
            Projectile.tileCollide = false;//false就能让他穿墙
            Projectile.timeLeft = 120;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            
            Projectile.penetrate = 1;//表示能穿透几次
            Projectile.ignoreWater = true;//无视液体
            Projectile.minionSlots = 1.5f;//表示该召唤物消耗几个召唤栏位（可以为小数）像我这样就是一个召唤物需要1个半栏位
            Projectile.minion = true;//让弹幕判定为召唤物
            Projectile.DamageType = DamageClass.Summon;//召唤伤害
            base.SetDefaults();
        }
        void MoveToTarget(Vector2 targetPos, float MaxSpeed = 20f, float accSpeed = 0.5f)//运用之前学到的惯性追击
        {
            //原理：比较目标和自己的横向或者纵向坐标差，然后给自己的速度加上向着差值变小前进的加速度
            //如果自己的速度坐标差一样，说明自己正在原理目标，需要更大的加速度，这里我设定的是2倍
            if (Projectile.Center.X - targetPos.X < 0f)
                Projectile.velocity.X += Projectile.velocity.X < 0 ? 2 * accSpeed : accSpeed;
            else
                Projectile.velocity.X -= Projectile.velocity.X > 0 ? 2 * accSpeed : accSpeed;

            if (Projectile.Center.Y - targetPos.Y < 0f)
                Projectile.velocity.Y += Projectile.velocity.Y < 0 ? 2 * accSpeed : accSpeed;
            else
                Projectile.velocity.Y -= Projectile.velocity.Y > 0 ? 2 * accSpeed : accSpeed;
            if (Math.Abs(Projectile.velocity.X) > MaxSpeed)//如果横向速度超越最大值，则回到最大值
                Projectile.velocity.X = MaxSpeed * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > MaxSpeed)//如果纵向速度超越最大值，则回到最大值
                Projectile.velocity.Y = MaxSpeed * Math.Sign(Projectile.velocity.Y);

        }
        void AttackShooting(NPC target)//攻击状态时使用的攻击AI
        {
            Projectile.ai[0]++;//随便拿一个ai0当计时器
            if (Projectile.ai[0] == 30)//每半秒攻击一次
            {
                SoundEngine.PlaySound(SoundID.Item16, Projectile.Center);//播放屁声
                Projectile.ai[0] = 0;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                    (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero),
                   ModContent.ProjectileType<MinionProj>(),//生成我们自己写的弹幕
                   Projectile.damage,
                   Projectile.knockBack,
                   Projectile.owner,//为接下来生成的弹幕提供主人
                   target.whoAmI//这里很关键，ai0我们传入敌人的号码，为接下来生成的弹幕提供目标
                   );
            }
        }
        public override bool? CanCutTiles()
        {
            return false;//我们不想召唤兽会割草
        }
        public override void AI()
        {
            if(player.HasBuff<Buffs.MinionBuff>())//如果玩家有召唤物BUFF
            Projectile.timeLeft = 2;//维持住弹幕的时间
            //召唤物的AI通常为：寻敌 - 移动 - 发射这三部分组成
            NPC target = null;//先设出目标NPC，默认为空
            // 这一段是当你的召唤兽设定了右键锁敌情况下必须要写的部分,防止进行寻敌判定
            if (player.HasMinionAttackTargetNPC)
            {
                target = Main.npc[player.MinionAttackTargetNPC];//让目标为鼠标锁住的敌人
                float between = Vector2.Distance(target.Center, Projectile.Center);
                // 小于2000防止锁住太远的敌人
                if (between > 2000f)
                {
                    target = null;
                }
            }

            if (target == null || !target.active)//如果目标是空的或者失活的，那么重新寻找敌人
            {
                int t = Projectile.FindTargetWithLineOfSight(1500);//寻找1500像素范围内最近敌人号码（不隔墙）
                //这个方法如果在没有敌怪时会返回-1，用来检测是否能找到敌人
                if (t >= 0)
                    target = Main.npc[t];//定义这个NPC为目标
            }
            if (target != null)//如果目标不为空且存活在此处执行攻击性AI
            {
                if (target.active)
                {
                    if (Vector2.Distance(player.Center, target.Center) > 2000)//如果找到的目标距离玩家太远了
                    {
                        Vector2 p = Vector2.Lerp(Projectile.Center, player.Center, 0.1f);
                        Projectile.velocity = p - Projectile.Center;//直接强制回归，不要继续攻击了
                        return;//我们的AI就不需要继续往下走了
                    }
                
                    Vector2 Position = target.Center + (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero) * 150;
                    //为什么要这么设置目标坐标呢，因为作为一个远程类召唤兽，保持一定距离进行射击才符合逻辑
                    MoveToTarget(Position, 24, 0.3f);//设置追击目标位置，最大速度，加速度
                    AttackShooting(target);//进行攻击AI
                }
            }
            else//否则说明没目标了，执行回归待机运动
            {
                Vector2 mypos = player.Center + new Vector2(0,-200);
                float dis = Projectile.Distance(mypos);//到玩家中心的距离
                if (dis > 1200)//距离玩家过远时加速回归
                {
                    Vector2 p = Vector2.Lerp(Projectile.Center, player.Center, 0.1f);
                    Projectile.velocity = p - Projectile.Center;
                }
                else if(dis > 620)//中程时，作惯性追击运动
                {
                    MoveToTarget(mypos, 20, 0.4f);//对着目标做追击运动
                }
                else//在玩家近处时，做待机运动
                {
                    StandByLinearQueue(player.Center, 60);
                }
            }
        }
        void SimpleStandBy(Vector2 position,float speed = 0.3f)//简单待机
        {
            if (Projectile.localAI[1] < 3)//弹幕刚生成的时候position有问题，所以不能直接插值，很难受
            {
                Projectile.position = position;//快速逼近目标点
                Projectile.localAI[1]++;
                return;
            }
          
            if (Projectile.Center.X < position.X)
            {
                Projectile.velocity.X += speed;
            }
            if(Projectile.Center.X > position.X)
            {
                Projectile.velocity.X -= speed;
            }
            if(Projectile.Center.Y < position.Y)
            {
                Projectile.velocity.Y += speed;
            }
            if(Projectile.Center.Y > position.Y)
            {
                Projectile.velocity.Y -= speed;
            }
        }
        //有序排列:有序排列又可以分为平行排列和圆形排列，圆形排列又可以是动态圆形排列
        void StandByLinearQueue(Vector2 center,float inter = 50)//平行排列待机 (光棱剑类) 参数为排队的起点
        {
            int index = 0;//定义序号为0
            foreach(var proj in Main.projectile)//遍历所有弹幕
            {
                if(proj.type == Projectile.type && proj.active && proj.whoAmI < Projectile.whoAmI)
                {
                    //检测是否有比自己先生成的同种弹幕,如果有说明自己的序号要后移一位
                    index++;
                }
            }
            index += 1;//为了让弹幕不从起点开始排
            Vector2 pos = center + new Vector2(-player.direction * inter * index, 0);//平行排列,玩家朝哪边就往背面走
            if (Projectile.localAI[1] < 3)//弹幕刚生成的时候position有问题，所以不能直接插值，很难受
            {
                Projectile.position = pos;//快速逼近目标点
                Projectile.localAI[1]++;
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.Center, pos, 0.1f) - Projectile.Center;//快速逼近运动
            }
        
        }
        void StandByCircularQueue(Vector2 center,float range = 200,float speed = 0.1f)//动态圆周待机运动,参数分别为圆心，半径，转速率
        {
            int index = 0;//定义序号为0
            foreach (var proj in Main.projectile)//遍历所有弹幕
            {
                if (proj.type == Projectile.type && proj.active && proj.whoAmI < Projectile.whoAmI)
                {
                    //检测是否有比自己先生成的同种弹幕,如果有说明自己的序号要后移一位
                    index++;
                }
            }
            int MaxIndex = player.ownedProjectileCounts[Type];//获取场上该弹幕总数
            float rad = MathHelper.TwoPi / MaxIndex;//用2pi除以总数得到两个弹幕之间的间隔弧度
            float GlobalRot = Main.GameUpdateCount * speed;//这是一个原版提供的全局计时器,用来给周期运动作计时器很好使
            Vector2 pos = center + (rad * index + GlobalRot).ToRotationVector2() * range;//圆周排列
            if(Projectile.localAI[1] < 3)//弹幕刚生成的时候position有问题，所以不能直接插值，很难受
            { 
                Projectile.position = pos;//快速逼近目标点
                Projectile.localAI[1]++;                          
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.Center, pos, 0.1f) - Projectile.Center;//快速逼近运动
            }
            
        }
        #region 寻敌算法（如果有兴趣可以看看）
        void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // 这是用来寻找敌人的AI方法
            distanceFromTarget = 1500f;//1500像素范围内寻找敌人
            targetCenter = Projectile.position;//先把变量初始化为弹幕位置
            foundTarget = false;//还没找到敌人，所以把这个bool赋为false

            // 这一段是当你的召唤兽设定了右键锁敌情况下必须要写的部分,防止进行寻敌判定
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];//让目标为鼠标锁住的敌人
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // 小于2000防止锁住太远的敌人
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }
            //下面这段是寻找敌人的迭代算法
            if (!foundTarget)
            {

                for (int i = 0; i < Main.maxNPCs; i++)//从0号到199号，也就是所有的NPC号码
                {
                    NPC npc = Main.npc[i];//遍历所有NPC

                    if (npc.CanBeChasedBy())//如果这个NPC可以被作为敌人锁定（也就是可被追踪）
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);//这是npc到弹幕的距离
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;//判断上一个找到的目标和这个npc哪个近
                        bool inRange = between < distanceFromTarget;//如果这个近
                        bool lineOfSight =
                            Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // 这一行是判断目标和召唤兽之间是否隔墙

                        bool closeThroughWall = between < 100f;//如果距离敌人过近，那么无视墙壁

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            //在迭代完之后会找到一个最近的NPC
        }
        #endregion
        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            //同时，需要进行的绘制在这里面写就好

            Texture2D texture = TextureAssets.Projectile[Type].Value;//声明本弹幕的材质
            Rectangle rectangle = new Rectangle(//因为手动绘制需要自己填写帧图框,所以要先算出来
                0,//这个框的左上角的水平坐标(填0就好)
                texture.Height / Main.projFrames[Type] * Projectile.frame,//框的左上角的纵向坐标 
                texture.Width, //框的宽度(材质宽度即可)
                texture.Height / Main.projFrames[Type]//框的高度（用材质高度除以帧数得到单帧高度）
                );

            //要制作拖尾，首先要建立一个for循环语句，从0一直走到轨迹末端
            //这里我们介绍一个能产生高亮叠加绘制的办法（A=0）
            Color MyColor = Color.White; MyColor.A = 0;//让A=0是为了能直接叠加颜色
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)//循环上限小于轨迹长度
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                //定义一个从新到旧由1逐渐减少到0的变量，比如i = 0时，factor = 1
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                //由于轨迹只能记录弹幕碰撞箱左上角位置，我们要手动加上弹幕宽高一半来获取中心
                Main.EntitySpriteDraw(texture, oldcenter, rectangle, MyColor * factor,//颜色逐渐变淡
                    Projectile.oldRot[i],//弹幕轨迹上的曾经的方向
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                     new Vector2(2),
                     SpriteEffects.None, 0);
            }
            //由于tr绘制是先执行的先绘制，所以要想残影不覆盖到本体上面，就要先写残影绘制

            Main.EntitySpriteDraw(  //entityspritedraw是弹幕，NPC等常用的绘制方法
                texture,//第一个参数是材质
                Projectile.Center - Main.screenPosition,//注意，绘制时的位置是以屏幕左上角为0点
                                                        //因此要用弹幕世界坐标减去屏幕左上角的坐标
                rectangle,//第三个参数就是帧图选框了
                Color.White,//第四个参数是颜色，这里我们用自带的lightcolor，可以受到自然光照影响
                Projectile.rotation,//第五个参数是贴图旋转方向
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                //第六个参数是贴图参照原点的坐标，这里写为贴图单帧的中心坐标，这样旋转和缩放都是围绕中心
                new Vector2(2),//第七个参数是缩放，X是水平倍率，Y是竖直倍率
                SpriteEffects.None,
                //第八个参数是设置图片翻转效果，需要手动判定并设置spriteeffects
                0//第九个参数是绘制层级，但填0就行了，不太好使
                );

            return false;//return false阻止自动绘制
        }
    }

}