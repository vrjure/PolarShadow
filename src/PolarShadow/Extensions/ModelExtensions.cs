using PolarShadow.Core;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    public static class ModelExtensions
    {
        public static LinkEntity ToLinkEntity(this ILink link)
        {
            return new LinkEntity
            {
                Name = link.Name,
                FromRequest = link.FromRequest,
                Request = link.Request,
                Site = link.Site,
                Src = link.Src,
                SrcType = link.SrcType
            };
        }

        public static EpisodeEntity ToEpisodeEntity(this Episode episode)
        {
            return new EpisodeEntity
            {
                Name = episode.Name,
                Tag = episode.Tag
            };
        }

        public static ResourceEntity ToResourceEntity(this Resource resource)
        {
            return new ResourceEntity
            {
                Description = resource.Description,
                FromRequest = resource.FromRequest,
                ImageSrc = resource.ImageSrc,
                Name = resource.Name,
                Request = resource.Request,
                Site = resource.Site,
                Src = resource.Src,
                SrcType = resource.SrcType
            };
        }
    }
}
