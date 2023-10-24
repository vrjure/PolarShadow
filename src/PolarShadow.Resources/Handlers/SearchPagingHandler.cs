using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Resources
{
    public class SearchPagingHandler<TLink> : ISearchHandler<TLink> where TLink : class, ILink
    {
        private readonly IPolarShadow _polar;
        private readonly ISite _site;
        private readonly string _requestName;
        private readonly SearchFilter _filter;

        private bool _isFirst = true;
        private int _endPage = 1;
        private ISiteRequestHandler _requestHandler;

        public SearchPagingHandler(IPolarShadow polar, ISite site, string requestName, string searchText) : this(polar, site, requestName, new SearchFilter(1, 10, searchText))
        {

        }

        private SearchPagingHandler(IPolarShadow polar, ISite site, string requestName, SearchFilter filter)
        {
            _polar = polar ?? throw new ArgumentNullException(nameof(polar));
            _site = site ?? throw new ArgumentNullException(nameof(site));
            _requestName = requestName ?? throw new ArgumentNullException(nameof(requestName));

            if (!_site.HasRequest(_requestName)) throw new InvalidOperationException($"The site [{_site.Name}] not contains request: [{_requestName}]");

            _filter = filter;
        }

        public async Task<ICollection<TLink>> SearchNextAsync(CancellationToken cancellation = default)
        {
            if (string.IsNullOrEmpty(_filter.SearchKey)) return null;
            if (_requestHandler == null)
            {
                _requestHandler = _site.CreateRequestHandler(_polar, _requestName);
            }

            if (BeforeRequest(_requestHandler))
            {
                return await _requestHandler.ExecuteAsync<SearchFilter, ICollection<TLink>>(_filter, cancellation).ConfigureAwait(false);
            }

            return null;
        }

        private bool BeforeRequest(ISiteRequestHandler handler)
        {
            if (_isFirst)
            {
                if (handler.TryGetParameter(SearchParams.StartPage, out int startPage))
                {
                    _filter.Page = startPage;
                }

                if(handler.TryGetParameter(SearchParams.EndPage, out int endPage))
                {
                    _endPage = endPage;
                }

                if (handler.TryGetParameter(SearchParams.PageSize, out int pageSize))
                {
                    _filter.PageSize = pageSize;
                }

                _isFirst = false;
                return true;
            }
            else
            {
                if (!handler.TryGetParameter(SearchParams.CanPage, out bool canPage))
                {
                    canPage = true;
                }

                if (canPage)
                {
                    if (_filter.Page >= _endPage)
                    {
                        return false;
                    }

                    _filter.Page++;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
