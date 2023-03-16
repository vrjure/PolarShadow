using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public sealed class WebAnalysisSource : IKeyName
    {
        public string Name { get; set; }
        public string Title { get; set; }

        public string Src { get; set; }
    }
}
