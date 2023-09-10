using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class Resource : Link
    {
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public ICollection<Episode> Episodes { get; set;}
    }
}
