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
    public interface ISite : IWriterJson
    {
        string Name { get; set; }
        string Domain { get; set; }
        bool UseWebView { get; set; }
        IKeyValueParameter Parameters { get; set; }
        IDictionary<string, ISiteRequest> Requests { get; }
        ISiteRequestHandler CreateRequestHandler(string requestName);
    }
}
