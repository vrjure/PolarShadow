using PolarShadow.Core;
using PolarShadow.Videos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    internal static class VideoDetailConverter
    {
        public static VideoDetailEntity ToEntity(this VideoDetail detail)
        {
            return new VideoDetailEntity
            {
                Description = detail.Description,
                Src = detail.Src,
                ImageSrc = detail.ImageSrc,
                Name = detail.Name,
                Site = detail.Site,
                Seasons = JsonSerializer.Serialize(detail.Seasons, JsonOption.DefaultSerializer)
            };
        }

        public static VideoDetail ToModel(this VideoDetailEntity entity)
        {
            return new VideoDetail
            {
                Description = entity.Description,
                Src = entity.Src,
                ImageSrc = entity.ImageSrc,
                Name = entity.Name,
                Site = entity.Site,
                Seasons = string.IsNullOrEmpty(entity.Seasons) ? new List<VideoSeason>() : JsonSerializer.Deserialize<ICollection<VideoSeason>>(entity.Seasons, JsonOption.DefaultSerializer)
            };
        }
    }
}
