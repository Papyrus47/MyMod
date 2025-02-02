
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
    public class TempleProj : ModProjectile //���Ǹ��ӵ���Ļ
    {
        public override void SetStaticDefaults()//������ÿ�μ���ģ��ʱִ��һ�Σ����ڷ��侲̬����
        {

            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;//����Ϊ4����Ϊ�ӵ���ײ�価����ҪС��
            //ע��ϸ���ε�Ļǧ�����պ�«��ư�ѳ�����ͼ���� ��Ϊ��ײ���ǹ̶��ģ�����������ͼ����ת����ת
            Projectile.friendly = true;//�ѷ���Ļ                                          
            Projectile.tileCollide = false;//false����������ǽ
            Projectile.timeLeft = 300;//��ɢʱ��
            Projectile.aiStyle = -1;//��ʹ��ԭ��AI
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;//��ʾ�ܴ�͸����
            Projectile.ignoreWater = true;//����Һ��
            Projectile.extraUpdates = 2;//ÿ֡�����2�Σ������ٶȿ��Է�2����ͬʱ��������ϡ��,Ҳ���Ա�֤���پ���NPC����ǽ��ʱ���ж�
            base.SetDefaults();
        }
        public override void AI()
        {
            //�����Ļ������һЩ������Ӫ��β����
            var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Torch, 0, 0, 0, default, 2);//����һ������,��Ϊd
            d.noGravity = true;//����������
            int index = Projectile.FindTargetWithLineOfSight(1000);//Ѱ��1000��Χ���������
            if (index >= 0)//����Ҳ�����index����-1
            {
                NPC npc = Main.npc[index];//�������NPC
                Projectile.velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10f;//��30����ÿ֡���ٶȽ���׷��(��Ϊ�������)
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)//����NPCʱ����һЩЧ��
        {
            if (Main.rand.NextBool(25))//25��֮һ�ļ���
            {
                int strike = (int)(target.lifeMax * 0.01f);//Ŀ���������1%�Ķ����˺�
                NPC.HitInfo hitInfo = new NPC.HitInfo();
                hitInfo.SourceDamage = strike;
                target.StrikeNPC(hitInfo);//����˺�
                //StrikeNPC������һ�����Զ�NPC���һ���˺��ķ���������Ϊ�˺������ˡ����˷��򡢱������Ƿ��������Ч�����Ƿ�������������
                //�������ܸ����DPS
            }
            //��������ӵ������ڻ���NPCʱ��4%������Ѫ1%
            //NPC(target, damage, knockback, crit);
        }
        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            //������ƶ����ˣ������ˡ�
            return false;//return false��ֹ�Զ�����
        }
    }

}