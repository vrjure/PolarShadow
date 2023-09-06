using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Navigations;
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
        public DetailViewModel(IPolarShadow polar, INotificationManager notify)
        {
            _polar = polar;
            _notify = notify;
        }

        public VideoSummary VideoSummary { get; set; }


        private VideoDetail _detail;
        public VideoDetail Detail
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
        public bool IsSave
        {
            get => _isSave;
            set => SetProperty(ref _isSave, value);
        }

        private IAsyncRelayCommand _refreshCommand;
        public IAsyncRelayCommand RefreshCommand => _refreshCommand ??= new AsyncRelayCommand(LoadingDeatil);

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if(parameters.TryGetValue(nameof(VideoSummary), out VideoSummary summary))
            {
                if (summary is VideoDetail detail)
                {
                    VideoSummary = Detail = detail;
                }
                else
                {
                    VideoSummary = summary;
                }
            }
        }

        public override async void OnLoad()
        {
            if (Detail != null)
            {
                return;
            }

            await LoadingDeatil();
        }

        private async Task LoadingDeatil()
        {
            if (VideoSummary == null)
            {
                _notify.Show("Missing parameters", NotificationType.Warning);
                return;
            }

            if(!_polar.TryGetSite(VideoSummary.Site, out ISite site))
            {
                _notify.Show($"Can not fond Site [{VideoSummary.Site}]", NotificationType.Warning);
                return;
            }

            IsLoading = true;
            try
            {
                var result = await site.GetDetailAsync(VideoSummary);
                if (result == null)
                {
                    _notify.Show("No Data");
                    return;
                }

                Detail = result;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
            IsLoading = false;
        }
    }
}
