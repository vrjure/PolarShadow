using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Resources
{
    internal class SiteDefault : ISite
    {
        [JsonRequired]
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Ico { get; set; }
        public IKeyValueParameter Parameters { get; set; }
        public IReadOnlyDictionary<string, ISiteRequest> Requests { get; set; }
    }
}
