using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Models
{
    public class EpisodeViewData : ViewData
    {
        public IEnumerable<ViewData>  Links { get; set; }
    }
}
