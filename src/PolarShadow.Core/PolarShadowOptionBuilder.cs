using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class PolarShadowOptionBuilder : IPolarShadowOptionBuilder
    {
        private readonly PolarShadowOption _option = new PolarShadowOption();

        public bool IsChanged { get; private set; }

        public IPolarShadowOptionBuilder AddParameter<T>(string name, T value)
        {
            _option.Parameters ??= new Dictionary<string, object>();
            _option.Parameters.Add(name, value);
            return this;
        }

        public IPolarShadowSiteOptionBuilder AddSite(string name)
        {
            var siteOption = new PolarShadowSiteOption(name);
            _option.Sites.Add(siteOption);
            return new PolarShadowSiteOptionBuilder(siteOption);
        }

        public IPolarShadowOptionBuilder AddWebAnalysisSite(WebAnalysisSource source)
        {
            _option.AnalysisSources ??= new KeyNameCollection<WebAnalysisSource>();
            _option.AnalysisSources.Add(source);
            return this;
        }

        public PolarShadowOption Build()
        {
            return _option;
        }

        public IPolarShadowOptionBuilder ClearParameter()
        {
            _option.Parameters?.Clear();
            return this;
        }

        public IPolarShadowOptionBuilder ClearSite()
        {
            _option.Sites?.Clear();
            return this;
        }

        public IPolarShadowOptionBuilder ClearWebAnalysisSite()
        {
            _option.AnalysisSources?.Clear();
            return this;
        }

        public IPolarShadowOptionBuilder ConfigureFromStream(Stream stream)
        {
             _option.Apply(JsonSerializer.Deserialize<PolarShadowOption>(stream));
            return this;
        }

        public IPolarShadowOptionBuilder RemoveParameter(string name)
        {
            _option.Parameters?.Remove(name);
            return this;
        }

        public IPolarShadowOptionBuilder RemoveSite(string name)
        {
            (_option.Sites as KeyNameCollection<PolarShadowSiteOption>)?.RemoveKey(name);
            return this;
        }

        public IPolarShadowOptionBuilder RemoveWebAnalysisSite(string name)
        {
            (_option.AnalysisSources as KeyNameCollection<PolarShadowSiteOption>)?.RemoveKey(name);
            return this;
        }

        public void WriteTo(Stream stream)
        {
            JsonSerializer.Serialize(stream, _option, JsonOption.DefaultSerializer);
        }

        private void CheckOption()
        {
            if (_option == null) throw new InvalidOperationException();
        }
    }
}
