using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Tool
{
    internal interface IContextStorage
    {
        object Save();
        void Apply(object context);
    }
}
