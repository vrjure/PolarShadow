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
        private readonly PageFilter _page;
        private readonly string _searchKey;

        private int _searchIndex = 0;
        private PageResult<VideoSummary> _resultCache;

        public SearcHandlerDefault(string searchKey, int pageSize, params IPolarShadowSite[] searcheSites)
        {
            _searcheSites = searcheSites;
            _searchKey = searchKey;
            _page = new PageFilter
            {
                Page = 1,
                PageSize = pageSize
            };
        }

        public void Reset()
        {
            _searchIndex = 0;
            _page.Page = 1;
            _resultCache = null;
        }

        public async Task<ICollection<VideoSummary>> SearchNextAsync(CancellationToken cancellation = default)
        {
            if (_searcheSites == null || _searcheSites.Length == 0 || cancellation.IsCancellationRequested)
            {
                return null;
            }

            if (_resultCache != null && _resultCache.Data != null && _resultCache.Data.Count != 0)
            {
                _page.Page++;
            }

            if (_searchIndex >= _searcheSites.Length)
            {
                return null;
            }

            var site = _searcheSites[_searchIndex];

            try
            {
                site.TryGetAbility(out ISearchAble search);
                _resultCache = await site.ExecuteAsync(search, new SearchVideoFilter(_page, _searchKey), cancellation);
            }
            catch
            {
                _resultCache = null;
            }

            if (_resultCache == null || _resultCache.Data == null || _resultCache.Data.Count == 0)
            {
                _page.Page = 1;
                _searchIndex++;
                return await SearchNextAsync(cancellation);
            }
            return _resultCache?.Data;

        }
    }
}
