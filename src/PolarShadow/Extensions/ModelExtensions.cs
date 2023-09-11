using Avalonia.Controls.Shapes;
using PolarShadow.Core;
using PolarShadow.Models;
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

        public static Resource ToResource(this ResourceEntity resource)
        {
            return new Resource
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

        public static ResourceEntity ToResourceEntityFromData(this ResourceViewData resource)
        {
            if (resource.Data is Resource r)
            {
                return r.ToResourceEntity();
            }
            else if (resource.Data is ResourceEntity re)
            {
                return re;
            }

            return null;
        }

        public static EpisodeEntity ToEpisodeEntityFromData(this EpisodeViewData viewData)
        {
            if (viewData.Data is Episode e)
            {
                return e.ToEpisodeEntity();
            }
            else if (viewData.Data is EpisodeEntity ee)
            {
                return ee;
            }

            return null;
        }

        public static LinkEntity ToLinkEntityFromData(this ViewData viewData)
        {
            if(viewData.Data is Link l)
            {
                return l.ToLinkEntity();
            }
            else if (viewData.Data is LinkEntity le)
            {
                return le;
            }

            return null;
        }

        public static ResourceViewData ToResourceViewData(this Resource resource)
        {
            return new ResourceViewData
            {
                Text = resource.Name,
                Image = resource.ImageSrc,
                Description = resource.Description,
                Site = resource.Site,
                Data = resource
            };
        }

        public static EpisodeViewData ToEpisodeViewData(this Episode episode)
        {
            return new EpisodeViewData
            {
                Text = episode.Name,
                Data = episode,
                Links = episode.Links?.Select(f => new ViewData
                {
                    Text = f.Name,
                    Data = f
                })
            };
        }

        public static ResourceViewData ToResourceViewData(this ResourceEntity resource)
        {
            return new ResourceViewData
            {
                Image = resource.ImageSrc,
                Description = resource.Description,
                Site = resource.Site,
                Text = resource.Name,
                Data = resource
            };
        }

        public static EpisodeViewData ToEpisodeViewData(this EpisodeEntity episode, IEnumerable<LinkEntity> links)
        {
            return new EpisodeViewData
            {
                Text = episode.Name,
                Data = episode,
                Links = links.Where(f => f.EpisodeId == episode.Id).Select(f => new ViewData
                {
                    Text = f.Name,
                    Data = f
                })
            };
        }
    }
}
