using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public class SiteInfoModel
    {
        public SiteModel Site { get; set; }
        public ICollection<RequestModel> Requests { get; set; }
    }
}
