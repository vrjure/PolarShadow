using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IHtmlAnalysisSource
    {
        string Name { get; }
        string Src { get; }
        string GetAnalysisSource(string src);
    }
}
