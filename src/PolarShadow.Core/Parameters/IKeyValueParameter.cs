using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IKeyValueParameter : IParameter
    {
        void Add(string key, ParameterValue value);
    }
}
