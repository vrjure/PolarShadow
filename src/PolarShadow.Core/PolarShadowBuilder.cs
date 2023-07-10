using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public class PolarShadowBuilder : IPolarShadowBuilder
    {
        private readonly ICollection<IPolarShadowItemBuilder> _itemBuilders;

        public PolarShadowBuilder()
        {
            _itemBuilders = new List<IPolarShadowItemBuilder>();
        }

        public IRequestHandler WebViewHandler { get; set; }

        public IRequestHandler HttpHandler { get; set; }

        public IEnumerable<IPolarShadowItemBuilder> ItemBuilders => _itemBuilders;

        public IPolarShadowBuilder Add(IPolarShadowItemBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            _itemBuilders.Add(builder);
            return this;
        }

        public IPolarShadow Build()
        {
            var list = new List<IPolarShadowItem>();
            foreach (var builder in _itemBuilders)
            {
                list.Add(builder.Build(this));
            }

            return new PolarShadowDefault(list);
        }
    }
}
