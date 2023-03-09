using PolarShadow.Core;
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
                DetailSrc = detail.DetailSrc,
                ImageSrc = detail.ImageSrc,
                Name = detail.Name,
                SiteName = detail.SiteName,
                Seasons = JsonSerializer.Serialize(detail.Seasons, JsonOption.DefaultSerializer)
            };
        }

        public static VideoDetail ToModel(this VideoDetailEntity entity)
        {
            return new VideoDetail
            {
                Description = entity.Description,
                DetailSrc = entity.DetailSrc,
                ImageSrc = entity.ImageSrc,
                Name = entity.Name,
                SiteName = entity.SiteName,
                Seasons = string.IsNullOrEmpty(entity.Seasons) ? new List<VideoSeason>() : JsonSerializer.Deserialize<ICollection<VideoSeason>>(entity.Seasons, JsonOption.DefaultSerializer)
            };
        }
    }
}
