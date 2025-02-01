using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MyMod
{
    public class ScreenMovePlayer : ModPlayer //ModPlayer类中有移动屏幕的函数(单独开一个，没必要所有功能全挤在一个modplayer类，反正最后都会一起执行)
    {
        public Vector2 TargetScreenPos = Vector2.Zero;//本字段是用以记录将要移动到的目标位置;
        public Vector2 CurrentScreenPos = Vector2.Zero;//本字段是用以记录屏幕上一个位置的，因为屏幕位置会被每帧重置
        //想要制作连续运动轨迹的屏幕，就必须预存一个坐标，再每帧把这个坐标进行改动，最后赋值到屏幕坐标
        public int ScreenShakeTimer = 0;//做一个震屏计时器
        public float ScreenShakeScale = 0;//做一个震屏幅度控制器(以像素为单位)
        public override void ResetEffects()//这个函数是每帧最先执行的，用来重置一些东西
        {
            if (TargetScreenPos == Vector2.Zero)//判断一下目标位置是否为默认的0向量
            {
                CurrentScreenPos = Main.screenPosition;//如果是0，说明我没有打算移动屏幕，所以把坐标字段记录为屏幕坐标
            }
            else//如果不是，说明我要移动到那个位置了
            {
                CurrentScreenPos = Vector2.Lerp(CurrentScreenPos, TargetScreenPos, 0.09f);//以一个平滑插值移动
                TargetScreenPos = Vector2.Zero;//然后让目标位置归零，防止外面不赋值了这个目标位置还在原地
            }
            base.ResetEffects();
        }
        public override void ModifyScreenPosition()//修改屏幕的坐标的钩子
        {
            if(TargetScreenPos != Vector2.Zero)//如果目标位置被修改了，此时current才能发挥作用
            Main.screenPosition = CurrentScreenPos;//在这里将屏幕坐标赋值为current
            //以上是移动屏幕坐标的手段，那么接下来介绍震屏
            if (ScreenShakeTimer > 0 && ScreenShakeScale > 0)//在这两个都被赋值为大于0的数时，执行震屏
            {
                Main.screenPosition += Main.rand.NextVector2Circular(ScreenShakeScale, ScreenShakeScale);
                //没错，直接让屏幕坐标每帧加上一个随机向量，这个随机向量在以scale为半径的圆内
                ScreenShakeTimer--;//让计时器减少(为什么我要在这里写减少而不是在reset里是因为写在这里在暂停时也会减少，避免震动时暂停导致一直震)
            }
            base.ModifyScreenPosition();
        }
    }
}