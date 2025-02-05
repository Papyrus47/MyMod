using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using MyMod.Content.ModProj;

namespace MyMod.Content.Items.Weapons
{
    /// <summary>
    /// 由小天使制作的技能表物品
    /// </summary>
    public class MySkillsItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.width = 44;
            Item.height = 44;
            Item.maxStack = 1;
            Item.damage = 50;
            Item.scale = 2f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(10);
            Item.useTurn = false; //禁止使用转向
            Item.autoReuse = false; // 禁止自动重用
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.knockBack = 4;
            //Item.shoot = ProjectileID.PurificationPowder;
            //Item.shootSpeed = 16f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            // 成长物品
            if (NPC.downedSlimeKing) // 击败史莱姆王
            {
                damage.Base++; // 基础面板+1
                damage += 0.1f; // 攻击力+10%
            }
            if (NPC.downedBoss1) // 击败克眼
            {
                damage.Base++; // 基础面板+1
                damage += 0.5f; // 攻击力+50%
            }
        }
        public override void HoldItem(Player player)
        {
            // 这里是手持时候的效果
            // 我们需要在这里生成弹幕
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MySkillsProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position, Vector2.Zero, ModContent.ProjectileType<MySkillsProj>(), player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
        }
    }
}
