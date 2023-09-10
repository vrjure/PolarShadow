using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class Episode
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public ICollection<ILink> Links { get; set; }
    }
}
