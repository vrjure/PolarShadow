using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Aria2;
using PolarShadow.Cache;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Options;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class DetailViewModel : ViewModelBase, IParameterObtain
    {
        private readonly IPolarShadow _polar;
        private readonly INotificationManager _notify;
        private readonly IMineResourceService _mineResourceService;
        private readonly IBufferCache _bufferCache;
        private readonly IPreference _preference;
        private readonly INavigationService _nav;
        private readonly IHistoryService _hisService;

        private ResourceModel _rootResourceInDb;
        
        public DetailViewModel(IPolarShadow polar, INotificationManager notify, IMineResourceService mineResourceService, IBufferCache bufferCache, IPreference preference, INavigationService nav, IHistoryService hisService)
        {
            _polar = polar;
            _notify = notify;
            _mineResourceService = mineResourceService;
            _bufferCache = bufferCache;
            _preference = preference;
            _nav = nav;
            _hisService = hisService;
        }

        public ILink Param_Link { get; set; }

        private ResourceTreeNode _resource;
        public ResourceTreeNode Resource
        {
            get => _resource;
            private set
            {
                if (SetProperty(ref _resource, value))
                {
                    SelectHeaderDefault();
                }
            }
        }

        private ISelectionModel _headerSelection;
        public ISelectionModel HeaderSelection
        {
            get => _headerSelection;
            set => SetProperty(ref _headerSelection, value);
        }

        private ICollection<ISite> _webAnalysisSites;
        public ICollection<ISite> WebAnalysisSites
        {
            get => _webAnalysisSites;
            private set => SetProperty(ref _webAnalysisSites, value);
        }

        private bool _isSave;
        public bool IsSaved
        {
            get => _isSave;
            private set => SetProperty(ref _isSave, value);
        }

        private HistoryModel _history;
        public HistoryModel History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private ICollection<ResourceTreeNode> _flyoutOptions;
        public ICollection<ResourceTreeNode> FlyoutOptions
        {
            get => _flyoutOptions;
            set => SetProperty(ref _flyoutOptions, value);
        }

        private IAsyncRelayCommand _refreshCommand;
        public IAsyncRelayCommand RefreshCommand => _refreshCommand ??= new AsyncRelayCommand(LoadOnline);

        private IAsyncRelayCommand _resourceOperateCommand;
        public IAsyncRelayCommand ResourceOperateCommand => _resourceOperateCommand ??= new AsyncRelayCommand(ResourceOperate);

        private IAsyncRelayCommand _linkClickCommand;
        public IAsyncRelayCommand LinkClickCommand => _linkClickCommand ??= new AsyncRelayCommand<ResourceTreeNode>(LinkClick);

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(nameof(Param_Link), out ILink link))
            {
                Param_Link = link;
            }
        }

        public ICollection<ISite> UpdateWebAnalysisSites()
        {
            return WebAnalysisSites = _polar.GetWebAnalysisSites().ToList();
        }

        protected override async void OnLoad()
        {
            if (this.Resource == null)
            {
                await LoadingDeatil();
            }
            else
            {
                History = await _hisService.GetByResourceIdAsync(this.Resource.Id);
            }
        }

        private async Task LoadingDeatil()
        {
            if (Param_Link == null)
            {
                _notify.Show("Missing parameters", NotificationType.Warning);
                return;
            }

            _rootResourceInDb = await _mineResourceService.GetRootResourceAsync(Param_Link.Name, Param_Link.Site);
            if (_rootResourceInDb != null)
            {
                IsSaved = true;
                await LoadLocal(_rootResourceInDb);

                History = await _hisService.GetByResourceIdAsync(_rootResourceInDb.Id);
            }
            else
            {
                await LoadOnline();
                History = null;
            }
        }

        private void SelectHeaderDefault()
        {
            if (this.Resource?.Children?.Count > 0)
            {
                HeaderSelection.Select(0);
            }
        }
        private async Task LoadLocal(ResourceModel root)
        {
            var children = await _mineResourceService.GetRootChildrenAsync(root.Id);
            if (children.Count == 0)
            {
                return;
            }

            var list = children as List<ResourceModel>;
            if (list != null)
            {
                list.Insert(0, root);
            }
            else if (list == null)
            {
                list = new List<ResourceModel>(children.Count + 1);
                list.Add(root);
                list.AddRange(children);
            }

            var tree = children.BuildTree();
            this.Resource = tree;
        }

        private async Task LoadOnline()
        {
            if (!_polar.TryGetSite(Param_Link.Site, out ISite site))
            {
                _notify.Show($"Can not fond site [{Param_Link.Site}]", NotificationType.Warning);
                return;
            }

            IsLoading = true;
            try
            {
                var result = await site.ExecuteAsync<ResourceTree>(_polar, this.Param_Link, Cancellation.Token);
                if (result == null)
                {
                    _notify.Show("No Data");
                    return;
                }

                this.Resource = result.ToResourceTreeNode();
                if (_rootResourceInDb != null)
                {
                    this.Resource.Id = _rootResourceInDb.Id;
                }

                if (IsSaved)
                {
                    await SaveResource();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
            IsLoading = false;
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
                await _mineResourceService.SaveResourceAsync(this.Resource);

                IsSaved = true;

                if (!string.IsNullOrEmpty(this.Resource.ImageSrc))
                {
                    await _bufferCache.CacheFileIfExisedInMemory(BufferCache.SHA(this.Resource.ImageSrc));
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
                await _mineResourceService.DeleteRootResourceAsync(this.Resource.Id);

                if (!string.IsNullOrEmpty(this.Resource.ImageSrc))
                {
                    _bufferCache.Remove(BufferCache.SHA(this.Resource.ImageSrc), BufferLocation.File);
                }
                IsSaved = false;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        protected override async void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                var resource = e.SelectedItems[0] as ResourceTreeNode;
                if (string.IsNullOrEmpty(resource.Src))
                {
                    FlyoutOptions = resource.Children;
                }
                else
                {
                    await LinkClick(resource);
                }
            }
        }

        private async Task LinkClick(ResourceTreeNode node)
        {
            try
            {
                if (string.IsNullOrEmpty(node?.Src))
                {
                    _notify.Show($"Invalid Src");
                    return;
                }
                switch (node.SrcType)
                {
                    case LinkType.Magnet:
                        await StartMagnet(node);
                        break;
                    case LinkType.HtmlSource:
                        var url = await TryAnalysisVideoUrl(node);
                        await ToPlayVideo(url);
                        break;
                    case LinkType.WebAnalysisSource:
                        FlyoutOptions = _polar.GetWebAnalysisSites().Select(f => new ResourceTreeNode
                        {
                            Name = f.Title,
                            Src = node.Src,
                            SrcType = LinkType.HtmlSource,
                            Site = f.Name
                        }).ToList();
                        break;
                    default:
                        _notify.Show($"Not support type:{node.SrcType}"); 
                        break;
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        private async Task StartMagnet(ResourceTreeNode node)
        {
            var rpc = await _preference.GetAsync(PreferenceOption.RPC, string.Empty);
            var downloadFolder = await _preference.GetAsync(PreferenceOption.DownloadPath, string.Empty);
            if (string.IsNullOrEmpty(rpc) || string.IsNullOrEmpty(downloadFolder))
            {
                _notify.Show("Aria2 configuration information is incomplete");
                return;
            }

            try
            {
                using (var aria2 = new Aria2Http(new Uri(rpc)))
                {
                    var request = aria2.CreateAddUri(new string[] { node.Src }, new Aria2InputFileOption
                    {
                        Dir = @$"{downloadFolder}\{Resource.Name}"
                    });

                    var result = await aria2.PostAsync(request);
                    if (result.IsOk())
                    {
                        _notify.Show($"{Resource.Name} downdload started");
                    }
                    else if (result.IsError())
                    {
                        _notify.Show(result.Error.Message, NotificationType.Error);
                    }
                    else
                    {
                        _notify.Show(result.Result);
                    }
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        private async Task<ILink> TryAnalysisVideoUrl(ILink link, ISite site = default)
        {
            try
            {
                IsLoading = true;

                if (site == null && !_polar.TryGetSite(link.Site, out site))
                {
                    _notify.Show($"Can not found site [{link.Site}]");
                    return null;
                }

                var requestName = link.Request;
                if (string.IsNullOrEmpty(requestName))
                {
                    requestName = Requests.Video;
                }

                if (!site.TryGetRequest(requestName, out ISiteRequest request))
                {
                    return null;
                }

                return await site.ExecuteAsync<ILink>(_polar, requestName, builder =>
                {
                    builder.AddObjectValue(link);
                }, Cancellation.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
            finally
            {
                IsLoading = false;
            }
            return null;
        }

        private async Task ToPlayVideo(ILink videoSource)
        {
            if (videoSource == null || string.IsNullOrEmpty(videoSource.Src))
            {
                _notify.Show("Invaild resource");
                return;
            }

            var episode = Param_Link?.Name;
            if (SelectionModel.SelectedItem != null)
            {
                episode = (SelectionModel.SelectedItem as ResourceTreeNode).Name;
            }

            _nav.Navigate<VideoPlayerViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                {nameof(VideoPlayerViewModel.Param_Episode), videoSource },
                {nameof(VideoPlayerViewModel.Param_Title), $"{this.Resource.Name}-{episode}" }
            }, true);

            var second = HeaderSelection.SelectedItem as ResourceTreeNode;
            var third = SelectionModel.SelectedItem as ResourceTreeNode;

            await TryUpdateHistory(this.Resource, second, third);
        }

        private async Task TryUpdateHistory(ResourceTreeNode root, ResourceTreeNode second, ResourceTreeNode third)
        {
            if (root == null || second == null || third == null)
            {
                return;
            }

            var his = History;
            var desc = $"{root.Name}-{second.Name}-{third.Name}";
            if (his == null)
            {
                his = new HistoryModel
                {
                    ResourceId = root.Id,
                    ProgressDesc = desc
                };
            }
            else
            {
                his.ProgressDesc = desc;
            }

            await _hisService.AddOrUpdateAsync(his);
        }
    }
}
