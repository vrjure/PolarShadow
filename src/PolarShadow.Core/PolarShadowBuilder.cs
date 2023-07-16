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
        private IKeyValueParameter _parameters;
        public PolarShadowBuilder()
        {
            _itemBuilders = new List<IPolarShadowItemBuilder>();
            _parameters = new KeyValueParameter();
        }

        public IRequestHandler WebViewHandler { get; set; }

        public IRequestHandler HttpHandler { get; set; }

        public IEnumerable<IPolarShadowItemBuilder> ItemBuilders => _itemBuilders;

        public IKeyValueParameter Parameters => _parameters;

        public IPolarShadowBuilder Add(IPolarShadowItemBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            _itemBuilders.Add(builder);
            return this;
        }

        public IPolarShadow Build()
        {         
            HttpHandler ??= new HttpClientRequestHandler();

            var items = new List<IPolarShadowItem>();
            foreach (var item in items)
            {

            }
            return new PolarShadowDefault(this);
        }
    }
}
