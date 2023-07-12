using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    internal class PolarShadowDefault : IPolarShadow
    {
        private readonly ICollection<IPolarShadowItem> _items;
        private readonly IPolarShadowBuilder _builder;
        private IParameterCollection _parameters;

        internal PolarShadowDefault(IEnumerable<IPolarShadowItem> items)
        {
            _items = new List<IPolarShadowItem>(items);
        }

        public PolarShadowDefault(IPolarShadowBuilder builder)
        {
            _builder = builder;
            _items = new List<IPolarShadowItem>();
            _parameters = new Parameters();
        }

        public IEnumerable<IPolarShadowItem> Items => _items;

        public void Load()
        {
            _parameters.Clear();
            _items.Clear();

            _parameters.Add(_builder.Parameters);
            var providers = new List<IPolarShadowProvider>();
            foreach (var item in _builder.Sources)
            {
                var provider = item.Build(_builder);
                if (provider != null)
                {
                    BuildParameters(provider);
                    providers.Add(provider);
                }
            }

            foreach (var provider in providers)
            {
                foreach (var builder in _builder.ItemBuilders)
                {
                    var item = builder.Build(_builder, provider);
                    if (item != null)
                    {
                        _items.Add(item);
                    }
                }
            }
        }

        private void BuildParameters(IPolarShadowProvider provider)
        {
            if (provider.TryGet("parameters", out JsonElement value))
            {
                _parameters.Add(ParametersBuilder.Build(value));
            }
        }
    }
}
