using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    /// <summary>
    /// 文件头
    /// </summary>
    internal class IcoFile
    {
        public short Reserved { get; set; }
        /// <summary>
        /// 资源类型 1:图标 2:光标
        /// </summary>
        public short Type { get; set; }
        /// <summary>
        /// 图像个数
        /// </summary>
        public short Count { get; set; }

        public IcoEntry[] Entries { get; set; }
        public IcoInfo[] Infos { get; set; }
    }
}
