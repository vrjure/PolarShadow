using PolarShadow.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Resources
{
    public class SearchSequentialHandler<TLink> : ISearchHandler<TLink> where TLink : class, ILink
    {
        private readonly IPolarShadow _polar;
        private readonly string _requestName;
        private readonly string _searchText;

        private IEnumerator<ISite> _sites;
        private ISearchHandler<TLink> _internalHandler;
        public SearchSequentialHandler(IPolarShadow polar, string requestName, string searchText)
        {
            _polar = polar;
            _requestName = requestName;
            _searchText = searchText;
            _sites = polar.GetSites(f => f.Equals(requestName)).GetEnumerator();
        }

        public async Task<ICollection<TLink>> SearchNextAsync(CancellationToken cancellation = default)
        {
            if (_sites == null) return null;

            if (_internalHandler == null && !MoveNext())
            {
                return null;
            }

            if (_internalHandler == null)
            {
                if (_sites.Current == null) return null;
                _internalHandler = new SearchPagingHandler<TLink>(_polar, _sites.Current, _requestName, _searchText);
            }

            try
            {
                var result = await _internalHandler.SearchNextAsync(cancellation);
                if (result?.Count != 0) return result;
            }
            catch
            {
                _internalHandler = null;
            }

            return await SearchNextAsync(cancellation);
        }

        private bool MoveNext()
        {
            if (_sites == null) return false;
            if (!_sites.MoveNext())
            {
                _sites.Dispose();
                _sites = null;
                return false;
            }

            return true;
        }
    }
}
