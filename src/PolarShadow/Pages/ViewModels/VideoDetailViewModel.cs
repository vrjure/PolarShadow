using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Pages.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Pages.ViewModels
{
    public partial class VideoDetailViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IPolarShadow _polarShadow;

        public static string Key_VideoSummary = "videosummary";
        public VideoDetailViewModel(IPolarShadow polarShadow)
        {
            _polarShadow = polarShadow;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(Key_VideoSummary, out object val))
            {
                if (val is VideoSummary summary)
                {
                    await GetDetailAsync(summary.DetailSrc, summary);
                }
                else if (val is string detailSrc)
                {
                    await GetDetailAsync(detailSrc, null);
                }
            }
        }

        private async Task GetDetailAsync(string detailSrc, VideoSummary summary)
        {
            var site = _polarShadow.GetSite(summary.SiteName);
            if (site == null)
            {
                return;
            }

            var detail = await site.GetVideoDetailAsync(detailSrc, summary);
            if (detail == null)
            {
                return;
            }

            this.Detail = detail;
        }

        [ObservableProperty]
        private VideoDetail detail;

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
        }

    }
}
