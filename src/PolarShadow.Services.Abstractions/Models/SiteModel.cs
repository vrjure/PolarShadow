using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public class SiteModel
    {
        public virtual string Name { get; set; }
        public virtual string Domain { get; set; }
        public virtual bool? UseWebView { get; set; }
        public virtual string Parameters { get; set; }
    }
}
