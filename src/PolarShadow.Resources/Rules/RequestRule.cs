using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public class RequestRule
    {
        public RequestRule(string requestName)
        {
            this.RequestName = requestName;
        }
        public string RequestName { get; }

        public ICollection<IContentWriting> Writings { get; set; }
    }
}
