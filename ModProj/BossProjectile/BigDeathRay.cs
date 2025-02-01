
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
    public class BigDeathRay : ModProjectile //βɱ�󼤹�
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
            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeTimer = 2;
            Main.LocalPlayer.GetModPlayer<ScreenMovePlayer>().ScreenShakeScale = 10;
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if(npc.active && npc.type == ModContent.NPCType<NPCs.PrimaryBoss>() && npc.ai[0] < 950)
            {
                if (Projectile.timeLeft < 17)Projectile.timeLeft = 17;//��֤NPC��ά�ּ��ⷢ��״̬ʱ����Ļ����ʱά���ڽ�����ʧ������ǰ1֡
                Projectile.velocity = npc.rotation.ToRotationVector2();//�õ�Ļ���ٶȷ��������NPC
                //��������
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                float randomLength = Main.rand.NextFloat(36, 1500);//��36~1500��һ��������������ӣ�Ӫ��Ч��
                var d = Dust.NewDustDirect(Projectile.Center + direction * randomLength, 0, 0, DustID.SolarFlare, 0, 0, 0,Color.Red,3f);
                d.noGravity = true;
            }
            //��һ����Ϊ���Ӿ�Ч�����õ�AI,localai0�����������Ƽ�����
            if(Projectile.localAI[0] < 15 && Projectile.timeLeft > 16)//��Ļ����ʱ����
            Projectile.localAI[0]++;
            if (Projectile.timeLeft < 16) Projectile.localAI[0]--;//��Ļ��Ҫ��ʧʱ����
            
            
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
                150//�ߵĿ��
                , ref point);
            if (K) return true;//������������ײ�жϣ������棬Ҳ���ǽ�����ײ�˺�
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            float Multiply = 150f / 36f;//��������Ҫ�Ŀ��(150)���Լ���ͼƬ���(36)�õ���ԭͼ�Ŵ󵽹涨�������Ҫ�ı���
            int Length = (int)LaserLength;//���弤�ⳤ��
            //��ɫ������ͼƬ�������Aֵ����0����������Additiveģʽ�Ļ����������Ǻ�ɫ��Ч���ܲ�
            //�������Ǽ򵥵��ӳ�����
            //�����Ǽ���ͷ���Ļ���
            Texture2D head = ModContent.Request<Texture2D>("MyMod/ModProj/BossProjectile/BigDeathRay_Head").Value;//��ȡͷ������
            Main.EntitySpriteDraw(head, Projectile.Center - Main.screenPosition,null,//����Ҫѡ��
            Color.White,
            Projectile.velocity.ToRotation(),//��ͼƬ����Ϊ��Ļ�ٶȷ���
            new Vector2(0, head.Height / 2),//�ο�ԭ��ѡ��ͼƬ����е�
            new Vector2(Multiply,Multiply * Projectile.localAI[0]/25f),//Ϊʹ�ü��������Ȼ������������
            SpriteEffects.None, 0);

            //�����Ǽ�������Ļ���
            Texture2D tex = TextureAssets.Projectile[Type].Value;//��ȡ���ʣ����Ǽ����в�
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition 
                + Projectile.velocity.SafeNormalize(Vector2.Zero) * head.Width * Multiply,//����ͷ�����棬���Լ���ͷ�����ȵķ�������
                new Rectangle(0, 0, Length, tex.Height),//�ڸ߶Ȳ���Ļ����ϣ�X���ӳ���length
                Color.White,
                Projectile.velocity.ToRotation(),//��ͼƬ����Ϊ��Ļ�ٶȷ���
                new Vector2(0, tex.Height / 2),//�ο�ԭ��ѡ��ͼƬ����е�
                new Vector2(1, Projectile.localAI[0] / 25f * Multiply),//Ϊʹ�ü��������Ȼ������������
                SpriteEffects.None, 0);
            return false;//return false��ֹ�Զ�����
        }
    }
   
}