using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class SearcHandlerDefault : ISearcHandler
    {
        private readonly ISearchAble[] _searches;
        private readonly SearchVideoFilter _filter;

        private int _searchIndex = -1;
        private PageResult<VideoSummary> _resultCache;

        public SearcHandlerDefault(string searchKey, int pageSize, params ISearchAble[] searches)
        {
            _searches = searches;
            _filter = new SearchVideoFilter(1, pageSize, searchKey);
        }

        public void Reset()
        {
            _searchIndex = -1;
            _filter.Page = 1;
            _resultCache = null;
        }

        public async Task<ICollection<VideoSummary>> SearchNextAsync(CancellationToken cancellation = default)
        {
            if (_searches == null || _searches.Length == 0 || cancellation.IsCancellationRequested)
            {
                return null;
            }

            if (_resultCache == null || _resultCache.Data?.Count == 0)
            {
                _searchIndex++;
            }
            else
            {
                _filter.Page++;
            }

            if (_searchIndex >= _searches.Length)
            {
                return null;
            }

            var search = _searches[_searchIndex];

            try
            {
                _resultCache = await search.SearchVideosAsync(_filter, cancellation);

                if (_resultCache == null || _resultCache.Data.Count == 0)
                {
                    _filter.Page = 1;
                    return await SearchNextAsync(cancellation);
                }
                return _resultCache?.Data;
            }
            catch
            {
                _resultCache = null;
                return null;
            }
            
        }
    }
}
