using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.ResourcePack
{
    public class YeMu : IHtmlAnalysisSource
    {
        public string Name => "夜幕";

        public string Src => "www.yemu.xyz";

        public string GetAnalysisSource(string src)
        {
            return HtmlAnalysisSource.FormateDefault(this,src);
        }
    }
}
