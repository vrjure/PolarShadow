using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class SearchFilter : PageFilter
    {
        public SearchFilter() { }
        
        public SearchFilter(int page, int pageSize, string searchKey)
        {
            this.Page = page;
            this.PageSize = pageSize;
            this.SearchKey = searchKey;
        }
        public string SearchKey { get; set; }
    }
}
