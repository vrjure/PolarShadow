using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public abstract class JsonProvider : IPolarShadowProvider
    {
        private JsonElement _root;

        public JsonElement Root => _root;

        public void Load()
        {
            _root = Parse();
        }

        protected abstract JsonElement Parse();
    }
}
