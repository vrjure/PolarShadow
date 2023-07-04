using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class PolarShadowSiteOptionBuilder : IPolarShadowSiteOptionBuilder
    {
        private readonly PolarShadowSiteOption _option;
        private readonly IPolarShadowOptionBuilder _optionBuilder;
        private IParametersBuilder _parameterBuilder;
        public PolarShadowSiteOptionBuilder(PolarShadowSiteOption siteOption, IPolarShadowOptionBuilder optionBuilder)
        {
            _option = siteOption;
            _optionBuilder = optionBuilder;
            _parameterBuilder = new ParametersBuilder(_option.Parameters ??= new Dictionary<string, object>(), optionBuilder);
        }

        public IParametersBuilder Parameters => _parameterBuilder;

        public string Domain { get => _option.Domain; set => _option.Domain = value; }
        public bool Enable { get => _option.Enable; set =>_option.Enable = value; }
        public bool UseWebView { get => _option.UseWebView; set => _option.UseWebView = value; }

        public IPolarShadowSiteOptionBuilder BuildAbility(string name, Action<IAnalysisAbilityBuilder> abilityBuilder)
        {
            _option.Abilities ??= new Dictionary<string, AnalysisAbility>();
            if(!_option.Abilities.TryGetValue(name, out AnalysisAbility ability))
            {
                ability = new AnalysisAbility();
                _option.Abilities.Add(name, ability);
            }

            abilityBuilder(new AnalysisAbilityBuilder(ability, _optionBuilder));
            _optionBuilder.ChangeNodify();
            return this;
        }

        public void Load(string config)
        {
            var siteOption = JsonSerializer.Deserialize<PolarShadowSiteOption>(config, JsonOption.DefaultSerializer);
            _option.Apply(siteOption);
            _optionBuilder.ChangeNodify();
        }

        public IPolarShadowSiteOptionBuilder RemoveAbility(string name)
        {
            _option.Abilities?.Remove(name);
            _optionBuilder.ChangeNodify();
            return this;
        }

        public void WriteTo(Stream output)
        {
            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(_option, JsonOption.DefaultSerializer));
            using var jsonWriter = new Utf8JsonWriter(output, JsonOption.DefaultWriteOption);
            doc.WriteTo(jsonWriter);
            jsonWriter.Flush();
        }
    }
}
