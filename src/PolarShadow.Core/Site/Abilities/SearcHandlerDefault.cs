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
        private readonly ISearchAble _searchAble;

        private int _searchIndex = 0;
        private PageResult<VideoSummary> _resultCache;

        public SearcHandlerDefault(string searchKey, int pageSize, ISearchAble searchAble, params IPolarShadowSite[] searcheSites)
        {
            _searcheSites = searcheSites;
            _searchKey = searchKey;
            _searchAble = searchAble;
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
            if (_searcheSites == null || _searcheSites.Length == 0 || cancellation.IsCancellationRequested || _searchIndex >= _searcheSites.Length)
            {
                return null;
            }

            var site = _searcheSites[_searchIndex];

            if (_resultCache != null && _resultCache.Data != null && _resultCache.Data.Count != 0)
            {
                if (_searchAble.CanPaging(site))
                {
                    _page.Page++;
                }
                else
                {
                    _page.Page = 1;
                    _searchIndex++;
                    return await SearchNextAsync(cancellation);
                }
            }

            try
            {
                _resultCache = await site.ExecuteAsync(_searchAble, new SearchVideoFilter(_page, _searchKey), cancellation);
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
