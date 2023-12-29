using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Resources
{
    public interface ISite
    {
        string Name { get; }
        string Domain { get; }
        string Ico { get; set; }
        IKeyValueParameter Parameters { get; }
        IReadOnlyDictionary<string, ISiteRequest> Requests { get; }
    }
}
