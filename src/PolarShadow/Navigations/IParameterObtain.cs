using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public interface IParameterObtain
    {
        void ApplyParameter(IDictionary<string, object> parameters);
    }
}
