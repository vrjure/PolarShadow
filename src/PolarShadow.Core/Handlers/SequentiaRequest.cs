using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Handlers
{
    public class SequentiaRequest : SequentialRequestBase<Resource>
    {
        public SequentiaRequest(string requestName, IEnumerable<ISite> sites) : base(requestName, sites)
        {

        }
    }
}
