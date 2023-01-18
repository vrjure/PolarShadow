using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public interface IHtmlAnalysisAble
    {
        ICollection<HtmlAnalysisSource> SupportSources { get; }
    }
}
