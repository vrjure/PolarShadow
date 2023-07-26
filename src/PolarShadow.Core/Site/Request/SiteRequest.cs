﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Core
{
    internal class SiteRequest : ISiteRequest
    {
        public AnalysisRequest Request { get; set; }
        public AnalysisResponse Response { get; set; }
        public IKeyValueParameter Parameter { get; set; }

        public bool? UseWebView { get; set; }

        public void LoadFrom(IPolarShadowSource source)
        {
            var provider = source.Build(null);
            if (provider != null || provider.Root.ValueKind != JsonValueKind.Object) return;
            var request = JsonSerializer.Deserialize<SiteRequest>(provider.Root, JsonOption.DefaultSerializer);

            this.Request = request.Request;
            this.Response = request.Response;
            this.Parameter = request.Parameter;
            this.UseWebView = request.UseWebView;
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            JsonSerializer.Serialize(writer, this, JsonOption.DefaultSerializer);
        }
    }
}
