using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public interface IPolarShadowSite
    {
        string Name { get; }
        string Domain { get; }
        bool HasAbility(string name);
        bool TryGetParameter<TValue>(string name, out TValue value);
        ISiteRequestHandler CreateRequestHandler(string name);
    }
}
