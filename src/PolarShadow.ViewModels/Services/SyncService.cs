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
        private readonly IMessageService _messageService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Type _type;

        public SyncService(IDbContextFactory<PolarShadowDbContext> dbFactory, IMessageService messageService, IServiceProvider serviceProvider)
        {
            _dbFactory = dbFactory;
            _type = typeof(T);
            _serviceProvider = serviceProvider;
            _messageService = messageService;
        }
        public async Task UploadAsync()
        {
            using var dbContext = _dbFactory.CreateDbContext();
            var data = await dbContext.Set<T>().ToListAsync();

            try
            {
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
            catch (Exception ex)
            {
                _messageService.Show(ex);
            }
        }
    }
}
