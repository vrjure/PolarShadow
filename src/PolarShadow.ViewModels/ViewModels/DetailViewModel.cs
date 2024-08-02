using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Essentials;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolarShadow.Navigations;
using Aria2Net;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace PolarShadow.ViewModels
{
    public partial class DetailViewModel : ViewModelBase, IParameterObtain
    {
        private readonly IPolarShadow _polar;
        private readonly IMessageService _notify;
        private readonly IMineResourceService _mineResourceService;
        private readonly IBufferCache _bufferCache;
        private readonly IPreference _preference;
        private readonly INavigationService _nav;
        private readonly IHistoryService _hisService;
        private readonly IDispatcherUI _dispathcerUI;

        private ResourceModel _rootResourceInDb;
        private TimeSpan _currentProgress;

        public DetailViewModel(IPolarShadow polar, IMessageService notify, IMineResourceService mineResourceService, IBufferCache bufferCache, IPreference preference, INavigationService nav, IHistoryService hisService, IVideoViewController videoController, IDispatcherUI dispatcherUI)
        {
            _polar = polar;
            _notify = notify;
            _mineResourceService = mineResourceService;
            _bufferCache = bufferCache;
            _preference = preference;
            _nav = nav;
            _hisService = hisService;
            VideoController = videoController;
            _dispathcerUI = dispatcherUI;
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
            if (this.Resource == null)
            {
                await LoadingDetail();
            }
        }

        protected override async void OnUnload()
        {
            if (FullScreen)
            {
                FullScreen = false;
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

        private async Task LoadingDetail()
        {
            if (Param_Link == null)
            {
                _notify.Show("Missing parameters", MessageType.Warning);
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
                list = [root, .. children];
            }

            var tree = children.BuildTree();
            this.Resource = tree;
        }

        private async Task LoadOnline()
        {
            if (!_polar.TryGetSite(Param_Link.Site, out ISite site))
            {
                _notify.Show($"Can not fond site [{Param_Link.Site}]", MessageType.Warning);
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
                    await _bufferCache.CacheFileIfExisedInMemory(SHA.SHA256(this.Resource.ImageSrc));
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
                    _bufferCache.Remove(SHA.SHA256(this.Resource.ImageSrc), BufferLocation.File);
                }
                IsSaved = false;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }

        private async Task EpisodeSelectedAsync(ResourceTreeNode node)
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
            var rpc = await _preference.GetAsync(Preferences.RPC, string.Empty);
            var downloadFolder = await _preference.GetAsync(Preferences.DownloadPath, string.Empty);
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
                        _notify.Show(result.Error.Message, MessageType.Error);
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
                if (IsLoading)
                {
                    Cancellation.Cancel();
                    //await WhenTaskCompleted(() => MediaController.IsLoading);
                }

                IsLoading = true;

                if (VideoController?.IsPlaying == true)
                {
                    await VideoController.StopAsync();
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
                IsLoading = false;
            }
            return null;
        }

        private async Task ToPlayVideo(ILink videoSource)
        {
            if (videoSource == null || string.IsNullOrEmpty(videoSource.Src))
            {
                return;
            }

            await VideoController?.PlayAsync(new Uri(videoSource.Src));
        }

        private async Task TryUpdateHistory()
        {
            await TryUpdateHistory(this.Resource, CurrentHead, CurrentEpisode, CurrentEpisodeIndex, (long)_currentProgress.TotalSeconds);
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

        private void _controller_TimeChanged(object sender, TimeSpan e)
        {
            _currentProgress = e;
        }

        private void Controller_MediaChanged(object sender, EventArgs e)
        {
            _dispathcerUI.Post(async () =>
            {
                if (History != null)
                {
                    var descItem = GetProgressDesc(History);
                    if (descItem.Item2 == CurrentEpisode.Name || descItem.Item3 == _currentEpisodeIndex)
                    {
                        VideoController.Time = _currentProgress;
                        return;
                    }
                }

                await TryUpdateHistory(this.Resource, CurrentHead, CurrentEpisode, CurrentEpisodeIndex, 0);
            });
        }

        private async Task PreviousAsync()
        {
            if (VideoController == null || CurrentHead == null || CurrentHead.Children?.Count == 0) return;
            var previousIndex = _currentEpisodeIndex - 1;
            if (previousIndex < 0)
            {
                return;
            }
            await EpisodeSelectedAsync(CurrentHead.Children.AsList()[previousIndex]);
            CurrentEpisodeIndex = previousIndex;
        }

        private async Task NextAsync()
        {
            if (VideoController == null || CurrentHead == null || CurrentHead.Children?.Count == 0) return;

            var nextIndex = _currentEpisodeIndex + 1;
            if (nextIndex >= CurrentHead.Children.Count)
            {
                return;
            }
            var next = CurrentHead.Children.AsList()[nextIndex];
            CurrentEpisodeIndex = nextIndex;
            await EpisodeSelectedAsync(next);
        }

        private async Task PlayPauseAsync()
        {
            if (VideoController == null) return;
            if (VideoController.IsPlaying)
            {
                await VideoController.PauseAsync();
            }
            else
            {
                await VideoController.PlayAsync();
            }
        }

        private void Controller_Ended(object sender, EventArgs e)
        {
            _dispathcerUI.Post(async () =>
            {
                await NextAsync();
            });
        }
    }
}
