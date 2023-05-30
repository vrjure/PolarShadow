using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PolarShadow.Core
{
    public interface IPolarShadowOptionBuilder
    {
        bool IsChanged { get; }
        IPolarShadowSiteOptionBuilder AddSite(string name);
        IPolarShadowOptionBuilder RemoveSite(string name);
        IPolarShadowOptionBuilder ClearSite();
        IPolarShadowOptionBuilder AddWebAnalysisSite(WebAnalysisSource source);
        IPolarShadowOptionBuilder RemoveWebAnalysisSite(string name);
        IPolarShadowOptionBuilder ClearWebAnalysisSite();
        IPolarShadowOptionBuilder AddParameter<T>(string name, T value);
        IPolarShadowOptionBuilder RemoveParameter(string name);
        IPolarShadowOptionBuilder ClearParameter();
        IPolarShadowOptionBuilder ConfigureFromStream(Stream stream);
        void WriteTo(Stream stream);
        PolarShadowOption Build();
    }
}
