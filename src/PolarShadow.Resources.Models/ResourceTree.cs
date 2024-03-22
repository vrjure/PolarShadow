using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Resources
{
    public class ResourceTree : Resource
    {
        private ICollection<ResourceTree> _children;
        public ICollection<ResourceTree> Children
        {
            get
            {
                if (ChildChildren?.Count > 0 && _children?.Count > 0)
                {
                    var list = ChildChildren as IList<ResourceTree> ?? ChildChildren.ToList();
                    var index = 0;
                    foreach (var item in _children)
                    {
                        if (index < list.Count)
                        {
                            item.Children = list[index].Children;
                        }
                        else
                        {
                            break;
                        }
                        index++;
                    }
                }
                return _children;
            }
            set => _children = value;
        }
        public ICollection<ResourceTree> ChildChildren { get; set; }
    }
}
