using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PolarShadow.Core
{
    internal class AnalysisAbilityBuilder : IAnalysisAbilityBuilder
    {
        private readonly AnalysisAbility _ability;
        private readonly IPolarShadowOptionBuilder _optionBuilder;
        private readonly IParametersBuilder _parametersBuilder;

        public AnalysisAbilityBuilder(AnalysisAbility ability, IPolarShadowOptionBuilder optionBuilder)
        {
            _ability = ability;
            _optionBuilder = optionBuilder;
            _parametersBuilder = new ParametersBuilder(_ability.Parameters ??= new Dictionary<string, object>(), optionBuilder);
        }

        public IParametersBuilder Parameters => _parametersBuilder;

        public string GetRequestAsString()
        {
            if (_ability.Request == null)
            {
                return default;
            }
            return JsonSerializer.Serialize(_ability.Request, JsonOption.DefaultSerializer);
        }

        public string GetResponseAsString()
        {
            if (_ability.Response == null)
            {
                return default;
            }
            return JsonSerializer.Serialize(_ability.Response, JsonOption.DefaultSerializer);
        }

        public void Load(string config)
        {
            var ability = JsonSerializer.Deserialize<AnalysisAbility>(config, JsonOption.DefaultSerializer);
            _ability.Request = ability.Request;
            _ability.Response = ability.Response;
            _ability.Parameters = ability.Parameters;
            _ability.Next = ability.Next;
            _optionBuilder.ChangeNodify();
        }

        public IAnalysisAbilityBuilder Next()
        {
            return new AnalysisAbilityBuilder(new AnalysisAbility(), _optionBuilder);
        }

        public IAnalysisAbilityBuilder SetRequest(string url, string body = null, HttpMethod method = null, Dictionary<string, string> headers = null)
        {
            _ability.Request ??= new AnalysisRequest();
            _ability.Request.Url = url;
            _ability.Request.Method = method?.Method;
            _ability.Request.Headers = headers;
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IAnalysisAbilityBuilder SetRequest(string request)
        {
            var req = JsonSerializer.Deserialize<AnalysisRequest>(request, JsonOption.DefaultSerializer);
            _ability.Request = req;
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IAnalysisAbilityBuilder SetRequest(AnalysisRequest request)
        {
            throw new NotImplementedException();
        }

        public IAnalysisAbilityBuilder SetResponse(string template, Encoding encoding = null)
        {
            _ability.Response ??= new AnalysisResponse();
            if (encoding != null)
            {
                _ability.Response.Encoding = encoding.EncodingName;
            }

            using var doc = JsonDocument.Parse(template);
            _ability.Response.Template = doc.RootElement.Clone();

            _optionBuilder.ChangeNodify();
            return this;
        }

        public IAnalysisAbilityBuilder SetResponse(string response)
        {
            var res = JsonSerializer.Deserialize<AnalysisResponse>(response, JsonOption.DefaultSerializer);
            _ability.Response = res;
            _optionBuilder.ChangeNodify();
            return this;
        }

        public IAnalysisAbilityBuilder SetResponse(AnalysisResponse response)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(Stream output)
        {
            using var jsonWriter = new Utf8JsonWriter(output, JsonOption.DefaultWriteOption);
            var doc = JsonDocument.Parse(JsonSerializer.Serialize(_ability, JsonOption.DefaultSerializer));
            doc.WriteTo(jsonWriter);
            jsonWriter.Flush();
        }
    }
}
