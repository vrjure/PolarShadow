using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public sealed class HtmlAnalysisSource
    {
        private readonly List<IHtmlAnalysisSource> _source = new List<IHtmlAnalysisSource>();
        internal HtmlAnalysisSource() { }

        public ICollection<IHtmlAnalysisSource> AllSupports => new List<IHtmlAnalysisSource>(_source);

        public void AddSource(IHtmlAnalysisSource source)
        {
            _source.Add(source);
        }

        public static string FormateDefault(IHtmlAnalysisSource source, string src)
        {
            return $"https://{source.Src}/?url={src}";
        }
    }
}
