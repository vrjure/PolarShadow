using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Services
{
    public static class ResourceExtensions
    {
        public static ResourceTreeNode BuildTree(this IEnumerable<ResourceModel> resources)
        {
            var order = resources.OrderBy(f => f.Level).ThenBy(f => f.ParentId);

            ResourceTreeNode tree = null;

            var dict = new Dictionary<int, ResourceTreeNode>();
            foreach (var item in resources)
            {
                if (item.Level == 0 && item.ParentId == 0)
                {
                    tree = item.CreateTreeNode();
                    dict.Add(item.Id, tree);
                    continue;
                }

                if (tree == null) continue;

                if (dict.TryGetValue(item.ParentId, out var parent))
                {
                    parent.Children ??= new List<ResourceTreeNode>();
                    var child = item.CreateTreeNode();
                    parent.Children.Add(child);
                    dict.Add(child.Id, child);
                }
            }

            return tree;
        }

        public static ResourceTreeNode CreateTreeNode(this ResourceModel resource)
        {
            if (resource is ResourceTreeNode rtm) return rtm;

            return new ResourceTreeNode
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                ImageSrc = resource.ImageSrc,
                FromRequest = resource.FromRequest,
                Request = resource.Request,
                Site = resource.Site,
                Src = resource.Src,
                SrcType = resource.SrcType,
                ParentId = resource.ParentId,
                RootId = resource.RootId,
                Level = resource.Level
            };
        }

        public static ResourceTreeNode ToResourceTreeNode(this ResourceTree tree)
        {
            return new ResourceTreeNode
            {
                Name = tree.Name,
                Description = tree.Description,
                ImageSrc = tree.ImageSrc,
                FromRequest = tree.FromRequest,
                Request = tree.Request,
                Site = tree.Site,
                Src = tree.Src,
                SrcType = tree.SrcType,
                Children = tree.Children?.Select(f => f.ToResourceTreeNode()).ToList()
            };
        }
    }
}
