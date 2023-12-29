using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class DbConfigurationProvider : IPolarShadowProvider
    {
        private readonly DbConfigurationSource _source;
        public DbConfigurationProvider(DbConfigurationSource source)
        {
            _source = source;
        }
        public JsonElement Root { get; private set; }

        public void Load()
        {
            var manager = new SiteService(_source.DbContextFactroy);
            Root = Convert(manager.GetSiteInfo());
        }

        public async Task LoadAsync()
        {
            var manager = new SiteService(_source.DbContextFactroy);
            Root = Convert(await manager.GetSiteInfoAsync());
        }


        private static JsonElement Convert(ICollection<SiteInfoModel> sites)
        {
            if (sites.Count == 0)
            {
                return default;
            }

            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms, JsonOption.DefaultWriteOption);
            jsonWriter.WriteStartObject();

            jsonWriter.WriteStartArray("sites");

            foreach (var item in sites)
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("name", item.Site.Name);
                jsonWriter.WriteString("domain", item.Site.Domain);
                jsonWriter.WriteString("ico", item.Site.Ico);
                if (!string.IsNullOrEmpty(item.Site.Parameters))
                {
                    jsonWriter.WritePropertyName("parameters");
                    jsonWriter.WriteRawValue(item.Site.Parameters);
                }

                jsonWriter.WriteStartObject("requests");
                foreach (var request in item.Requests)
                {
                    jsonWriter.WriteStartObject(request.Name);

                    if (!string.IsNullOrEmpty(request.Parameters))
                    {
                        jsonWriter.WritePropertyName("parameters");
                        jsonWriter.WriteRawValue(request.Parameters);
                    }

                    if (!string.IsNullOrEmpty(request.Request))
                    {
                        jsonWriter.WritePropertyName("request");
                        jsonWriter.WriteRawValue(request.Request);
                    }

                    jsonWriter.WritePropertyName("response");
                    jsonWriter.WriteRawValue(request.Response);

                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();

            jsonWriter.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            using var doc = JsonDocument.Parse(ms);
            return doc.RootElement.Clone();
        }
    }
}
