
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MyMod.Content.ModProj.BossProjectile
{
    public class RoundLight2 : ModProjectile //一种圆球状视觉效果弹幕(收缩)
    {
        public override void SetStaticDefaults()//本函数每次加载模组时执行一次，用于分配静态属性
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//长宽无所谓
            Projectile.friendly = false;
            Projectile.tileCollide = false;//false就能让他穿墙,就算是不穿墙激光也不要设置不穿墙
            Projectile.timeLeft = 600;//消散时间
            Projectile.aiStyle = -1;//不使用原版AI
            Projectile.ignoreWater = true;//无视液体
            base.SetDefaults();
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            //我们使用ai0存储NPC的索引，使用ai1控制收缩的速度(以1为收缩完成)
            if (Projectile.localAI[0] < 1)
            {
                Projectile.timeLeft = 2;//锁住弹幕倒计时
                Projectile.localAI[0] += 1 / Projectile.ai[1];//一个不断增加的计时器，增加到1所需要的时间取决于你填写的ai[1]
            }
            else
            {
                Projectile.active = false;//增长到1以上就可以消失了
            }
            if (npc != null && npc.active)//判断NPC是不是存在
            {
                Projectile.Center = npc.Center;//锁在NPC身上
            }
            //不使用scale是怕出现其他模组修改弹幕大小的情况
        }

        public override bool PreDraw(ref Color lightColor)//predraw返回false即可禁用原版绘制
        {
            var tex = TextureAssets.Projectile[Type].Value;
            Color red = new Color(255, 0, 0, 0);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, red, 0, tex.Size() / 2,
                5 * (1 - Projectile.localAI[0]), SpriteEffects.None, 0);

            return false;//return false阻止自动绘制
        }
    }

}