using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface ISite
    {
        string Name { get; set; }
        string Domain { get; set; }
        bool UseWebView { get; set; }
        IKeyValueParameter Parameters { get; set; }
        ISiteRequest this[string requestName] { get; set; }
        IEnumerable<KeyValuePair<string, ISiteRequest>> Requests { get; }
        void Remove(string requestName);
        ISiteRequestHandler CreateRequestHandler(string requestName);
        void WriteTo(Utf8JsonWriter writer);
    }
}
