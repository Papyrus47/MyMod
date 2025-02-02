
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
    public class BossDeathRay : ModProjectile //�жԼ��ⵯĻ
    {
        float LaserLength = 0;//�趨һ�������ֶ�
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
            LaserLength = 2500;//2500�ĳ���
            Projectile.width = Projectile.height = 32;//��������ν��������Ҫ��д��ײ����
            //ע��ϸ���ε�Ļǧ�����պ�«��ư�ѳ�����ͼ������Ϊ��ײ���ǹ̶��ģ�����������ͼ����ת����ת
            Projectile.friendly = false;//�ѷ���Ļ
            Projectile.hostile = true;//�жԵ�Ļ
            Projectile.tileCollide = false;//false����������ǽ,�����ǲ���ǽ����Ҳ��Ҫ���ò���ǽ
            Projectile.timeLeft = 120;//��ɢʱ��
            Projectile.aiStyle = -1;//��ʹ��ԭ��AI
            Projectile.penetrate = -1;//��ʾ�ܴ�͸���ι��-1��������
            Projectile.ignoreWater = true;//����Һ��
            base.SetDefaults();
        }
        public override bool ShouldUpdatePosition()//���������Ļ���ٶ��Ƿ��������λ�ñ仯
        {
            return false;
            //ע�⣬�����൯ĻҪ����false,�ٶ�ֻ���������輤�ⷽ��ͻ������ģ�Ҫ�޸�λ����ֱ�Ӷ�center
        }
        public override void AI()//����AI��Ҫ�ǿ��Ʒ����Դ��λ��
        {
            //��һ����Ϊ���Ӿ�Ч�����õ�AI,localai0�����������Ƽ�����
            if (Projectile.localAI[0] < 15 && Projectile.timeLeft > 16)//��Ļ����ʱ����
                Projectile.localAI[0]++;
            if (Projectile.timeLeft < 16) Projectile.localAI[0]--;//��Ļ��Ҫ��ʧʱ����

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float randomLength = Main.rand.NextFloat(36, 1500);//��36~1500��һ��������������ӣ�Ӫ��Ч��
            Dust.NewDust(Projectile.Center + direction * randomLength, 0, 0, DustID.Phantasmal);


        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.localAI[0] < 15) return false;//���ⲻ����ʱ���ж�
            int Length = (int)LaserLength;//���弤�ⳤ��
            //����������ڿ��Ƶ�Ļ��ײ�жϣ����������ײ����ʱ�����漴��
            float point = 0f;//����ճ�����
            Vector2 startPoint = Projectile.Center;
            Vector2 endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Length;
            //�������ڵ�Ļ�ٶȷ����Ͼ���1500���ش���λ��
            bool K =
                Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), //�Է���ײ���λ��
                targetHitbox.Size(),//�Է���ײ��Ĵ�С 
                startPoint,//������ײ����ʼ�� 
                endPoint,//������
                50//�ߵĿ��
                , ref point);
            if (K) return true;//������������ײ�жϣ������棬Ҳ���ǽ�����ײ�˺�
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            int Length = (int)LaserLength;//���弤�ⳤ��
            //��ɫ������ͼƬ�������Aֵ����0����������Additiveģʽ�Ļ����������Ǻ�ɫ��Ч���ܲ�
            //�������Ǽ򵥵��ӳ�����
            Color color1 = Color.White;//��ɫ���ƾ���ͼƬԭɫ
            color1.A = 0;//A����0ʹ��ͼƬ��ɫ��Ϊ����,����ȥ����ɫ����
            //�����Ǽ���ͷ���Ļ���
            Texture2D head = ModContent.Request<Texture2D>("MyMod/ModProj/DeathRayHead").Value;//��ȡͷ������
            Main.EntitySpriteDraw(head, Projectile.Center - Main.screenPosition, null,//����Ҫѡ��
            color1,//�޸ĺ����ɫ
            Projectile.velocity.ToRotation(),//��ͼƬ����Ϊ��Ļ�ٶȷ���
            new Vector2(0, head.Height / 2),//�ο�ԭ��ѡ��ͼƬ����е�
            new Vector2(1, Projectile.localAI[0] / 25f),//Ϊʹ�ü��������Ȼ������������
            SpriteEffects.None, 0);
            //�����Ǽ�������Ļ���
            Texture2D tex = TextureAssets.Projectile[Type].Value;//��ȡ���ʣ����Ǽ����в�
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition
                + Projectile.velocity.SafeNormalize(Vector2.Zero) * head.Width,//����ͷ�����棬���Լ���ͷ�����ȵķ�������
                new Rectangle(0, 0, Length, tex.Height),//�ڸ߶Ȳ���Ļ����ϣ�X���ӳ���length
                color1,//�޸ĺ����ɫ
                Projectile.velocity.ToRotation(),//��ͼƬ����Ϊ��Ļ�ٶȷ���
                new Vector2(0, tex.Height / 2),//�ο�ԭ��ѡ��ͼƬ����е�
                new Vector2(1, Projectile.localAI[0] / 25f),//Ϊʹ�ü��������Ȼ������������
                SpriteEffects.None, 0);
            //�����Ǽ���β���Ļ���
            Texture2D Tail = ModContent.Request<Texture2D>("MyMod/ModProj/DeathRayTail").Value;//��ȡͷ������
            Main.EntitySpriteDraw(Tail, Projectile.Center - Main.screenPosition
             + Projectile.velocity.SafeNormalize(Vector2.Zero) * (head.Width + Length),//��������ĩ�˵ĺ���
            null,//����Ҫѡ��
            color1,//�޸ĺ����ɫ
            Projectile.velocity.ToRotation(),//��ͼƬ����Ϊ��Ļ�ٶȷ���
            new Vector2(0, Tail.Height / 2),//�ο�ԭ��ѡ��ͼƬ����е�
           new Vector2(1, Projectile.localAI[0] / 15f),//Ϊʹ�ü��������Ȼ������������    
            SpriteEffects.None, 0);
            return false;//return false��ֹ�Զ�����
        }
    }

}