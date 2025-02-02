
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;

namespace MyMod.NPCs // 命名空间咱们写文件夹路径
{
	// 这个NPC绝大多数时候与普通僵尸一样，但是会窃取（捡起地上的）ExampleItem并保存在自己身上，如果窃取的数量足够多，这个僵尸将会和物品一起保存在世界里
	public class CloneZombie : ModNPC //这是一个换皮NPC
	{
		public int StolenItems = 0;

		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("First hostile NPC");
			//lation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "第一个怪物");

			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			// 原先方法不再使用
			//NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
			//	// 影响僵尸在生物图鉴中的外观
			//	Velocity = 1f // 在生物图鉴中NPC以+1图格的速度前进（向右）
			//};
			//NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f // 在生物图鉴中NPC以+1图格的速度前进（向右）
			});
		}

		public override void SetDefaults() {
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 14;
			NPC.defense = 6;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;

			NPC.aiStyle = 3; // 3为战士AI，此处应选择与需求相匹配的内置AI
            AIType = NPCID.Zombie; // 使用僵尸ID的AI，因为一个aistyle是一个大类,aistyle相同不代表行为相同。
								   // AItype是让你精确到具体的NPC。

			AnimationType = NPCID.Zombie; // 调用僵尸的贴图帧数与处理方式(也就是说你的贴图格式一定要和僵尸保持一致)
			Banner = Item.NPCtoBanner(NPCID.Zombie); // 这个生物会受到僵尸旗帜的伤害影响
			BannerItem = Item.BannerToItem(Banner); // 击杀这个生物会为获得僵尸旗帜增加计数
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// 我们使用AddRange而不是多次调用Add来添加多个条目
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// 设置生物图鉴中显示的生成条件
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,//夜晚

				// 设置生物图鉴中的描述
				new FlavorTextBestiaryInfoElement(""),
			});
		}

		public override void AI() {
			if (Main.netMode == NetmodeID.MultiplayerClient) // 目的：为了不让NPC的AI在多人模式下的个人客户端中运行
				return;
			

			Rectangle hitbox = NPC.Hitbox;
			foreach (Item item in Main.item) {
				// 当碰到地上掉落的ExampleItem时将其捡起
				if (item.active && !item.beingGrabbed &&
				    hitbox.Intersects(item.Hitbox)) {
					item.active = false;
					StolenItems += item.stack;

					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
				}
			}
		}

		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write(StolenItems);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			StolenItems = reader.ReadInt32();
		}

		public override void OnKill() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}

			// NPC死亡时掉落全部捡起的ExampleItem
			while (StolenItems > 0) {
				// 在物品掉落前不断循环，以防止超出数组范围
				//int droppedAmount = Math.Min(ModContent.GetInstance<ExampleItem>().Item.maxStack, StolenItems);
				//StolenItems -= droppedAmount;
				//Item.NewItem(NPC.GetSource_Death(), NPC.Center, ModContent.ItemType<ExampleItem>(), droppedAmount, true);
			}
		}

	/*	public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<ExampleSurfaceBiome>()) // 只在ExampleSurfaceBiome中生成
			    && !NPC.AnyNPCs(Type)) {
				// 只会在没有别的窃贼僵尸存在时生成
				return SpawnCondition.OverworldNightMonster.Chance * 0.1f; // 普通僵尸1/10的生成几率
			}

			return 0f;
		}*/

		public override bool NeedSaving() {
			return StolenItems >= 10; // 当NPC持有超过10个物品时将会被保存，这个条件是为了防止占用太多存储空间
		}

		public override void SaveData(TagCompound tag) {
			if (StolenItems > 0) {
				// 如果别的mod之类的想存下这个NPC，他不一定持有10个以上物品
				tag["StolenItems"] = StolenItems;
			}
		}

		public override void LoadData(TagCompound tag) {
			StolenItems = tag.GetInt("StolenItems");
		}
	}
}