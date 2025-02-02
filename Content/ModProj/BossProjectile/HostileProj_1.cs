
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
    public class HostileProj_1 : ModProjectile // 
    {
        public override string Texture => "MyMod/TuPian/Trail1";//�����һ��ֱ�����ñ�ģ���ڵ�ͼƬ
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;//��һ�ֵ2���Լ�¼�˶��켣�ͷ�������������β��
            ProjectileID.Sets.TrailCacheLength[Type] = 10;//��һ������¼�Ĺ켣�����׷�ݵ�����֡��ǰ(ע�����ֵȡ����)
            // DisplayName.SetDefault("ħ��II");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeFire);//��¡��ӦID�ĵ�Ļ����
            Projectile.aiStyle = -1;//���ǲ�ʹ��ԭ�浯Ļ����Ϊ
            Projectile.timeLeft = 360;//��Ļ����6��
            Projectile.width = Projectile.height = 16;//
        }
        public override void AI()
        {
            Projectile.velocity *= 1.022f;//���ϼ���
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            //ͬʱ����Ҫ���еĻ�����������д�ͺ�

            Texture2D texture = TextureAssets.Projectile[Type].Value;//��������Ļ�Ĳ���
            //Ҫ������β������Ҫ����һ��forѭ����䣬��0һֱ�ߵ��켣ĩ��
            //�������ǽ���һ���ܲ����������ӻ��Ƶİ취��A=0��
            Color MyColor = Main.DiscoColor; MyColor.A = 0;//��A=0��Ϊ����ֱ�ӵ�����ɫ
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)//ѭ������С�ڹ켣����
            {
                float factor = 1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                //����һ�����µ�����1�𽥼��ٵ�0�ı���������i = 0ʱ��factor = 1
                Vector2 oldcenter = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                //���ڹ켣ֻ�ܼ�¼��Ļ��ײ�����Ͻ�λ�ã�����Ҫ�ֶ����ϵ�Ļ���һ������ȡ����
                Main.EntitySpriteDraw(texture, oldcenter, null, MyColor * factor,//��ɫ�𽥱䵭
                    Projectile.oldRot[i] - 1.57f,//��Ļ�켣�ϵ������ķ���
                    new Vector2(texture.Width / 2, texture.Height / 2 / Main.projFrames[Type]),
                     new Vector2(1, 1) * factor,//��С�𽥱�С
                    SpriteEffects.None, 0);
            }
            //����tr��������ִ�е��Ȼ��ƣ�����Ҫ���Ӱ�����ǵ��������棬��Ҫ��д��Ӱ����

            Main.EntitySpriteDraw(  //entityspritedraw�ǵ�Ļ��NPC�ȳ��õĻ��Ʒ���
                texture,//��һ�������ǲ���
                Projectile.Center - Main.screenPosition,//ע�⣬����ʱ��λ��������Ļ���Ͻ�Ϊ0��
                                                        //���Ҫ�õ�Ļ���������ȥ��Ļ���Ͻǵ�����
                null,//��������������֡ͼѡ����
                MyColor,//���ĸ���������ɫ�������������Դ���lightcolor�������ܵ���Ȼ����Ӱ��
                Projectile.rotation - 1.57f,//�������������ͼ��ת����
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