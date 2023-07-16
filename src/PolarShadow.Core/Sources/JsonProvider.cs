using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public abstract class JsonProvider : IPolarShadowProvider
    {
        private JsonElement _root;

        public void Load()
        {
            _root = Parse();
        }

        protected abstract JsonElement Parse();

        public bool TryGet(string name, out JsonElement value)
        {
            if (_root.ValueKind == JsonValueKind.Undefined)
            {
                value = default;
                return false;
            }
            return _root.TryGetProperty(name, out value);
        }
    }
}
