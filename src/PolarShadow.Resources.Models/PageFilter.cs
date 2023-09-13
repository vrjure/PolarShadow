using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Resources
{
    public class PageFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class PageResult<T> : PageFilter where T : class
    {
        public int Total { get; set; }
        public ICollection<T> Data { get; set; }
    }
}
