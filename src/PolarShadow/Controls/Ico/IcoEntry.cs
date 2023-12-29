using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    /// <summary>
    /// 图像信息块
    /// </summary>
    internal class IcoEntry
    {
        /// <summary>
        /// 图标宽度
        /// </summary>
        public byte Width { get; set; }
        /// <summary>
        /// 图标高度
        /// </summary>
        public byte Height { get; set; }
        /// <summary>
        /// 颜色计数 02单色; 00>=256色
        /// </summary>
        public byte Colors { get; set; }
        public byte Reserved { get; set; }

        /// <summary>
        /// 位面板数
        /// </summary>
        public short Planes { get; set; }
        /// <summary>
        /// 每像素所占位数
        /// </summary>
        public short BitsPrePixel { get; set; }

        /// <summary>
        /// 图像数据块长度
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// 图像数据块相对于文件头部的偏移量
        /// </summary>
        public int Offset { get; set; }
    }
}
