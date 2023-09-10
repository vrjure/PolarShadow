using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Models
{
    public class DetailViewData : NameViewData
    {
        public string Image { get; set; }
        public string Description { get; set; }
        public string Site { get; set; }
        public IDictionary<string, IEnumerable<NameViewData>> Episodes { get; set; }
    }
}
