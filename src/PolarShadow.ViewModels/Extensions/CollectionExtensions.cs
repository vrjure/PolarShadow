using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static IList<T> AsList<T>(this ICollection<T> source)
        {
            if (source is IList<T> list)
            {
                return list;
            }
            return source.ToList();
        }
    }
}
