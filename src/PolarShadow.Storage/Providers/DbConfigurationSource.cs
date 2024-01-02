using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Storage
{
    public class DbConfigurationSource : IPolarShadowSource
    {
        public IDbContextFactory<PolarShadowDbContext> DbContextFactroy { get; set; }
        public IPolarShadowProvider Build(IPolarShadowBuilder builder)
        {
            return new DbConfigurationProvider(this);
        }

        public void Save(Stream content)
        {
            content.Seek(0, SeekOrigin.Begin);
            using var doc = JsonDocument.Parse(content);
            var infos = new List<SiteInfoModel>();
            if (!CreateSiteInfos(infos, doc.RootElement))
            {
                throw new InvalidOperationException("Invalid data");
            }
            var siteService = new SiteService(DbContextFactroy);
            siteService.Save(infos);
        }

        public async Task SaveAsync(Stream content)
        {
            content.Seek(0, SeekOrigin.Begin);
            using var doc = JsonDocument.Parse(content);
            var infos = new List<SiteInfoModel>();
            if (!CreateSiteInfos(infos, doc.RootElement))
            {
                throw new InvalidOperationException("Invalid data");
            }
            var siteService = new SiteService(DbContextFactroy);
            await siteService.SaveAsync(infos);
        }

        private static bool CreateSiteInfos(ICollection<SiteInfoModel> info, JsonElement root)
        {
            if (!root.TryGetProperty("sites", out JsonElement sitesSection) || sitesSection.ValueKind != JsonValueKind.Array) return false;

            var siteArray = sitesSection.EnumerateArray();
            foreach (var site in siteArray)
            {
                if (!site.TryGetProperty("name", out JsonElement siteElement) || siteElement.ValueKind != JsonValueKind.String)
                {
                    continue;
                }
                var siteName = siteElement.GetString();
                var domain = string.Empty;
                var parameters = string.Empty;
                var ico = string.Empty;
                if (site.TryGetProperty("domain", out JsonElement domainElement) && domainElement.ValueKind == JsonValueKind.String)
                {
                    domain = domainElement.GetString();
                }

                if (site.TryGetProperty("icon", out JsonElement icoElement) && domainElement.ValueKind == JsonValueKind.String)
                {
                    ico = icoElement.GetString();
                }

                if (site.TryGetProperty("parameters", out JsonElement parameterElement) && parameterElement.ValueKind == JsonValueKind.Object)
                {
                    parameters = parameterElement.GetRawText();
                }

                info.Add(new SiteInfoModel
                {
                    Site = new SiteModel
                    {
                        Name = siteName,
                        Domain = domain,
                        Ico = ico,
                        Parameters = parameters
                    }
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

                    var reqEntity = new RequestModel()
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
                        if (request.Name.Equals("parameters", StringComparison.OrdinalIgnoreCase))
                        {
                            reqEntity.Parameters = request.Value.GetRawText();
                        }
                    }

                    var last = info.Last();
                    last.Requests ??= new List<RequestModel>();
                    last.Requests.Add(reqEntity);
                }
            }

            return true;
        }
    }
}
