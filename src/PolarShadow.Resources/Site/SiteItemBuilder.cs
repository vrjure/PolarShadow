using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Resources
{
    internal sealed class SiteItemBuilder : ISiteItemBuilder
    {   
        public IRequestHandler WebViewHandler { get; set; }
        public IRequestHandler HttpHandler { get; set; }

        public ICollection<IContentWriting> Writings { get; } = new List<IContentWriting>();

        public IPolarShadowItem Build(IPolarShadowBuilder builder)
        {
            var writingDict = new Dictionary<string, ICollection<IContentWriting>>();
            foreach (var b in Writings)
            {
                if (b.RequestFilter == null)
                {
                    continue;
                }

                foreach (var item in b.RequestFilter)
                {
                    if (!writingDict.TryGetValue(item, out ICollection<IContentWriting> writings))
                    {
                        writings = new List<IContentWriting>();
                        writingDict[item] = writings;
                    }

                    writings.Add(b);
                }
            }
            return new SiteItem(HttpHandler ?? new HttpClientRequestHandler(), WebViewHandler, builder.Parameters, writingDict);
        }
    }
}
