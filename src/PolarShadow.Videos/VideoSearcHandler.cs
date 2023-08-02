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
        bool _isSiteFirstRequest = true;
        public VideoSearcHandler(string requestName, SearchVideoFilter input, IEnumerable<ISite> sites) : base(requestName, input, sites)
        {
        }

        public async Task<PageResult<VideoSummary>> SearchNextAsync(CancellationToken cancellation = default)
        {
            PageResult<VideoSummary> result = null;
            if (Current != null)
            {
                result = await ExecuteAsync(cancellation).ConfigureAwait(false);
                if (result != null && result.Data != null && result.Data.Count > 0) return result;
            }

            if (!MoveNext()) return default;

            _isSiteFirstRequest = true;

            return await SearchNextAsync(cancellation);
        }

        protected override void BeforeRequest(ISiteRequestHandler request)
        {
            if (_isSiteFirstRequest)
            {
                if (request.TryGetParameter("startPage", out int startPage))
                {
                    Input.Page = startPage;
                }
                else
                {
                    Input.Page = 1;
                }
            }
            else
            {
                Input.Page++;
            }
        }

        protected override void AfterRequest(ISiteRequestHandler request)
        {
            _isSiteFirstRequest = false;
        }
    }
}
