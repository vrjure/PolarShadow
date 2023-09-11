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
using System.Collections.ObjectModel;
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

        public IBufferCache ImageCache => _bufferCache;

        private ResourceViewData _resource;
        public ResourceViewData Resource
        {
            get => _resource;
            set => SetProperty(ref _resource, value);
        }

        private ObservableCollection<GroupEpisodeViewData> _groupEpisode;
        public ObservableCollection<GroupEpisodeViewData> GroupEpisode
        {
            get => _groupEpisode;
            set => SetProperty(ref _groupEpisode, value);
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
            if (parameters.TryGetValue(nameof(Resource), out ResourceViewData detail))
            {
                Resource = detail;
            }
        }

        public override async void OnLoad()
        {
            await LoadingDeatil();
        }

        private async Task LoadingDeatil()
        {
            if (Resource == null)
            {
                _notify.Show("Missing parameters", NotificationType.Warning);
                return;
            }

            var savedResource = await GetSavedResource();
            if (savedResource != null)
            {
                IsSaved = true;
                await LoadLocal(savedResource);
            }
            else
            {
                await LoadOnline();
            }            
        }

        private async Task LoadLocal(ResourceEntity resource)
        {
            this.Resource = new ResourceViewData
            {
                Image = resource.ImageSrc,
                Description = resource.Description,
                Site = resource.Site,
                Text = resource.Name,
                Data = resource
            };

            using var dbContext = _dbFactory.CreateDbContext();
            var manager = new ResourceManager(dbContext);
            var episodes = await manager.GetEpisodesAsync(resource.Id);
            var links = await manager.GetLinksAsync(resource.Id);
            var groupEpisode = episodes.GroupBy(f => f.Tag ?? "").Select(f => new GroupEpisodeViewData
            {
                Text = f.Key,
                Episodes = f.Select(e => e.ToEpisodeViewData(links)).ToList()
            });
            this.GroupEpisode = new ObservableCollection<GroupEpisodeViewData>(groupEpisode);
        }

        private async Task LoadOnline()
        {
            if (Resource == null)
            {
                return;
            }

            if (!_polar.TryGetSite(Resource.Site, out ISite site))
            {
                _notify.Show($"Can not fond Site [{Resource.Site}]", NotificationType.Warning);
                return;
            }

            IsLoading = true;
            try
            {
                ILink link = null;
                if (Resource.Data is Core.Resource r)
                {
                    link = r;
                }
                else if (Resource.Data is ResourceEntity re)
                {
                    link = re.ToResource();
                }

                var result = await site.GetDetailAsync(link);
                if (result == null)
                {
                    _notify.Show("No Data");
                    return;
                }

                Resource = result.ToResourceViewData();

                if (result.Episodes != null)
                {
                    var ge = result.Episodes.GroupBy(f => f.Tag ?? "").Select(f => new GroupEpisodeViewData
                    {
                        Text = f.Key,
                        Episodes = f.Select(e => e.ToEpisodeViewData()).ToList()
                    });

                    this.GroupEpisode = new ObservableCollection<GroupEpisodeViewData>(ge);
                }

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

        private async Task<ResourceEntity> GetSavedResource()
        {
            if (Resource == null)
            {
                return null;
            }

            using var context = _dbFactory.CreateDbContext();
            var resourceManager = new ResourceManager(context);
            return await resourceManager.GetResourceAsync(Resource.Text);
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
                ResourceEntity resource = Resource.ToResourceEntityFromData();

                var episodes = new List<KeyValuePair<EpisodeEntity, IEnumerable<LinkEntity>>>();

                if (GroupEpisode != null)
                {
                    foreach (var ge in GroupEpisode)
                    {
                        foreach (var item in ge.Episodes)
                        {
                            EpisodeEntity ee = item.ToEpisodeEntityFromData();
                            var links = new List<LinkEntity>();
                            if (item.Links != null)
                            {
                                foreach (var link in item.Links)
                                {
                                    var linkEntity = link.ToLinkEntityFromData();
                                    if (linkEntity != null)
                                    {
                                        links.Add(link.ToLinkEntityFromData());
                                    }
                                }
                            }
                            episodes.Add(new KeyValuePair<EpisodeEntity, IEnumerable<LinkEntity>>(ee, links));
                        }
                    }
                }
                

                using var context = _dbFactory.CreateDbContext();
                var resourceManger = new ResourceManager(context);
                var savedResource = await resourceManger.GetResourceAsync(resource.Name);
                if (savedResource != null)
                {
                    await resourceManger.DeleteResourceAsync(savedResource.Id);
                    resource.Id = savedResource.Id;
                }
                await resourceManger.SaveResourceAsync(resource, episodes);
                IsSaved = true;

                if (!string.IsNullOrEmpty(resource.ImageSrc))
                {
                    await _bufferCache.CacheFileIfExisedInMemory(BufferCache.SHA(resource.ImageSrc));
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
                using var context = _dbFactory.CreateDbContext();
                var resourceManger = new ResourceManager(context);
                var savedResource = await resourceManger.GetResourceAsync(Resource.Text);
                if (savedResource == null)
                {
                    IsSaved = false;
                    return;
                }

                await resourceManger.DeleteResourceAsync(savedResource.Id);

                if (!string.IsNullOrEmpty(savedResource.ImageSrc))
                {
                    _bufferCache.Remove(BufferCache.SHA(savedResource.ImageSrc), BufferLocation.File);
                }
                IsSaved = false;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }
    }
}
