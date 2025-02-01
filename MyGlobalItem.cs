using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MyMod
{
	// 这个类代表对所有的物品进行操作，你可以针对任何一个符合条件的物品独立修改
	// 这意味着你可以修改任何原版和模组物品(甚至别的模组的物品)
	public class MyGlobalItem : GlobalItem //这是一个全局物品的示例
	{
		public override bool InstancePerEntity => true;
        // 使得每个实体有自己的属性, 否则所有物品将共用你设置的属性



        //globalitem类的函数与ModItem钩子大体上相同，参数中的item参数可以用以判断这是什么物品
        //这里举几个例子,（功能是乱写的，主要告诉你global系列的应用的思路）
        public override void SetDefaults(Item item)//这是物品生成出来执行一次的函数（与ModItem同理）
        {
            //这的item参数就是给你用的，例如我这里写一个判断item符合哪些条件执行什么事
            if (item.type == ItemID.CopperShortsword)//如果这个物品是铜短剑
            {
                item.damage = 99999;//初始伤害=99999
                item.crit = 99;//暴击率为99%
                item.rare = ItemRarityID.Master;//大师稀有度
                item.master = true;//大师物品

                //注意！修改物品的DamageClass只能对物品本身的近战攻击有效，生成的弹幕不会一起改变(短剑是弹幕武器)
            }
            if(item.DamageType == DamageClass.Magic)//对于所有魔法武器
            {
                item.mana = 0;//魔法消耗量=0
            }
            if(item.consumable)//如果物品是可消耗物品
            {
                item.maxStack = 1145;//堆叠上限变为1145
            }
            base.SetDefaults(item);
        }
        public override float UseTimeMultiplier(Item item, Player player)//这里可以修改物品的使用时间
        {
            //（一般饰品，BUFF之类的修改使用速度的可以写在这。利用modplayer传入字段）
            if(item.DamageType == DamageClass.Ranged)//对于全体远程武器
            {
                if(item.useAmmo == AmmoID.Bullet)//如果物品使用的弹药是子弹
                    return base.UseTimeMultiplier(item, player) / 2f;//在原本的使用时间基础上除以2
                if(item.useAmmo == AmmoID.Arrow)//如果物品使用弹药为箭
                    return base.UseTimeMultiplier(item, player) / 4f;//在原本的使用时间基础上除以4
            }
            return base.UseTimeMultiplier(item, player);
        }
        public int UseRecord = 0;//这里我们创建一个字段，用于记录物品使用次数
        //注意，字段如果不保存，退出世界会重置。想要保存一个字段，需要利用Save和Load,将会在之后介绍
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            //本函数可以（以倍率）修改物品实际伤害
            //修改damage即可修改伤害（以1为基准,如果damage *= 1.5，那就是1.5倍原伤害）
            //例如我这里想做一个"物品每次使用都会增加1%伤害，封顶1000%的功能"下面的useitem函数里已经写好使用次数记录了
            //damage *= 1 + (UseRecord / 100f);//就这么简单
            if(item.type == ItemID.Zenith)
            {
                damage *= 0.25f;//天顶攻击力变成四分之一
            }
            base.ModifyWeaponDamage(item, player, ref damage);
        }
        public override bool CanUseItem(Item item, Player player)//这个函数会在玩家试图使用物品时执行一次
        {
            if(base.CanUseItem(item, player))//如果能使用，说明接下来会使用一次物品，也就是真的使用了一次物品
            {
                if (UseRecord < 1000)
                {
                    UseRecord++;//每次使用都加一
                }
            }
            return base.CanUseItem(item, player);//这个东西返回false会让玩家无法使用这个物品。
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)//globalitem的Shoot函数和ModItem大同小异
        {
            if(item.type == ItemID.Meowmere)//如果物品是喵刃
            {
                for(int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(6.283f/6f * i) * 2, //以两倍速度六向发射
                        type,//改为光明剑气
                        damage, knockback,player.whoAmI);

                }

                return false;//我不需要原本的发射了
            }
            if(item.type == ItemID.TerraBlade) //为泰拉之刃额外增加两条剑气
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.3f), //逆时针偏转0.3弧度发射
                      ProjectileID.LightBeam,//发射圣剑剑气
                      damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.3f), //顺时针偏转0.3弧度发射
                      ProjectileID.NightBeam,//发射永夜剑气
                      damage, knockback, player.whoAmI);
                //不反回false是因为我还需要原本的泰拉剑气
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            //这个函数是修改物品的详情页信息词条（只要是写在物品详情页的都能修改,例如伤害，击退，暴击，提示语）
            //首先介绍增加一行提示的办法。
            tooltips.Add(new TooltipLine(Mod, "Tooltip2", "使用了" + UseRecord +"次"));//新增一个记录使用次数的词条
            //new Tooltipline的第一个参数照抄，第二个参数是给新增的词条命名(一般没用),第三个参数就是你要展示的字符串
            //接下来介绍修改物品原有词条的办法
            foreach(var line in tooltips)
            {
                if(line.Mod == "Terraria")//判断这词条是不是原版的
                {
                    //接下来是重点，我们需要判断这个词条的名称，名称图鉴在下面。
                    if(line.Name == "ItemName")//例如这个就是物品名的词条
                    {
                        line.Text += " B站搜索泰拉瑞亚MOD制作";//给该词条后加上" B站搜索泰拉瑞亚MOD制作"这几个字
                    }
                    if(line.Name == "Knockback")//当然你也可以直接改掉这个词条
                    {
                        line.Text = "击退没用，不想显示了，自己猜去吧";//...
                        line.OverrideColor = Color.Red;//修改本词条的颜色
                    }
                    if(line.Name == "Speed")
                    {
                        line.Text += " 所以" + line.Text + "是多快?";//我承认我皮了
                    }
            
                }
            }//以下是Name对应的含义。（带问号说明我不确定是什么意思）
            //     • "ItemName" - 物品名称.
            //     • "Favorite" - 是否标记为收藏.
            //     • "FavoriteDesc" - 标记为收藏描述.
            //     • "Social" - 可装备在时装栏.
            //     • "SocialDesc" -  可装备在时装栏描述
            //     • "Damage" - 伤害词条
            //     • "CritChance" - 暴击词条
            //     • "Speed" - 使用速度词条
            //     • "Knockback" - 击退
            //     • "FishingPower" - 渔力
            //     • "NeedsBait" - 需要鱼饵
            //     • "BaitPower" - 饵力
            //     • "Equipable" - 可装备
            //     • "WandConsumes" - 物块消耗墙体？
            //     • "Quest" - 标记为任务鱼
            //     • "Vanity" - 标记为“时装物品”
            //     • "Defense" - 防御力
            //     • "PickPower" - 镐力
            //     • "AxePower" - 斧力
            //     • "HammerPower" - 锤力
            //     • "TileBoost" - 可挖掘范围
            //     • "HealLife" - 回复x生命值
            //     • "HealMana" - 回复x魔力
            //     • "UseMana" - 消耗的魔力
            //     • "Placeable" - 可放置
            //     • "Ammo" - 弹药
            //     • "Consumable" - 可消耗
            //     • "Material" - 材料
            //     • "Tooltip#" - 提示词条序列，例如"Tooltip0"为第一条，"Tooltip1"为第二条以此类推
            //     • "EtherianManaWarning" - 天国魔力数值
            //     • "WellFedExpert" - 专家模式食物回复?
            //     • "BuffTime" - buff效果持续时间
            //     • "OneDropLogo" - 悠悠球的logo
            //     • "PrefixDamage" - 前缀对武器伤害的加成词条
            //     • "PrefixSpeed" - 前缀对武器攻速的加成词条
            //     • "PrefixCritChance"  - 前缀对武器暴击的加成词条
            //     • "PrefixUseMana"  - 前缀对武器耗魔的加成词条
            //     • "PrefixSize"  - 前缀对武器大小的加成词条
            //     • "PrefixShootSpeed"  - 前缀对弹幕飞行速度的加成词条
            //     • "PrefixKnockback"  - 前缀对武器击退的加成词条
            //     • "PrefixAccDefense" - 饰品前缀对防御力加成的词条
            //     • "PrefixAccMaxMana" - 饰品前缀对魔力上限加成
            //     • "PrefixAccCritChance" - 饰品前缀对人物暴击率加成
            //     • "PrefixAccDamage" - 饰品前缀对伤害的加成
            //     • "PrefixAccMoveSpeed" -饰品前缀对移动速度的加成
            //     • "PrefixAccMeleeSpeed" - 饰品前缀加成近战攻速
            //     • "SetBonus" - 套装效果词条
            //     • "Expert" - 专家标记
            //     • "SpecialPrice" - 标记为特别昂贵的物品（卖钱）
            //     • "Price" - 物品价格
            base.ModifyTooltips(item, tooltips);
        }
        //以下两个函数就是存储和读取了,因为字段会在退出世界后重置，我们需要特意保存它们。
        public override void SaveData(Item item, TagCompound tag)
        {
            tag["UseRecord"] = UseRecord;//新建一个标签，赋值为我们想要存入的属性
            base.SaveData(item, tag);
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            UseRecord = tag.GetInt("UseRecord");//读取我们存入的tag，记住你存的是什么类型就用对应的Get方法。
            //例如这里存入的是int类型，那么我们就用GetInt方法读取。
            base.LoadData(item, tag);
        }
        //其他函数的功能暂时不介绍，不过你可以自行配合其他mod的源码研究
    }
    public class MyGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if(source is EntitySource_OnHit)
            {
                
            }
            base.OnSpawn(projectile, source);
        }

        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            return base.CanHitNPC(projectile, target);
        }
    }
}