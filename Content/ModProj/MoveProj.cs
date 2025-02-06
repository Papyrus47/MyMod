
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MyMod.Content.ModProj
{
    public class MoveProj : ModProjectile //基础运动教学
    {
        Player player => Main.player[Projectile.owner];
        //定义生成弹幕时传入的owner参数对应的玩家
        public override void SetStaticDefaults()//本函数每次加载模组时执行一次，用于分配静态属性
        {
            Main.projFrames[Type] = 1;//你的帧图有多少帧就填多少
            ProjectileID.Sets.TrailingMode[Type] = 2;//这一项赋值2可以记录运动轨迹和方向（用于制作拖尾）
            ProjectileID.Sets.TrailCacheLength[Type] = 10;//这一项代表记录的轨迹最多能追溯到多少帧以前(注意最大值取不到)
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;//这一项代表弹幕超过屏幕外多少距离以内可以绘制
                                                                //用于长条形弹幕绘制

            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽为两格物块长度
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕                                          
            Projectile.tileCollide = false;//false就能让他穿墙
            Projectile.timeLeft = 1220;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.DamageType = DamageClass.Ranged;//魔法伤害
            Projectile.penetrate = 1;//表示能穿透几次
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        Vector2 coord = Vector2.Zero;//定义一个坐标字段
        float timer = 0;//定义一个计时器字段
        float startVel = 0;//预存速度方向字段
        public override void AI()//本次重点在于AI
        {
            //AI内的东西在非暂停状态下，每帧执行一次，是弹幕表现行为的基础

            //首先是匀速运动,在无任何AI的情况下，弹幕速度是不会变的，所以匀速运动不需要修改速度
            Projectile.rotation = Projectile.velocity.ToRotation();//让贴图方向等于速度方向
                                                                   // 求一个x = 1，y = 1的向量的长度
                                                                   // 勾股定理
                                                                   // 根号（x 的 平方 + y 的平方） = 向量长度
                                                                   // Projectile.velocity.Length(); 获取向量的长度
            #region 速度赋予向量
            //下面介绍直接对速度赋予向量的效果
            //if (Projectile.timeLeft == 1210)
            //{
            //    Projectile.velocity = new Vector2(0, 10);
            //}
            //if (Projectile.timeLeft == 1200)
            //{
            //    Projectile.velocity = new Vector2(-10, 0);
            //}
            //if (Projectile.timeLeft == 1100)
            //{
            //    Projectile.velocity = new Vector2(10, -10);
            //}
            #endregion
            #region 加减速运动
            //下面介绍加速运动和减速运动
            //首先是直接成倍率的加减速
            //Projectile.velocity = Projectile.velocity * 0.95f;//速度每帧减少5%，一秒之后速度只剩原来的4%了
            //其次是具体增加数值的加减速，这个反而更复杂一点
            //Projectile.velocity = (Projectile.velocity.Length() + 0.05f) * Projectile.velocity.SafeNormalize(Vector2.Zero);
            //速率数值即速度向量.length()，我们先获取速率再加一，再乘以单位速度,就能让速率每帧 + 1

            #endregion
            #region 曲线运动
            //好了，下面介绍弹幕速度进行偏转后产生的曲线运动吧
            //Projectile.velocity = Projectile.velocity.RotatedBy(0.034f);//速度每帧顺时针偏转0.034弧度

            #endregion
            #region 折线运动
            //如果我们只在特定的时间节点偏转速度，那么就能出现折线运动
            //if (Projectile.timeLeft % 20 == 9)//每20帧中的第十帧执行（0是第一帧）
            //{
            //    Projectile.velocity = Projectile.velocity.RotatedBy(0.8f);//顺时针偏转0.8弧度
            //}
            //if (Projectile.timeLeft % 20 == 19)//每20帧中的第二十帧执行（0是第一帧）
            //{
            //    Projectile.velocity = Projectile.velocity.RotatedBy(-0.8f);//逆时针偏转0.8弧度
            //}
            #endregion
            #region 正弦函数运动
            //由于函数运动需要一个原点作为参照，所以我们新建一个二维向量字段
            //我们用的最多的函数运动就是正弦运动了
            //这里写一个简单的水平向右的正弦运动
            /* if(coord == Vector2.Zero)//弹幕生成的那一刻，这个字段默认为0向量，我们赋予它弹幕此时的坐标
             {
                 Projectile.velocity *= 0;//这时候我们不需要速度
                 coord = Projectile.Center;
             }
             else //之后coord已经被赋予弹幕的初始坐标了，此时就可以让弹幕动起来了
             {
                 timer++;//计时器每帧+1
                 float height = 250;//定义正弦运动上下摆动幅度
                 float timeLoop = 30f;//定义正弦运动的周期
                 float Xspeed = 15f;//定义弹幕在横向的速度
                 float w = MathHelper.TwoPi / timeLoop;//用2Π除以周期得到角速度
                 Vector2 position = coord + //math.sin这玩意是double，必须强制转换成float
                     new Vector2(timer * Xspeed,//随着时间X轴坐标不断增加
                     height * (float)Math.Sin(timer * w));//Y轴坐标随着时间上下摆动
                 Projectile.velocity = position - Projectile.Center;//最后再让弹幕速度一直朝着函数图像走
             }*/
            #endregion
            #region 极坐标思想
            //上面的正弦运动很好，但是我并不想只让他水平向右，那该怎么办呢?
            /*if (coord == Vector2.Zero)//弹幕生成的那一刻，这个字段默认为0向量，我们赋予它弹幕此时的坐标
            {
                startVel = Projectile.velocity.ToRotation();//先预存这个速度方向
                Projectile.velocity *= 0;//但是我们不能让弹幕有初速度了
                coord = Projectile.Center;
            }
            else //之后coord已经被赋予弹幕的初始坐标了，此时就可以让弹幕动起来了
            {
                timer++;//计时器每帧+1
                float height = 60;//定义正弦运动上下摆动幅度
                float timeLoop = 30f;//定义正弦运动的周期
                float Xspeed = 15f;//定义弹幕在横向的速度
                float w = MathHelper.TwoPi / timeLoop;//用2Π除以周期得到角速度

                Vector2 pos = new Vector2(timer * Xspeed,//随着时间X轴坐标不断增加
                    height * (float)Math.Sin(timer * w));//Y轴坐标随着时间上下摆动
                pos = pos.RotatedBy(startVel);//重点：让整个函数图像偏转成初速度的方向
                Vector2 position = coord + pos;//再加回预存的初始位置
                Projectile.velocity = position - Projectile.Center;//最后再让弹幕速度一直朝着函数图像走
            }*/

            #endregion
            #region 圆周运动及椭圆运动
            //那么，学习完极坐标，想必你对二维向量有一个更好的认识了
            //接下来就让我介绍如何制作围绕某点的圆周运动吧
            /*
            timer++;//我们还是需要一个计时器的，用来控制角度
            float rotation = timer * 0.25f;//随便乘以个小数作为角度使用
            float Range = 150; //圆的半径
            Vector2 originPos = player.Center;//（以玩家中心为原点为例）
            Vector2 circlePos = rotation.ToRotationVector2() * Range;//ToRotVec2方法可以将角度转单位向量，再乘以长度
                                                                     //就可以得到圆周上的点坐标了
           //那么椭圆呢？其实利用的是参数方程的思想，先让圆周点的坐标的X或者Y减少一定倍率
              //再进行向量偏转，最后和原点坐标相加，即可得到任意方向椭圆
            circlePos.Y *= 0.5f;//我这里将圆的Y轴减少50%，形成一个椭圆
            circlePos = circlePos.RotatedBy(-0.5f);//整体偏转0.5弧度试试看
            Projectile.velocity = circlePos + originPos - Projectile.Center;//再对速度赋值
            */
            #endregion
            //至此，基础运动就都介绍完了
            //下面，我将介绍游戏中更广泛使用的一些运动方法
            #region 追击运动
            //追击运动是指弹幕对某个坐标（坐标不确定）进行追赶的运动
            //由于追击运动一般不能限制运动的时间，所以只能以距离和速度作为判断依据
            //追击运动的实现方法有很多，这里我介绍三种常见的办法
            //第一种：直接追击
            /*Vector2 targetPos = Main.MouseWorld;//为了更好演示，我选择鼠标作为被追目标
            if ((targetPos - Projectile.Center).Length() > 8)//如果到目标的距离超过一个值再运动，防止抽搐
            {
                Vector2 vel = Projectile.DirectionTo(targetPos);
                //directionto是泰拉实体自带的获取到目标单位向量的方法！
                //直接追击的优点是速度可控，方向性极好，直奔目标，缺点则是接近目标后容易抽搐,不适合作为NPC的追敌运动
                vel *= MathHelper.Min(16, Projectile.Distance(targetPos));//抽搐问题需要把速度用Min压进距离缓解
                Projectile.velocity = vel;//最后把这个速度交给弹幕
            }
            else
            {
                Projectile.velocity *= 0;//过近就停下吧
            }
            */
            //第二种:正交惯性追击   正交惯性追击是原版使用较多的一种追击手段，运动十分自然，适合飞行单位使用
            /*Vector2 targetPos = Main.MouseWorld;//为了更好演示，我选择鼠标作为被追目标
            float MaxSpeed = 20f;//设定横纵向最大速度
            float accSpeed = 0.5f;//设定横纵向加速度
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
                Projectile.velocity.Y = MaxSpeed * Math.Sign(Projectile.velocity.Y);*/

            ////第三种：插值渐进运动   插值渐进运动是一种在短时间内快速稳定移动到目标处的运动，适合召唤物强制回归等行为
            Vector2 targetPos = Main.MouseWorld;//为了更好演示，我选择鼠标作为被追目标
            Vector2 pos = Vector2.Lerp(Projectile.Center, targetPos, 0.1f);//第三个空是第一个空转变到第二个空的比率
            //通常填0.1以内即可，0.1以上的话太快了
            Projectile.velocity = pos - Projectile.Center;//赋予速度
            #endregion
        }


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
                     new Vector2(1, 1),
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
                new Vector2(1, 1),//第七个参数是缩放，X是水平倍率，Y是竖直倍率
                SpriteEffects.None,
                //第八个参数是设置图片翻转效果，需要手动判定并设置spriteeffects
                0//第九个参数是绘制层级，但填0就行了，不太好使
                );

            return false;//return false阻止自动绘制
        }
    }

}