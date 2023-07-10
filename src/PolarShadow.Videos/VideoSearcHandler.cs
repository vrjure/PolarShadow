using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Videos
{
    internal class VideoSearcHandler : SequentialRequestBase<SearchVideoFilter, PageResult<VideoSummary>>, IVideoSearcHandler
    {
        public VideoSearcHandler(string abilityName, SearchVideoFilter input, IEnumerable<ISite> sites) : base(abilityName, input, sites)
        {
        }

        protected override bool HasResult(Stream stream)
        {
            if (stream.Length == 0)
            {
                return false;
            }

            var result = JsonSerializer.Deserialize<PageResult<VideoSummary>>(stream, JsonOption.DefaultSerializer);
            return result != null && result.Data != null && result.Data.Count > 0;
        }

        protected override void NextRequest(SearchVideoFilter input, ISiteRequestHandler request)
        {
            input.Page++;
        }

        protected override void ResetRequest(SearchVideoFilter input, ISiteRequestHandler request)
        {
            if(request.TryGetParameter("startPage", out int startPage))
            {
                input.Page = startPage;
            }
            else
            {
                input.Page = 1;
            }
        }
    }
}
