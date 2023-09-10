using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class SiteManager
    {
        private PolarShadowDbContext _context;
        public SiteManager(PolarShadowDbContext context)
        {
            _context = context;
        }

        public async Task<JsonElement> GetAsJsonAsync()
        {
            var sites = await _context.Sites.OrderBy(f => f.Name).AsNoTracking().ToListAsync().ConfigureAwait(false);
            var requests = await _context.Requests.OrderBy(f => f.SiteName).AsNoTracking().ToListAsync().ConfigureAwait(false);

            return Convert(sites, requests);
        }

        public JsonElement GetAsJson()
        {
            var sites =  _context.Sites.OrderBy(f => f.Name).AsNoTracking().ToList();
            var requests = _context.Requests.OrderBy(f => f.SiteName).AsNoTracking().ToList();

            return Convert(sites, requests);
        }

        public async Task SaveJsonAsync(JsonElement root)
        {
            var sites = new List<SiteEntity>();
            var requests = new List<RequestEntity>();
            if (!Convert(sites, requests, root))
            {
                throw new InvalidOperationException("Invalid data");
            }

            await _context.Sites.ExecuteDeleteAsync();
            await _context.Requests.ExecuteDeleteAsync();

            _context.Sites.AddRange(sites);
            _context.Requests.AddRange(requests);
            await _context.SaveChangesAsync();
        }

        public void SaveJson(JsonElement root)
        {
            var sites = new List<SiteEntity>();
            var requests = new List<RequestEntity>();
            if (!Convert(sites, requests, root))
            {
                throw new InvalidOperationException("Invalid data");
            }

            _context.Sites.ExecuteDelete();
            _context.Requests.ExecuteDelete();

            _context.Sites.AddRange(sites);
            _context.Requests.AddRange(requests);
            _context.SaveChanges();
        }

        private JsonElement Convert(ICollection<SiteEntity> sites, List<RequestEntity> requests)
        {
            if (sites.Count == 0)
            {
                return default;
            }

            using var ms = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(ms, JsonOption.DefaultWriteOption);
            jsonWriter.WriteStartObject();

            jsonWriter.WriteStartArray("sites");

            var j = 0;
            foreach (var item in sites)
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("name", item.Name);
                jsonWriter.WriteString("domain", item.Domain);

                jsonWriter.WriteStartObject("requests");
                for (int i = j; i < requests.Count; i++)
                {
                    j = i;
                    var request = requests[i];
                    if (string.Compare(item.Name, request.SiteName, true) != 0) break;

                    jsonWriter.WriteStartObject(request.Name);
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

        private bool Convert(ICollection<SiteEntity> sites, ICollection<RequestEntity> requests, JsonElement root)
        {
            if (!root.TryGetProperty("sites", out JsonElement sitesSection) || sitesSection.ValueKind != JsonValueKind.Array) return false;

            var siteArray = sitesSection.EnumerateArray();
            foreach (var site in siteArray)
            {
                if(!site.TryGetProperty("name", out JsonElement siteElement) || siteElement.ValueKind != JsonValueKind.String)
                {
                    continue;
                }
                var siteName = siteElement.GetString();
                var domain = string.Empty;
                if (site.TryGetProperty("domain", out JsonElement domainElement) || domainElement.ValueKind == JsonValueKind.String)
                {
                    domain = domainElement.GetString();
                }
                sites.Add(new SiteEntity
                {
                    Name = siteName,
                    Domain = domain
                });

                if (!site.TryGetProperty("requests", out JsonElement requestElement) || requestElement.ValueKind != JsonValueKind.Object) break;

                var requestArray = requestElement.EnumerateObject();
                foreach (var item in requestArray)
                {
                    var requestName = item.Name;
                    if (item.Value.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    var reqEntity = new RequestEntity()
                    {
                        Name = requestName,
                        SiteName = siteName
                    };
                    foreach (var request in item.Value.EnumerateObject())
                    {
                        if (request.Name.Equals("request", StringComparison.OrdinalIgnoreCase))
                        {
                            reqEntity.Request = request.Value.GetRawText();
                        }

                        if (request.Name.Equals("response", StringComparison.OrdinalIgnoreCase))
                        {
                            reqEntity.Response = request.Value.GetRawText();
                        }
                    }

                    requests.Add(reqEntity);
                }
            }

            return true;
        }
    }
}
