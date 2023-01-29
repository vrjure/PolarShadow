using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IHtmlAnalysisAble
    {
        string GetAnalysisedSource(VideoSource source, HtmlAnalysisSource analysisSource);
    }
}
