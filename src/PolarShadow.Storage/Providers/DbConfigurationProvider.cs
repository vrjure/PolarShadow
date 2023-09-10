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
            using var context = _source.DbCreater();
            var manager = new SiteManager(context);
            Root = manager.GetAsJson();
        }

        public async Task LoadAsync()
        {
            using var context = _source.DbCreater();
            var manager = new SiteManager(context);
            Root = await manager.GetAsJsonAsync().ConfigureAwait(false);
        }

    }
}
