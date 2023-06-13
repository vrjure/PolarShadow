using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Videos
{
    public static class PolarshadowExtensions
    {
        public static IVideoSearcHandler CreateVideoSearcHandler(this IPolarShadow polar, SearchVideoFilter videoFilter)
        {
            var sites = polar.GetSites().Where(f => f.HasAbility(VideoAbilities.Search));
            return new VideoSearcHandler(VideoAbilities.Search, videoFilter, sites, polar.Parameters);
        }
    }
}
