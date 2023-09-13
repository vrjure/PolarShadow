using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Resources
{
    internal class SiteRequest : ISiteRequest
    {
        public IRequestTemplate Request { get; set; }
        public IResponseTemplate Response { get; set; }
        public IKeyValueParameter Parameter { get; set; }

        public bool? UseWebView { get; set; }
    }
}
