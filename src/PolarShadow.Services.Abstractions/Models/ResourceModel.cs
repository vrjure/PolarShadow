using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public class ResourceModel : Resource, ISyncAbleModel, IKey
    {
        public virtual long Id { get; set; }
        public virtual long ParentId { get; set; }
        public virtual long RootId { get; set; }
        public virtual int Level { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
