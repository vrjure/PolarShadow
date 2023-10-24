using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Resources
{
    public class SearchBatchHandler<TLink> : ISearchHandler<TLink> where TLink : class, ILink
    {
        private readonly string _searchText;
        private readonly IPolarShadow _polar;
        private readonly string _requestName;
        private readonly int _maxTaskCount = 3;

        private IEnumerator<ISite> _sites;
        private readonly ICollection<Task> _tasks;

        public SearchBatchHandler(IPolarShadow polar, string requestName, string searchText, int maxTaskCount = 3) : this(polar, polar.GetSites(f=>f.HasRequest(requestName)), requestName, searchText, maxTaskCount)
        {

        }
        public SearchBatchHandler(IPolarShadow polar, IEnumerable<ISite> sites, string requestName, string searchText, int maxTaskCount = 3)
        {
            _polar = polar;
            _sites = sites.GetEnumerator();
            _requestName = requestName;
            _maxTaskCount = maxTaskCount <= 0 ? _maxTaskCount : maxTaskCount;
            _tasks = new List<Task>();
            _searchText = searchText;
        }

        public async Task<ICollection<TLink>> SearchNextAsync(CancellationToken cancellation = default)
        {
            start:
            for (int i = 0; i < _maxTaskCount; i++)
            {
                if (_sites == null) break;

                if (!MoveNext())
                {
                    break;
                }

                _tasks.Add(Searching(cancellation));
                if(_tasks.Count >= _maxTaskCount)
                {
                    break;
                }
            }

            if (_tasks.Count == 0) return null;

            List<TLink> result = null;
            while (_tasks.Any())
            {
                var task = (Task<ICollection<TLink>>)await Task.WhenAny(_tasks);

                try
                {
                    var list = await task;
                    if (result == null) result = new List<TLink>();

                    result.AddRange(list);
                }
                catch { }

                _tasks.Remove(task);
            }

            if (result == null || result.Count == 0)
            {
                goto start;
            }

            return result;
        }

        private async Task<ICollection<TLink>> Searching(CancellationToken cancellation)
        {
            var handler = new SearchPagingHandler<TLink>(_polar, _sites.Current, _requestName, _searchText);
            List<TLink> result = null;
            var searchResult = await handler.SearchNextAsync(cancellation);
            while (searchResult != null && searchResult.Count > 0)
            {
                if (result == null) result = new List<TLink>();

                result.AddRange(searchResult);

                searchResult = await handler.SearchNextAsync(cancellation);
            }

            return result;
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
