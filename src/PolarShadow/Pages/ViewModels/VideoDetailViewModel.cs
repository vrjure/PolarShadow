using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Pages.Controls;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Pages.ViewModels
{
    public partial class VideoDetailViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IPolarShadow _polarShadow;
        private readonly IMyCollectionService _myCollectionService;
        private readonly IWatchRecordService _watchRecordService;

        public static string Key_VideoSummary = "videosummary";
        public VideoDetailViewModel(IPolarShadow polarShadow, IMyCollectionService myCollectionService, IWatchRecordService watchRecordService)
        {
            _polarShadow = polarShadow;
            _myCollectionService = myCollectionService;
            _watchRecordService = watchRecordService;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(Key_VideoSummary, out object val))
            {
                if (val is VideoSummary summary)
                {
                    await GetDetailAsync(summary);
                }
            }
        }

        private async Task GetDetailAsync(VideoSummary summary)
        {
            var site = _polarShadow.GetSite(summary.SiteName);
            if (site == null)
            {
                return;
            }

            if (site.TryGetAbility(out IGetDetailAble detailHandler))
            {
                var detail = await detailHandler.GetVideoDetailAsync(summary);
                if (detail == null)
                {
                    return;
                }

                this.Detail = detail;

                IsCollected = await _myCollectionService.HasAsync(detail);

                await UpdateEpisodeAsync();
            }
        }

        private async Task UpdateEpisodeAsync()
        {
            var episodeList = new List<CombinationObject<VideoEpisode, string>>();
            var records = await _watchRecordService.GetRecordsAsync(detail.Name);
            //foreach (var episode in detail.Episodes)
            //{
            //    var record = records.FirstOrDefault(f => f.EpisodeName == episode.Name);
            //    if (record == null)
            //    {
            //        episodeList.Add(new CombinationObject<VideoEpisode, string>
            //        {
            //            Object1 = episode,
            //            Object2 = "(未看)"
            //        });
            //    }
            //    else
            //    {
            //        episodeList.Add(new CombinationObject<VideoEpisode, string>
            //        {
            //            Object1 = episode,
            //            Object2 = "(已看)"
            //        });
            //    }
            //}

            CombinationEpisodes.Clear();
            foreach (var item in episodeList)
            {
                CombinationEpisodes.Add(item);
            }
        }

        [ObservableProperty]
        private VideoDetail detail;
        public ObservableCollection<CombinationObject<VideoEpisode, string>> CombinationEpisodes { get; } = new();

        [RelayCommand]
        public void EpisodeSelectedChange(VideoEpisode episode)
        {
            if (episode == null || episode.Sources == null)
            {
                BtnPlayEnable = false;
                BtnDownloadEnable = false;
                return;
            }

            foreach (var item in episode.Sources)
            {
                if (item.SrcType == SrcType.HTML || item.SrcType == SrcType.HTMLAnalysis)
                {
                    BtnPlayEnable = true;
                    BtnDownloadEnable = false;
                    return;
                }
                else if (item.SrcType == SrcType.Magnet)
                {
                    BtnDownloadEnable = true;
                    BtnPlayEnable = false;
                    return;
                }
            }

            BtnPlayEnable = false;
            BtnDownloadEnable = false;
        }

        [ObservableProperty]
        private bool btnDownloadEnable;
        [ObservableProperty]
        private bool btnPlayEnable;
        [ObservableProperty]
        private bool isCollected;

        [RelayCommand]
        public Task BtnDownloadClick(VideoEpisode selectItem)
        {
            if (selectItem == null)
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task BtnPlayClick(VideoEpisode selectItem)
        {
            if (selectItem == null)
            {
                return;
            }
            await Launcher.OpenAsync(selectItem.Sources.First().Src);
            await _watchRecordService.AddOrUpdateRecordAsync(new WatchRecord
            {
                EpisodeName = selectItem.Name,
                Name = Detail.Name,
            });
            await UpdateEpisodeAsync();
        }

        [RelayCommand]
        public async Task ToggedChanged(ToggledEventArgs arg)
        {
            if (arg.Value)
            {
                await _myCollectionService.AddToMyCollectionAsync(this.Detail);
            }
            else
            {
                await _myCollectionService.RemoveFromMyCollectionAsync(this.detail.Name);
            }
        }
    }
}
