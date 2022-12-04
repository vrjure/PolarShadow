using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.ResourcePack
{
    internal class ZmzSearchResult
    {
        public ZmzPage Page { get; set; }
        public ICollection<ZmzSearchItem> Data { get; set; }
    }
}
