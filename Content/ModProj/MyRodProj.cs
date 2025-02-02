
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
    public class MyRodProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Terrarian;
        //这行可以将弹幕的材质引用指向原版对应路径的贴图，非常好用，这样就不用准备图片了
        //材质路径格式：Terraria/Images/Item_ 等等，具体请参考解包出来的贴图包名称
        Player player => Main.player[Projectile.owner];
        //定义生成弹幕时传入的owner参数对应的玩家
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽为两格物块长度
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕                                          
            Projectile.tileCollide = false;//false就能让他穿墙
            Projectile.timeLeft = 20;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.DamageType = DamageClass.Magic;//魔法伤害
            Projectile.penetrate = 1;//表示能穿透几次
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        public override void AI()
        {
            if (player.channel && Projectile.ai[0] == 0)//只有玩家持续按住鼠标时并且ai0没被改掉时触发
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero)//这是获取弹幕到鼠标的单位向量 
                    * MathHelper.Min(Vector2.Distance(Projectile.Center, Main.MouseWorld), 15f);
                //每帧给予本弹幕一个朝向鼠标的15f每帧的速度,但是由于靠近鼠标时再以这么大的速度走会不停抽搐，
                //所以需要用min(两者取最小)方法把速率因子压进距离
                player.itemAnimation = player.itemTime = 10; //固定玩家使用时间，这样松开鼠标10帧之后玩家使用完毕
                Projectile.timeLeft = 182;//固定弹幕消散倒计时，这样松开鼠标后弹幕会再运行180帧也就是3秒

                //下面是使得玩家的法杖方向对着弹幕的效果，由于tr奇妙的手方向判定，我们必须要分情况讨论
                if (player.direction == 1)//如果玩家朝着右边
                {
                    player.itemRotation = (Projectile.Center - player.Center).ToRotation();//获取玩家到弹幕向量的方向
                }
                else
                {
                    player.itemRotation = (Projectile.Center - player.Center).ToRotation() + 3.1415926f;//反之需要+半圈
                }
            }
            else
            {
                Projectile.ai[0] = 1;//如果玩家松手了，那就改掉ai0不让他继续执行被按住的AI，转而加速直线运动下去
                if (Projectile.velocity.Length() < 1) Projectile.velocity = (Projectile.Center - player.Center).SafeNormalize(
                    Vector2.Zero) * 10;//如果弹幕速度太小，那么就让它直接发射出去好了
                Projectile.velocity *= 1.055f;//不断加速
            }
            //以上是手持弹幕的核心,下面是视觉效果的AI,使用for循环让他反复生成粒子
            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch,
                    0, 0, 0, default, 2);
                d.noGravity = true;//禁用粒子重力
                                   //为了让弹幕动起来更好看，生成一些粒子是必要的,那么再来一点紫色粒子吧
                d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch,
                    0, 0, 0, default, 2);
                d.noGravity = true;//禁用粒子重力
                                   //放心，生成粒子的方法会在第一个参数和第二第三个参数组成的一个矩形中随机位置生成粒子，不会重叠的
            }
            base.AI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //为了让弹幕具有打击感，我们还要在击中函数中写一次生成爆炸的弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromAI()//生成源一般不知道填什么的时候就这么写，反正没用
                , Projectile.Center, Vector2.Zero,//因为爆炸弹幕是静止的所以速度为0
                ProjectileID.DaybreakExplosion,//借用一下破晓的爆炸效果
                0, //伤害为0因为我们不想让他造成伤害
                0, player.whoAmI);

        }
    }
    public class MyGunProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";//因为是伪武器，就直接用透明贴图好了
        //这行可以将弹幕的材质引用指向原版对应路径的贴图，非常好用，这样就不用准备图片了
        //材质路径格式：Terraria/Images/Item_ 等等，具体请参考解包出来的贴图包名称
        Player player => Main.player[Projectile.owner];
        //定义生成弹幕时传入的owner参数对应的玩家
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽为两格物块长度
            //注意细长形弹幕千万不能照葫芦画瓢把长宽按贴图设置因为碰撞箱是固定的，不会随着贴图的旋转而旋转
            Projectile.friendly = true;//友方弹幕                                          
            Projectile.tileCollide = false;//false就能让他穿墙
            Projectile.timeLeft = 20;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.DamageType = DamageClass.Ranged;//远程
            Projectile.penetrate = 1;//表示能穿透几次
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        public override bool? CanDamage()//注意！因为这是一个只用来发射子弹的武器弹幕，不能造成伤害
        {
            return false;//因此需要返回false
        }
        public override void AI()
        {
            if (player.channel)//玩家按住时执行
            {
                //因为这是一把枪，我希望让玩家总是面朝枪指着的半边，所以写如下代码
                player.direction = (Main.MouseWorld - player.Center).X > 0 ? 1 : -1;//这样就能实现了       
                //下面是使得玩家的武器方向对着弹幕的效果
                if (player.direction == 1)//如果玩家朝着右边
                {
                    player.itemRotation = (Main.MouseWorld - player.Center).ToRotation();//获取玩家到弹幕向量的方向
                }
                else
                {
                    player.itemRotation = (Main.MouseWorld - player.Center).ToRotation() + 3.1415926f;//反之需要+半圈
                }

                Projectile.timeLeft = 2;//因为这是以弹幕作为武器，所以松手后必须马上消失!
                player.itemTime = player.itemAnimation = 20;//同样的我们需要让玩家保持使用状态
                //以下是武器主要行为
                //伪武器弹幕需要时刻定在玩家身上，所以
                Projectile.Center = player.Center;
                //“暖机”类型的武器（如幻影弓）攻击强度随着时间增加，因此需要介绍计时器,这里我们采用ai[0]
                Projectile.ai[0]++;//首先让ai0每帧加一
                int Jiange;//我们定义一个整数局部变量，用它作为间隔使用
                if (Projectile.ai[0] < 120)//前两秒内的攻击间隔
                {
                    Jiange = 50;//攻击间隔为50
                }
                else if (Projectile.ai[0] < 210)//第2~3.5秒的攻击间隔
                {
                    Jiange = 36;//攻击间隔为36
                }
                else if (Projectile.ai[0] < 270)//第3.5~4.5秒的攻击间隔
                {
                    Jiange = 22;//攻击间隔为22
                }
                else //再之后的攻击间隔
                {
                    Jiange = 10;//攻击间隔为10
                }
                //局部变量做好了，接下来就是实现每隔这么多间隔发射一次弹幕了,咱们用ai[1]作为计时
                Projectile.ai[1]++;
                if (Projectile.ai[1] > Jiange)
                {
                    Projectile.ai[1] = 0;//超过间隔就重置
                    player.PickAmmo(player.HeldItem, out int type, out float speed, out int damage, out float knockback,
                        out int ammo);//这段非常重要，是获取玩家武器发射子弹种类以及伤害速度等必备属性的方法
                    //下面是利用循环和随机数发射不规则散弹的教程
                    for (int i = 0; i < 6; i++)//循环执行6次
                    {
                        float r = Main.rand.NextFloat(-0.33f, 0.33f);//一个随机偏移量
                        //发射弹幕
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                 (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(r) * speed,//向鼠标发射再乘以速度
                 type,//获取出pickammo的out出来的type，便可以得到消耗子弹对应的弹幕
                  damage,//同理
                  knockback,//同理
                  player.whoAmI//owner为玩家索引
                  );
                    }
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item36, player.Center);//别忘记播放音效！
                }
            }

            base.AI();
        }
    }
}