using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class SearcHandlerDefault : ISearcHandler
    {
        private readonly IPolarShadowSite[] _searcheSites;
        private readonly SearchVideoFilter _filter;

        private int _searchIndex = -1;
        private PageResult<VideoSummary> _resultCache;

        public SearcHandlerDefault(string searchKey, int pageSize, params IPolarShadowSite[] searcheSites)
        {
            _searcheSites = searcheSites;
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
            if (_searcheSites == null || _searcheSites.Length == 0 || cancellation.IsCancellationRequested)
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

            if (_searchIndex >= _searcheSites.Length)
            {
                return null;
            }

            var site = _searcheSites[_searchIndex];

            try
            {
                site.TryGetAbility(out ISearchAble search);
                _resultCache = await site.ExecuteAsync(search, _filter, cancellation);
            }
            catch
            {
                _resultCache = null;
                return null;
            }

            if (_resultCache == null || _resultCache.Data?.Count == 0)
            {
                _filter.Page = 1;
                return await SearchNextAsync(cancellation);
            }
            return _resultCache?.Data;

        }
    }
}
