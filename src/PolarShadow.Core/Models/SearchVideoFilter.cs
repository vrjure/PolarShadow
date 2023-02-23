using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class SearchVideoFilter : PageFilter
    {
        public SearchVideoFilter() { }
        public SearchVideoFilter(int page, int pageSize, string searchKey)
        {
            this.Page = page;
            this.PageSize = pageSize;
            this.SearchKey = searchKey;
        }

        public SearchVideoFilter(PageFilter page, string searchKey):this(page.Page, page.PageSize, searchKey)
        {

        }

        public string SearchKey { get; set; }
    }
}
