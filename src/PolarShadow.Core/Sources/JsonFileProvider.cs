using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class JsonFileProvider : IPolarShadowProvider
    {
        private FileSource _fileSource;
        private JsonElement _root;
        public JsonFileProvider(FileSource source)
        {
            _fileSource = source;
            if (_fileSource == null) { throw new ArgumentNullException(nameof(source)); }
        }

        public void Load()
        {
            if (string.IsNullOrEmpty(_fileSource.Path)) return;
            if (!File.Exists(_fileSource.Path)) return;
            using var fs = new FileStream(_fileSource.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var doc = JsonDocument.Parse(fs);
            _root = doc.RootElement.Clone();
        }

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
