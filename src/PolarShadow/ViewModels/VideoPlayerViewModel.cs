using Avalonia.Controls.Notifications;
using LibVLCSharp.Shared;
using PolarShadow.Navigations;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class VideoPlayerViewModel : ViewModelBase, IParameterObtain
    {
        private readonly LibVLC _libVLC;
        private readonly INotificationManager _notify;
        private readonly INavigationService _nav;
        public VideoPlayerViewModel(LibVLC libVLC, INotificationManager notify, INavigationService nav)
        {
            _libVLC = libVLC;
            _notify = notify;
            _nav = nav;
        }

        public ILink Param_Episode { get; set; }

        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set => SetProperty(ref _mediaPlayer, value);
        }

        public string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(nameof(Param_Episode), out ILink node))
            {
                Param_Episode = node;
                Title = Param_Episode.Name;
            }
        }

        protected override void OnLoad()
        {
            if (Param_Episode == null)
            {
                _notify.Show($"Miss {nameof(Param_Episode)}");
                return;
            }

            

            if (MediaPlayer == null)
            {
                MediaPlayer = new MediaPlayer(_libVLC);
            }
        }
    }
}
