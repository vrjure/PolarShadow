using PolarShadow.Core;
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
        public Func<PolarShadowDbContext> DbCreater { get; set; }
        public IPolarShadowProvider Build(IPolarShadowBuilder builder)
        {
            return new DbConfigurationProvider(this);
        }

        public void Save(Stream content)
        {
            content.Seek(0, SeekOrigin.Begin);
            using var doc = JsonDocument.Parse(content);
            using var context = DbCreater();
            var manager = new SiteManager(context);
            manager.SaveJson(doc.RootElement);
        }

        public async Task SaveAsync(Stream content)
        {
            content.Seek(0, SeekOrigin.Begin);
            using var doc = JsonDocument.Parse(content);
            using var context = DbCreater();
            var manager = new SiteManager(context);
            await manager.SaveJsonAsync(doc.RootElement);
        }

    }
}
