using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    public interface IPolarShadowOptionBuilder
    {
        bool IsChanged { get; }
        void ChangeNodify();
        bool ContainKey(string name);
        bool TryGetOption<T>(string name, out T option);
        T GetOption<T>(string name);
        IPolarShadowOptionBuilder AddOptions<T>(string name, IEnumerable<T> options);
        IPolarShadowOptionBuilder AddOption<T>(string name, T option);
        IPolarShadowOptionBuilder RemoveOption(string name);
        IParametersBuilder Parameters { get; }
        void WriteTo(Stream stream);
        IPolarShadowOptionBuilder Load(Stream stream);
    }
}
