using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;//要想写能存储的东西，必引用这个

namespace MyMod
{
	public class MyWorldSystem:ModSystem //modsys是1.3版本的modworld，也就是控制世界的东西
	{
		public static bool DownedTutorialEye = false;//定义一个字段，代表是否击杀了教程之魔眼
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