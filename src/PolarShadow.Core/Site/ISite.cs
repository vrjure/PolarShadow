using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface ISite
    {
        string Name { get; }
        string Domain { get; }
        IParameter Parameters { get; }
        ISiteRequest this[string requestName] { get; }
        IEnumerable<ISiteRequest> Requests { get; }
        ISiteRequestHandler CreateRequestHandler(string requestName);
    }
}
