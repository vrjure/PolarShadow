using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Aria2;
using PolarShadow.Cache;
using PolarShadow.Controls;
using PolarShadow.Core;
using PolarShadow.Essentials;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Options;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public partial class DetailViewModel : ViewModelBase, IParameterObtain
    {
        private readonly IPolarShadow _polar;
        private readonly INotificationManager _notify;
        private readonly IMineResourceService _mineResourceService;
        private readonly IBufferCache _bufferCache;
        private readonly IPreference _preference;
        private readonly INavigationService _nav;
        private readonly IHistoryService _hisService;
        private readonly ITopLevelService _topLevelService;

        private ResourceModel _rootResourceInDb;
        private TimeSpan _currentProgress;

        public DetailViewModel(IPolarShadow polar, INotificationManager notify, IMineResourceService mineResourceService, IBufferCache bufferCache, IPreference preference, INavigationService nav, IHistoryService hisService, ITopLevelService toplevelService)
        {
            _polar = polar;
            _notify = notify;
            _mineResourceService = mineResourceService;
            _bufferCache = bufferCache;
            _preference = preference;
            _nav = nav;
            _hisService = hisService;
            _topLevelService = toplevelService;
        }

        public ILink Param_Link { get; set; }   

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(nameof(Param_Link), out ILink link))
            {
                Param_Link = link;
            }
        }

        protected override async void OnLoad()
        {
            _topLevelService.PropertyChanged += _TopLevelService_PropertyChanged;
            if (this.Resource == null)
            {
                await LoadingDetail();
            }
        }

        protected override async void OnUnload()
        {
            _topLevelService.PropertyChanged -= _TopLevelService_PropertyChanged;
            if (MediaController.FullScreen)
            {
                MediaController.FullScreen = false;
            }

            try
            {
                await TryUpdateHistory();
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }


        private void _controller_TimeChanged(object sender, TimeSpan e)
        {
            _currentProgress = e;
        }

        private void Controller_MediaChanged(object sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(async () =>
            {
                var second = HeaderSelection.SelectedItem as ResourceTreeNode;
                var third = SelectionModel.SelectedItem as ResourceTreeNode;
                var thirdIndex = SelectionModel.SelectedIndex;

                if (History != null)
                {
                    var descItem = GetProgressDesc(History);
                    if (descItem.Item2 == third.Name || descItem.Item3 == thirdIndex)
                    {
                        MediaController.Controller.Time = _currentProgress;
                        return;
                    }
                }

                await TryUpdateHistory(this.Resource, second, third, thirdIndex, 0);
            });           
        }

        private async Task LoadingDetail()
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

            }
            else
            {
                await LoadOnline();
            }

            DetectSource();

            History = await _hisService.GetByResourceNameAsync(Param_Link.Name);
            if (History != null)
            {
                _currentProgress = TimeSpan.FromMilliseconds(History.Progress);
            }
        }

        private void DetectSource()
        {
            if (this.Resource == null || this.Resource.Children?.Count == 0 || this.Resource.Children.First().Children?.Count == 0) return;
            
            var first = this.Resource.Children.First().Children.First();
            if (string.IsNullOrEmpty(first.Src) && first.Children?.Count == 1)
            {
                first = first.Children.First();
            }

            if (first.SrcType == LinkType.WebAnalysisSource)
            {
                WebAnalysisSites = _polar.GetWebAnalysisSites().ToList();
            }
            else
            {
                WebAnalysisSites = null;
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

        private void _TopLevelService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ITopLevelService.FullScreen)))
            {
                MediaController.FullScreen = _topLevelService.FullScreen;
            }
        }

        private void MediaController_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IMediaController.FullScreen)))
            {
                _topLevelService.FullScreen = MediaController.FullScreen;
            }
        }

        private void Controller_NextClick(object sender, EventArgs e)
        {
            if (SelectionModel == null)
            {
                return;
            }

            var count = (SelectionModel.Source as ICollection<ResourceTreeNode>)?.Count;
            if (SelectionModel.SelectedIndex < count)
            {
                SelectionModel.Select(SelectionModel.SelectedIndex + 1);
            }
        }

        private void Controller_PreviousClick(object sender, EventArgs e)
        {
            if (SelectionModel == null)
            {
                return;
            }
            var count = (SelectionModel.Source as ICollection<ResourceTreeNode>)?.Count;
            if (SelectionModel.SelectedIndex > 0 && count > 0)
            {
                SelectionModel.Select(SelectionModel.SelectedIndex - 1);
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
                        break;
                    case LinkType.Video:
                        await ToPlayVideo(node);
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
                if (MediaController.IsLoading)
                {
                    Cancellation.Cancel();
                    await WhenTaskCompleted(() => MediaController.IsLoading);
                }

                MediaController.IsLoading = true;

                if (MediaController.Controller?.IsPlaying == true)
                {
                    await MediaController.Controller.StopAsync();
                }

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
                MediaController.IsLoading = false;
            }
            return null;
        }

        private async Task ToPlayVideo(ILink videoSource)
        {
            if (videoSource == null || string.IsNullOrEmpty(videoSource.Src))
            {
                return;
            }

            var episode = Param_Link?.Name;
            if (SelectionModel.SelectedItem != null)
            {
                episode = (SelectionModel.SelectedItem as ResourceTreeNode).Name;
            }

            MediaController.Title = $"{this.Resource.Name}-{episode}";
            await MediaController.Controller?.PlayAsync(new Uri(videoSource.Src));
        }

        private async Task TryUpdateHistory()
        {
            var second = HeaderSelection.SelectedItem as ResourceTreeNode;
            var third = SelectionModel.SelectedItem as ResourceTreeNode;
            var thirdIndex = SelectionModel.SelectedIndex;

            await TryUpdateHistory(this.Resource, second, third, thirdIndex, (long)_currentProgress.TotalMilliseconds);
        }

        private string CombineDesc(ResourceTreeNode second, ResourceTreeNode third) => $"{second.Name}-{third.Name}";

        private async Task TryUpdateHistory(ResourceTreeNode root, ResourceTreeNode second, ResourceTreeNode third, int thirdIndex, long progress)
        {
            if (root == null || second == null || third == null)
            {
                return;
            }

            var his = History;
            var desc = CombineDesc(second, third);
            if (his == null)
            {
                his = new HistoryModel
                {
                    ResourceName = root.Name,
                    ProgressDesc = desc,
                    Progress = progress,
                    ProgressIndex = thirdIndex
                };
            }
            else
            {
                his.ProgressDesc = desc;
                his.ProgressIndex = thirdIndex;
                his.Progress = progress;
            }

            await _hisService.AddOrUpdateAsync(his);

            this.History = null;
            this.History = his;
        }

        private (string, string, int) GetProgressDesc(HistoryModel history)
        {
            if (history == null) return default;

            if(string.IsNullOrEmpty(history.ProgressDesc)) return default;

            var descSplit = history.ProgressDesc.Split('-');
            var episode = string.Empty;
            var category = string.Empty;
            if (descSplit.Length >= 2)
            {
                category = descSplit[0];
                episode = descSplit[1];
            }

            return (category, episode, history.ProgressIndex);
        }
    }
}
