using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMod.Common
{
    /// <summary>
    /// 实现这个系统 添加绘制
    /// </summary>
    public class DrawCecheSystem
    {
        /// <summary>
        /// 这是缓存
        /// </summary>
        public class Ceche
        {
            public bool Remove;
            public virtual void DrawCeche() { }
            public virtual void UpdateCeche() { }
        }
        public List<Ceche> CecheList = new();
    }
}
