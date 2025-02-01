
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

namespace MyMod.ModProj.BossProjectile
{
    public class TutorialFlame : ModProjectile // ����ħ���۵����Ч��
    {
        public override string Texture => "Terraria/Images/Item_0";//0��͸����ͼ
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("�̳�֮��");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire);//��¡��ӦID�ĵ�Ļ����
            Projectile.aiStyle = -1;//���ǲ�ʹ��ԭ�浯Ļ����Ϊ
            Projectile.timeLeft = 60;//��Ļ����һ��
            Projectile.width = Projectile.height = 48;//������ײ��
            Projectile.extraUpdates = 4;//ÿ֡����5��
        }
        public override void AI()
        {
            int random = Main.rand.Next(3);//����һ��0��1��2������֮һ���������
            
            switch (random)
            {
                case 0:
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 0, default, 3f);
                    d.noGravity = true; d.velocity = Main.rand.NextVector2Circular(5, 5);d.velocity += Projectile.velocity;
                    break;
                case 1:
                    d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0, 0, 0, default,3);
                    d.noGravity = true; d.velocity = Main.rand.NextVector2Circular(5, 5); d.velocity += Projectile.velocity;
                    break;
                case 2:
                    d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, 0, default, 3);
                    d.noGravity = true; d.velocity = Main.rand.NextVector2Circular(5, 5); d.velocity += Projectile.velocity;
                    break;
                //�������ǾͿ���������ɻ��� ��Ӱ�� ˪�� ������
            }
        

        }
        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            //�����ӵĵ�Ļ�������ͼƬ
            return false;//return false��ֹ�Զ�����
        }
    }
   
}