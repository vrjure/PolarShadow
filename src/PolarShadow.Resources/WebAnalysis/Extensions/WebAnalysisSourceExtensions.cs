using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Resources
{
    public static class WebAnalysisSourceExtensions
    {
        public static string GetVideoSourceUrl(this WebAnalysisSource source, ILink videoSource)
        {
            if (source == null) { throw new ArgumentNullException(nameof(videoSource)); }
            var p = new KeyValueParameter();
            p.Add(videoSource);
            return source.Src.Format(p);
        }
    }
}
