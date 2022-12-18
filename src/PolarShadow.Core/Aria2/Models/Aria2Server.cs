using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Aria2
{
    public class Aria2Server
    {
        /// <summary>
        /// Index of the file, starting at 1, in the same order as files appear in the multi-file metalink.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// A list of structs which contain the following keys.
        /// </summary>
        public ICollection<Aria2ServerInfo> Servers { get; set; }
    }
}
