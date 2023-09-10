using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PolarShadow.Cache;
using PolarShadow.Core;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Storage;
using PolarShadow.Videos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class DetailViewModel : ViewModelBase, IParameterObtain
    {
        private readonly IPolarShadow _polar;
        private readonly INotificationManager _notify;
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        private readonly IBufferCache _bufferCache;

        public DetailViewModel(IPolarShadow polar, INotificationManager notify, IDbContextFactory<PolarShadowDbContext> dbFactory, IBufferCache bufferCache)
        {
            _polar = polar;
            _notify = notify;
            _dbFactory = dbFactory;
            _bufferCache = bufferCache;
        }

        public Resource ResourceParam { get; private set; }
        private ResourceEntity _dbResource;

        public IBufferCache ImageCache => _bufferCache;

        private DetailViewData _detail;
        public DetailViewData Detail
        {
            get => _detail;
            set => SetProperty(ref _detail, value);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isSave;
        public bool IsSaved
        {
            get => _isSave;
            set => SetProperty(ref _isSave, value);
        }

        private IAsyncRelayCommand _refreshCommand;
        public IAsyncRelayCommand RefreshCommand => _refreshCommand ??= new AsyncRelayCommand(LoadingDeatil);

        private IAsyncRelayCommand _resourceOperateCommand;
        public IAsyncRelayCommand ResourceOperateCommand => _resourceOperateCommand ??= new AsyncRelayCommand(ResourceOperate);

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if(parameters.TryGetValue(nameof(ResourceParam), out Resource r))
            {
                ResourceParam = r;
            }

            if (parameters.TryGetValue(nameof(Detail), out DetailViewData detail))
            {
                Detail = detail;
            }
        }

        public static DetailViewData ConvertToViewData(Resource r)
        {
            var viewData = new DetailViewData()
            {
                Name = r.Name,
                Image = r.ImageSrc,
                Description = r.Description,
                Site = r.Site,
                Tag = r
            };

            viewData.Episodes = new Dictionary<string, IEnumerable<NameViewData>>();
            foreach (var item in r.Episodes)
            {
                List<NameViewData> episodes = null;
                var key = item.Tag ?? "";
                if (!viewData.Episodes.TryGetValue("", out IEnumerable<NameViewData> value))
                {
                    value = episodes = new List<NameViewData>();
                    viewData.Episodes.Add("", value);
                }
                else
                {
                    episodes = value as List<NameViewData>;
                }

                episodes.Add(new NameViewData
                {
                    Name = item.Name,
                    Tag = item
                });
            }
            
            return viewData;
        }

        public override async void OnLoad()
        {
            if (Detail != null)
            {
                return;
            }

            await LoadingDeatil();
            IsSaved = await CheckSaveStatus();
        }

        private async Task LoadingDeatil()
        {
            if (ResourceParam == null)
            {
                _notify.Show("Missing parameters", NotificationType.Warning);
                return;
            }

            if(!_polar.TryGetSite(ResourceParam.Site, out ISite site))
            {
                _notify.Show($"Can not fond Site [{ResourceParam.Site}]", NotificationType.Warning);
                return;
            }

            IsLoading = true;
            try
            {
                var result = await site.GetDetailAsync(ResourceParam);
                if (result == null)
                {
                    _notify.Show("No Data");
                    return;
                }

                Detail = ConvertToViewData(result);
                if (IsSaved)
                {
                    await SaveResource();
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
            IsLoading = false;
        }

        private async Task<bool> CheckSaveStatus()
        {
            if (ResourceParam == null)
            {
                return false;
            }

            using var context = _dbFactory.CreateDbContext();
            var resourceManager = new ResourceManager(context);
            _dbResource = await resourceManager.GetResourceAsync(ResourceParam.Name);
            return _dbResource != null;
        }

        private async Task ResourceOperate()
        {
            if (IsSaved)
            {
                await DeleteResource();
            }
            else
            {
                await SaveResource();
            }
        }

        private async Task SaveResource()
        {
            try
            {
                var detail = Detail.Tag as Resource;

                var resource = detail.ToResourceEntity();

                var episodes = new List<KeyValuePair<EpisodeEntity, IEnumerable<LinkEntity>>>();

                foreach (var episode in detail.Episodes)
                {
                    episodes.Add(new KeyValuePair<EpisodeEntity, IEnumerable<LinkEntity>>(episode.ToEpisodeEntity(), episode.Links.Select(f => f.ToLinkEntity()).ToList()));
                }

                using var context = _dbFactory.CreateDbContext();
                var resourceManger = new ResourceManager(context);
                if (_dbResource != null)
                {
                    await resourceManger.DeleteResourceAsync(_dbResource.Id);
                }
                await resourceManger.SaveResourceAsync(resource, episodes);
                _dbResource = resource;
                IsSaved = true;

                if (!string.IsNullOrEmpty(detail.ImageSrc))
                {
                    await _bufferCache.CacheFileIfExisedInMemory(BufferCache.SHA(detail.ImageSrc));
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        private async Task DeleteResource()
        {
            try
            {
                if (_dbResource == null)
                {
                    return;
                }
                using var context = _dbFactory.CreateDbContext();
                var resourceManger = new ResourceManager(context);
                await resourceManger.DeleteResourceAsync(_dbResource.Id);

                if (!string.IsNullOrEmpty(_dbResource.ImageSrc))
                {
                    _bufferCache.Remove(BufferCache.SHA(_dbResource.ImageSrc), BufferLocation.File);
                }

                _dbResource = null;
                IsSaved = false;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }
    }
}
