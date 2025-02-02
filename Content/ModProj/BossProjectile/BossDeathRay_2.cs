
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
    public class BossDeathRay_2 : ModProjectile //�ж�˫���򼤹ⵯĻ(����Ҫͷβ��)
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
            Dust.NewDust(Projectile.Center + direction * randomLength, 0, 0, DustID.RedTorch, 0, 0, 0, default, 3);

            if (Projectile.timeLeft == 114)//������ֺ��䴹ֱ��һ�ŵ�Ļ
            {

                for (int i = -10; i <= 10; i++)//��Ƕ��ѭ�������
                {
                    //��һ��ѭ��������Ļ�ڼ����ϵ�λ��
                    float inter = 150;//ÿ����λ�õļ��
                    Vector2 p = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * i * inter;
                    for (int j = -1; j <= 1; j += 2)
                    {
                        float rotation = j * MathHelper.Pi / 2 + Projectile.velocity.ToRotation();//Ҫô˳ʱ��ƫ��90�ȣ�Ҫô��ʱ��ƫ��90�ȣ������������෴����
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), p, rotation.ToRotationVector2() * 0.09f,
                            ModContent.ProjectileType<HostileProj_1>(), 19, 1);
                    }
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.localAI[0] < 15) return false;//���ⲻ����ʱ���ж�
            int Length = (int)LaserLength;//���弤�ⳤ��
            //����������ڿ��Ƶ�Ļ��ײ�жϣ����������ײ����ʱ�����漴��
            float point = 0f;//����ճ�����
            Vector2 startPoint = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * Length;//˫����ļ���,������ڷ�����Զ��
            Vector2 endPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Length;//˫����ļ��⣬�յ���������Զ��
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
            var tex = TextureAssets.Projectile[Type].Value;
            Color colorA = new Color(255, 255, 255, 0);//
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, colorA, Projectile.velocity.ToRotation(),
                tex.Size() / 2, new Vector2(50, Projectile.localAI[0] / 30f), SpriteEffects.None, 0);
            return false;//return false��ֹ�Զ�����
        }
    }

}