
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

namespace MyMod.ModProj.BossProjectile
{
    public class RoundLight : ModProjectile //һ��Բ��״�Ӿ�Ч����Ļ
    {   
        public override void SetStaticDefaults()//������ÿ�μ���ģ��ʱִ��һ�Σ����ڷ��侲̬����
        {
            Main.projFrames[Type] = 1;//���֡ͼ�ж���֡�������
          //  ProjectileID.Sets.TrailingMode[Type] = 2;//��һ�ֵ2���Լ�¼�˶��켣�ͷ�������������β��
          //  ProjectileID.Sets.TrailCacheLength[Type] = 10;//��һ������¼�Ĺ켣�����׷�ݵ�����֡��ǰ(ע�����ֵȡ����)
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//��һ�����Ļ������Ļ����پ������ڿ��Ի���
                                                                //���ڳ����ε�Ļ����
                                                                //���ⵯĻ����4000����
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;//��������ν
            Projectile.friendly = false;
            Projectile.tileCollide = false;//false����������ǽ,�����ǲ���ǽ����Ҳ��Ҫ���ò���ǽ
            Projectile.timeLeft = 60;//��ɢʱ��
            Projectile.aiStyle = -1;//��ʹ��ԭ��AI
            Projectile.ignoreWater = true;//����Һ��
            base.SetDefaults();
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.2f;//һ���������ӵļ�ʱ�������Կ��ƹ�Ч��С

            //��ʹ��scale���³�������ģ���޸ĵ�Ļ��С�����
        }

        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            var tex = TextureAssets.Projectile[Type].Value;
            Color red = new Color(255, 0, 0, 0);//������A=0�İ취ȥ���ڵ�
            Color white = new Color(255, 255, 255, 0);//������A=0�İ취ȥ���ڵ�
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, red, 0, tex.Size() / 2, 
                Projectile.ai[0], SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null,white, 0, tex.Size() / 2,
               Projectile.ai[0], SpriteEffects.None, 0);
            //Ϊʲô��Ҫ����һ�κ��һ�ΰ׵ģ���Ϊ��A=0������£���ɫ�ǿ��Ե��ӵģ�ͬʱ����һ�κ��һ�ΰ׿�������ǿ��Ч�ĸо�
            //
          
            return false;//return false��ֹ�Զ�����
        }
    }
   
}