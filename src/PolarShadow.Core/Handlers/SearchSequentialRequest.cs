using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class SearchSequentialRequest : SequentialRequestBase<SearchFilter, PageResult<Resource>>, ISearchHandler
    {
        bool _isSiteFirstRequest = true;

        public SearchSequentialRequest(string requestName, SearchFilter input, IEnumerable<ISite> sites) : base(requestName, input, sites)
        {

        }

        public async Task<PageResult<Resource>> SearchNextAsync(CancellationToken cancellation = default)
        {
            PageResult<Resource> result = null;
            if (Current != null)
            {
                result = await ExecuteAsync(cancellation).ConfigureAwait(false);
                if (result != null && result.Data != null && result.Data.Count > 0) return result;
            }

            if (!MoveNext()) return default;

            _isSiteFirstRequest = true;

            return await SearchNextAsync(cancellation);
        }

        protected override bool BeforeRequest(ISiteRequestHandler request)
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
                if (!request.TryGetParameter("canPage", out bool canPage))
                {
                    Input.Page++;
                    return true;
                }

                if (canPage)
                {
                    Input.Page++;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        protected override void AfterRequest(ISiteRequestHandler request)
        {
            _isSiteFirstRequest = false;
        }
    }
}
