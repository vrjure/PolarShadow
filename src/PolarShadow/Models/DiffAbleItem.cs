using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Models
{
    internal class DiffAbleItem<T>
    {
        public T Item { get; set; }
        public bool IsDifference { get; set; }
        public int DiffCount { get; set; }
    }
}
