
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

namespace MyMod.Content.ModProj.BossProjectile
{
    public class HostileProj_2 : ModProjectile // 
    {
        public override string Texture => "MyMod/TuPian/LaserProj";//�����һ��ֱ�����ñ�ģ���ڵ�ͼƬ
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;//��һ�ֵ2���Լ�¼�˶��켣�ͷ�������������β��
            ProjectileID.Sets.TrailCacheLength[Type] = 10;//��һ������¼�Ĺ켣�����׷�ݵ�����֡��ǰ(ע�����ֵȡ����)
            // DisplayName.SetDefault("ħ��III");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()//��NewProjectileʱ�����Ը�ֵ��ֱ�Ӹ��ǵ�setdef��Ķ����Եĸ�ֵ
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire);//��¡��ӦID�ĵ�Ļ����
            Projectile.aiStyle = -1;//���ǲ�ʹ��ԭ�浯Ļ����Ϊ
            Projectile.timeLeft = 360;//��Ļ����6��
            Projectile.width = Projectile.height = 4;//�����ӵ���Ļ����ײ��Сһ�����

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            //ͬʱ����Ҫ���еĻ�����������д�ͺ�
            Color MyColor = Color.Red;
            MyColor.A = 0;
            Texture2D texture = TextureAssets.Projectile[Type].Value;//��������Ļ�Ĳ���
            Main.EntitySpriteDraw(  //entityspritedraw�ǵ�Ļ��NPC�ȳ��õĻ��Ʒ���
                texture,//��һ�������ǲ���
                Projectile.Center - Main.screenPosition,//ע�⣬����ʱ��λ��������Ļ���Ͻ�Ϊ0��
                                                        //���Ҫ�õ�Ļ���������ȥ��Ļ���Ͻǵ�����
                null,//��������������֡ͼѡ����
                MyColor,//���ĸ���������ɫ�������������Դ���lightcolor�������ܵ���Ȼ����Ӱ��
                Projectile.rotation,//�������������ͼ��ת����
                new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                //��������������ͼ����ԭ������꣬����дΪ��ͼ��֡���������꣬������ת�����Ŷ���Χ������
                new Vector2(1, 1),//���߸����������ţ�X��ˮƽ���ʣ�Y����ֱ����
                SpriteEffects.None,
                0//�ھŸ������ǻ��Ʋ㼶������0�����ˣ���̫��ʹ
                );

            return false;//return false��ֹ�Զ�����
        }
    }

}