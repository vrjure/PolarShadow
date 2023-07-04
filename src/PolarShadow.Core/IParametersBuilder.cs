using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    public interface IParametersBuilder
    {
        IParametersBuilder AddParameter<T>(string name, T value);
        IParametersBuilder RemoveParameter(string name);
        IParametersBuilder SetParameter(string parameters);
        void WriteTo(Stream output);
    }
}
