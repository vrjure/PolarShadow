using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.ResourcePack
{
    public class XMFlv : IHtmlAnalysisSource
    {
        public string Name => "虾米";

        public string Src => "jx.xmlv.com";

        public string GetAnalysisSource(string src)
        {
            return HtmlAnalysisSource.FormateDefault(this, src);
        }
    }
}
