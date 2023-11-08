using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Services
{
    public class RequestModel
    {
        public virtual string Name { get; set; }
        public virtual string Parameters { get; set; }
        public virtual string Request { get; set; }
        public virtual string Response { get; set; }
        public virtual string SiteName { get; set; }
    }
}
