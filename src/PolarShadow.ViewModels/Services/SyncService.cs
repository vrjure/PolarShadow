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

                var remoteData = await remoteResourceService.DownloadAsync();
                var localData = await dbResourceService.DownloadAsync();

                await dbResourceService.UploadAsync(remoteData);
                await remoteResourceService.UploadAsync(localData);
                
            }
            else if (_type == typeof(HistoryModel))
            {
                var dbHistoryService = _serviceProvider.GetRequiredService<IDbHistoryService>();
                var remoteHistoryService = _serviceProvider.GetRequiredService<IHttpHistoryService>();

                var remoteData = await remoteHistoryService.DownloadAsync();
                var localData = await dbHistoryService.DownloadAsync();

                await dbHistoryService.UploadAsync(remoteData);
                await remoteHistoryService.UploadAsync(localData);
            }
            else if (_type == typeof(SourceModel))
            {
                var dbSoureService = _serviceProvider.GetRequiredService<IDbSourceService>();
                var remoteSourceService = _serviceProvider.GetRequiredService<IHttpSourceService>();

                var remoteData = await remoteSourceService.DownloadAsync();
                var locaData = await dbSoureService.DownloadAsync();

                await dbSoureService.UploadAsync(remoteData);
                await remoteSourceService.UploadAsync(locaData);
            }
        }

        public async Task<ICollection<T>> DownloadAsync()
        {
            if (_type == typeof(ResourceModel))
            {
                var service = _serviceProvider.GetRequiredService<IHttpMineResourceService>();
                return (ICollection<T>)await service.DownloadAsync();
            }
            else if (_type == typeof(HistoryModel))
            {
                var service = _serviceProvider.GetRequiredService<IHttpHistoryService>();
                return (ICollection<T>)await service.DownloadAsync();
            }
            else if (_type == typeof(SourceModel))
            {
                var service = _serviceProvider.GetRequiredService<IHttpSourceService>();
                return (ICollection<T>)await service.DownloadAsync();
            }

            return null;
        }

        public async Task UploadAsync()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var data = await dbContext.Set<T>().ToListAsync();

            if (_type == typeof(ResourceModel))
            {
                var service = _serviceProvider.GetRequiredService<IHttpMineResourceService>();
                await service.UploadAsync(data as ICollection<ResourceModel>);
            }
            else if (_type == typeof(HistoryModel))
            {
                var service = _serviceProvider.GetRequiredService<IHttpHistoryService>();
                await service.UploadAsync(data as ICollection<HistoryModel>);
            }
            else if (_type == typeof(SourceModel))
            {
                var service = _serviceProvider.GetRequiredService<IHttpSourceService>();
                await service.UploadAsync(data as ICollection<SourceModel>);
            }
        }

        private static IEnumerable<ResourceModel> CreateEnumerator(ResourceModel root, IEnumerable<ResourceModel> children)
        {
            yield return root;
            foreach (var item in children)
            {
                yield return item;
            }
        }

    }
}
