using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;//要想写能存储的东西，必引用这个

namespace MyMod
{
	public class MyWorldSystem:ModSystem //modsys是1.3版本的modworld，也就是控制世界的东西
	{
		public static bool DownedTutorialEye = false;//定义一个字段，代表是否击杀了教程之魔眼
        public static LocalizedText AnyIronBar = Language.GetOrRegister("Mods.MyMod.AnyIronBar", () => "Any Iron Bar");
        public override void AddRecipeGroups()
        {
            // 这里添加配方组
            RecipeGroup.RegisterGroup("MyMod.AnyIronBar", new(()=> AnyIronBar.Value, ItemID.TungstenBar,ItemID.PlatinumBar,ItemID.MythrilBar,ItemID.OrichalcumBar,ItemID.AdamantiteBar,ItemID.CobaltBar,ItemID.PalladiumBar,ItemID.OrichalcumBar,ItemID.MythrilBar,ItemID.TungstenBar,ItemID.PlatinumBar,ItemID.TinBar,ItemID.LeadBar,ItemID.SilverBar,ItemID.CopperBar,ItemID.IronBar));
        }
        public override void SaveWorldData(TagCompound tag)//退出世界时存入tag
        {
            tag["DownedTutorialEye"] = DownedTutorialEye;//将tag赋值为它
            base.SaveWorldData(tag);
        }
        public override void LoadWorldData(TagCompound tag)//加载世界时读取tag
        {
            DownedTutorialEye = tag.GetBool("DownedTutorialEye");//加载世界时读取这个tag，并且赋值给字段
            base.LoadWorldData(tag);
        }
    }
}