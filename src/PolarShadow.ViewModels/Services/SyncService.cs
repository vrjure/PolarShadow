using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PolarShadow.Essentials;
using PolarShadow.Services;
using PolarShadow.Services.Http;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels.Services
{
    internal class SyncService<T> : ISyncService<T> where T : class
    {
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Type _type;

        public SyncService(IDbContextFactory<PolarShadowDbContext> dbFactory, IServiceProvider serviceProvider)
        {
            _dbFactory = dbFactory;
            _type = typeof(T);
            _serviceProvider = serviceProvider;
        }

        public async Task SyncAsync()
        {
            if (_type == typeof(ResourceModel))
            {
                var dbResourceService = _serviceProvider.GetRequiredService<IDbMineResourceService>();
                var remoteResourceService = _serviceProvider.GetRequiredService<IHttpMineResourceService>();

                var dbLastTime = (await dbResourceService.GetLastTimeAsync()).ToUniversalTime();
                var remoteLastTime = (await remoteResourceService.GetLastTimeAsync()).ToUniversalTime();

                if (dbLastTime > remoteLastTime)
                {
                    var localData = await dbResourceService.DownloadAsync(remoteLastTime);
                    await remoteResourceService.UploadAsync(localData);
                }
                else if (remoteLastTime > dbLastTime)
                {
                    var remoteData = await remoteResourceService.DownloadAsync(dbLastTime);
                    await dbResourceService.UploadAsync(remoteData);
                }
            }
            else if (_type == typeof(HistoryModel))
            {
                var dbHistoryService = _serviceProvider.GetRequiredService<IDbHistoryService>();
                var remoteHistoryService = _serviceProvider.GetRequiredService<IHttpHistoryService>();

                var dbLastTime = (await dbHistoryService.GetLastTimeAsync()).ToUniversalTime();
                var remoteLastTime = (await remoteHistoryService.GetLastTimeAsync()).ToUniversalTime();

                if (dbLastTime > remoteLastTime)
                {
                    var localData = await dbHistoryService.DownloadAsync(remoteLastTime);
                    await remoteHistoryService.UploadAsync(localData);
                }
                else if (remoteLastTime > dbLastTime)
                {
                    var remoteData = await remoteHistoryService.DownloadAsync(dbLastTime);
                    await dbHistoryService.UploadAsync(remoteData);
                }
            }
            else if (_type == typeof(SourceModel))
            {
                var dbSoureService = _serviceProvider.GetRequiredService<IDbSourceService>();
                var remoteSourceService = _serviceProvider.GetRequiredService<IHttpSourceService>();

                var dbLastTime = (await dbSoureService.GetLastTimeAsync()).ToUniversalTime();
                var remoteLastTime = (await remoteSourceService.GetLastTimeAsync()).ToUniversalTime();

                if (dbLastTime > remoteLastTime)
                {
                    var locaData = await dbSoureService.DownloadAsync(remoteLastTime);
                    await remoteSourceService.UploadAsync(locaData);

                }
                else if (remoteLastTime > dbLastTime)
                {
                    var remoteData = await remoteSourceService.DownloadAsync(dbLastTime);
                    await dbSoureService.UploadAsync(remoteData);
                }
            }
        }
    }
}
