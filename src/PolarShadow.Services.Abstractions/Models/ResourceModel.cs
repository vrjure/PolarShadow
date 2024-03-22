using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public class ResourceModel : Resource
    {
        public virtual int Id { get; set; }
        public virtual int ParentId { get; set; }
        public virtual int RootId { get; set; }
        public virtual int Level { get; set; }
    }
}
