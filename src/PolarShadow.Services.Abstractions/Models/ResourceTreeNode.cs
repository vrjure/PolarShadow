using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public class ResourceTreeNode : ResourceModel
    {
        public ICollection<ResourceTreeNode> Children { get; set; }
    }
}
