
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
    public class RoundLight2 : ModProjectile //һ��Բ��״�Ӿ�Ч����Ļ(����)
    {
        public override void SetStaticDefaults()//������ÿ�μ���ģ��ʱִ��һ�Σ����ڷ��侲̬����
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//��������ν
            Projectile.friendly = false;
            Projectile.tileCollide = false;//false����������ǽ,�����ǲ���ǽ����Ҳ��Ҫ���ò���ǽ
            Projectile.timeLeft = 600;//��ɢʱ��
            Projectile.aiStyle = -1;//��ʹ��ԭ��AI
            Projectile.ignoreWater = true;//����Һ��
            base.SetDefaults();
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            //����ʹ��ai0�洢NPC��������ʹ��ai1�����������ٶ�(��1Ϊ�������)
            if (Projectile.localAI[0] < 1)
            {
                Projectile.timeLeft = 2;//��ס��Ļ����ʱ
                Projectile.localAI[0] += 1 / Projectile.ai[1];//һ���������ӵļ�ʱ�������ӵ�1����Ҫ��ʱ��ȡ��������д��ai[1]
            }
            else
            {
                Projectile.active = false;//������1���ϾͿ�����ʧ��
            }
            if (npc != null && npc.active)//�ж�NPC�ǲ��Ǵ���
            {
                Projectile.Center = npc.Center;//����NPC����
            }
            //��ʹ��scale���³�������ģ���޸ĵ�Ļ��С�����
        }

        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            var tex = TextureAssets.Projectile[Type].Value;
            Color red = new Color(255, 0, 0, 0);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, red, 0, tex.Size() / 2,
                5 * (1 - Projectile.localAI[0]), SpriteEffects.None, 0);

            return false;//return false��ֹ�Զ�����
        }
    }

}