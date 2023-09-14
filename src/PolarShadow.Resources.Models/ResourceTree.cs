using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public class ResourceTree : Resource
    {
        public ICollection<ResourceTree> Children { get; set; }
    }
}
