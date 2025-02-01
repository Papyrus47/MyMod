
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
using MyMod.NPCs;
using Terraria.Audio;

namespace MyMod.ModProj.BossProjectile
{
    public class WarningLine : ModProjectile // Ԥ���ߵ�ʾ��
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("�̳�Ԥ����");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//��һ�����Ļ������Ļ����پ������ڿ��Ի���
                                                                                                 //���ڳ����ε�Ļ����
                                                                                                 //���ⵯĻ����4000����
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false;Projectile.ignoreWater = true;//��ǽ���Ҳ���ˮӰ��
            Projectile.width = Projectile.height = 4;//װ���Ե�Ļ������д��Сһ�����
            Projectile.timeLeft = 40;//Ԥ���߳���40֡����BOSS���������ʱ��
        }
        NPC npc => Main.npc[(int)Projectile.ai[0]];//���嵯Ļ�����NPC����ָ���NPC����
        public override void AI()
        {
            if(npc.active && npc.type == ModContent.NPCType<PrimaryBoss>())//ȷ��NPC�ǲ������ǵ��Ǹ�BOSS
            {
                Projectile.Center = npc.Center;//�̶���NPC����
                Projectile.rotation = npc.rotation ;//���򽥱䵽��NPC��ͬ��Ӫ��һ�ֽ����
            }
            else
            {
                Projectile.active = false;//������������������������ֱ����ȥ�����õ�Ļ��
            }
        }
        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            float n = Projectile.timeLeft / 40f;//��ʣ��ʱ���һ�����ڳ�����ɫ
            //Ҫ�뽫�Ҷ�ͼ�ĺڵ�ȥ�����������Բ�����ɫAֵ��0�ķ�����Ҳ����ֱ�ӿ���������ģʽ,�������£������ճ�
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.Additive,//�ص��ǵڶ������������Additive����ͼƬ��RGBֱֵ�Ӽ��㵽��Ļ��
                SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, 
                Main.GameViewMatrix.TransformationMatrix);
            Texture2D line = TextureAssets.Projectile[Type].Value;//��ȡ����Ļ����
            Main.EntitySpriteDraw(line, Projectile.Center - Main.screenPosition, null,
                Color.Red * n,//����һ�ֽ���������Ч��
                Projectile.rotation,
                new Vector2(0, line.Height / 2),//������������������ԭ��ѡ������е�
                new Vector2(100, 1),//X��������100��
                SpriteEffects.None, 0);
            //�����˰ѻ���ģʽŪ����
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,//Alphablend��Ĭ�ϵĻ��ģʽ
                SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null,
                Main.GameViewMatrix.TransformationMatrix);

            return false;//return false��ֹ�Զ�����
        }
    }
    public class WarningLine_2 : ModProjectile // �����Ԥ����
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("�̳�Ԥ����");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//��һ�����Ļ������Ļ����پ������ڿ��Ի���
                                                                                                 //���ڳ����ε�Ļ����
                                                                                                 //���ⵯĻ����4000����
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false; Projectile.ignoreWater = true;//��ǽ���Ҳ���ˮӰ��
            Projectile.width = Projectile.height = 4;//װ���Ե�Ļ������д��Сһ�����
            Projectile.timeLeft = 40;//Ԥ���߳���40֡����BOSS���������ʱ��
        }
        public override bool ShouldUpdatePosition()
        {
            return false;//��ֹ�ٶ�Ӱ�쵯Ļλ��
        }
        public override void AI()
        {
            //�����Ļû����Ϊ
        }
        public override void Kill(int timeLeft)//��Ļ����ʱ����һ������
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                Projectile.velocity, ModContent.ProjectileType<BossDeathRay>(), 25, 1, Main.myPlayer);
            base.Kill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            //���������һ���򵥵��Ԥ���ߣ�Ҳ���Ǻ��ɫ��������
            float factor = (1 + (float)Math.Sin(Projectile.timeLeft / 3f));//����һ��0~2���˰ڶ������Һ���
            factor /= 2f;//���Զ���Ҳ���ǹ�һ��

            Color color1 = Color.Lerp(Color.Red, Color.Yellow, factor);
            //lerp�����Բ�ֵ�����Ը��ݵ������մ���ı���(0~1)��ѡ���һ���͵ڶ�������֮��ָ�����ʵ��м�ֵ

            var tex = TextureAssets.MagicPixel.Value;//���������һ����ɫ����
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition,
                new Rectangle(0, 0, 2, 2),//����ѡȡ2x2�Ĵ�С
                color1, Projectile.velocity.ToRotation()//���ٶȷ������Ԥ���߷���
                , Vector2.Zero, new Vector2(1500, 1)//X������1500��
                , SpriteEffects.None, 0);
            return false;//return false��ֹ�Զ�����
        }
    }
    public class WarningLine_3 : ModProjectile // Ԥ���ߵ�ʾ��(��BOSS����Ԥ�м���ʱʹ��)
    {
        public override string Texture => "Terraria/Images/Projectile_1";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("�̳�Ԥ����");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 4000;//��һ�����Ļ������Ļ����پ������ڿ��Ի���
                                                                //���ڳ����ε�Ļ����
                                                                //���ⵯĻ����4000����
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.tileCollide = false; Projectile.ignoreWater = true;//��ǽ���Ҳ���ˮӰ��
            Projectile.width = Projectile.height = 4;//װ���Ե�Ļ������д��Сһ�����
            Projectile.timeLeft = 40;//Ԥ���߳���40֡����BOSS���������ʱ��
        }
        NPC npc => Main.npc[(int)Projectile.ai[0]];//���嵯Ļ�����NPC����ָ���NPC����
        public override void AI()
        {
            if (npc.active && npc.type == ModContent.NPCType<PrimaryBoss>())//ȷ��NPC�ǲ������ǵ��Ǹ�BOSS
            {
                Projectile.Center = npc.Center;//�̶���NPC����
                Projectile.rotation = npc.rotation;//�����NPC��ͬ
            }
            else
            {
                Projectile.active = false;//������������������������ֱ����ȥ�����õ�Ļ��
            }
        }
        public override void Kill(int timeLeft)//��Ļ����ʱ����һ������
        {
            SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                Projectile.rotation.ToRotationVector2(), ModContent.ProjectileType<BossDeathRay_2>(), 25, 1, Main.myPlayer);
            base.Kill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)//predraw����false���ɽ���ԭ�����
        {
            //���������һ���򵥵��Ԥ���ߣ�Ҳ���Ǻ��ɫ��������
            float factor = (1 + (float)Math.Sin(Projectile.timeLeft / 3f));//����һ��0~2���˰ڶ������Һ���
            factor /= 2f;//���Զ���Ҳ���ǹ�һ��

            Color color1 = Color.Lerp(Color.Red, Color.Yellow, factor);
            //lerp�����Բ�ֵ�����Ը��ݵ������մ���ı���(0~1)��ѡ���һ���͵ڶ�������֮��ָ�����ʵ��м�ֵ

            var tex = TextureAssets.MagicPixel.Value;//���������һ����ɫ����
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition,
                new Rectangle(0, 0, 2, 2),//����ѡȡ2x2�Ĵ�С
                color1, Projectile.rotation
                , Vector2.Zero, new Vector2(1500, 1)//X������1500��
                , SpriteEffects.None, 0);
            return false;
        }
    }
}