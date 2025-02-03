using Terraria.ID;

namespace MyMod.Content.ModProj
{
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