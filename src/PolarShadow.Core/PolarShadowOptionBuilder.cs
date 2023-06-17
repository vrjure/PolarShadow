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

        public bool IsChanged { get; internal set; }

        public IPolarShadowOptionBuilder AddParameter<T>(string name, T value)
        {
            _option.Parameters ??= new Dictionary<string, object>();
            _option.Parameters.Add(name, value);
            ChangeNodify();
            return this;
        }

        public IPolarShadowSiteOptionBuilder AddSite(string name)
        {
            var sites = _option.Sites as KeyNameCollection<PolarShadowSiteOption>;
            if (sites != null && sites.TryGetValue(name, out PolarShadowSiteOption op))
            {
                return new PolarShadowSiteOptionBuilder(op, this);
            }
            _option.Sites ??= new KeyNameCollection<PolarShadowSiteOption>();
            var siteOption = new PolarShadowSiteOption(name);
            _option.Sites.Add(siteOption);
            ChangeNodify();
            return new PolarShadowSiteOptionBuilder(siteOption, this);
        }

        public IPolarShadowOptionBuilder AddWebAnalysisSite(WebAnalysisSource source)
        {
            _option.AnalysisSources ??= new KeyNameCollection<WebAnalysisSource>();
            _option.AnalysisSources.Add(source);
            ChangeNodify();
            return this;
        }

        public PolarShadowOption Build()
        {
            IsChanged = false;
            return _option;
        }

        public void ChangeNodify()
        {
            IsChanged = true;
        }

        public IPolarShadowOptionBuilder ClearParameter()
        {
            _option.Parameters?.Clear();
            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder ClearSite()
        {
            _option.Sites?.Clear();
            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder ClearWebAnalysisSite()
        {
            _option.AnalysisSources?.Clear();
            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder Load(Stream stream)
        {
             _option.Apply(JsonSerializer.Deserialize<PolarShadowOption>(stream, JsonOption.DefaultSerializer));
            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder RemoveParameter(string name)
        {
            _option.Parameters?.Remove(name);
            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder RemoveSite(string name)
        {
            (_option.Sites as KeyNameCollection<PolarShadowSiteOption>)?.RemoveKey(name);
            ChangeNodify();
            return this;
        }

        public IPolarShadowOptionBuilder RemoveWebAnalysisSite(string name)
        {
            (_option.AnalysisSources as KeyNameCollection<PolarShadowSiteOption>)?.RemoveKey(name);
            ChangeNodify();
            return this;
        }

        public void WriteTo(Stream stream)
        {
            JsonSerializer.Serialize(stream, _option, JsonOption.DefaultSerializer);
        }
    }
}
