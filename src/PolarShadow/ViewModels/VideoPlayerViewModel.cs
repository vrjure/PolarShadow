using Avalonia;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using PolarShadow.Core;
using PolarShadow.Models;
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
        private static readonly Thickness FullSceenPadding = new Thickness(0);
        private static readonly Thickness NormalScreenPadding = new Thickness(1, 0, 2, 1);

        private readonly LibVLC _libVLC;
        private readonly INotificationManager _notify;
        private readonly ITopLevelService _topLevel;
        private readonly IPolarShadow _polar;
        public VideoPlayerViewModel(LibVLC libVLC, INotificationManager notify, ITopLevelService topLevel, IPolarShadow polar)
        {
            _libVLC = libVLC;
            _notify = notify;
            _topLevel = topLevel;
            _polar = polar;
        }

        public ILink Param_Episode { get; set; }
        public string Param_Title { get; set; }

        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set => SetProperty(ref _mediaPlayer, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _showNext;
        public bool ShowNext
        {
            get => _showNext;
            set => SetProperty(ref _showNext, value);
        }

        private bool _showPrevious;
        public bool ShowPrevious
        {
            get => _showPrevious;
            set => SetProperty(ref _showPrevious, value);
        }

        private bool _fullScreen;
        public bool FullScreen
        {
            get => _fullScreen;
            set
            {
                SetProperty(_fullScreen, value, v =>
                {
                    _fullScreen = value;
                    if (_fullScreen)
                    {
                        SetFullScreen();
                    }
                    else
                    {
                        ExitFullScreen();
                    }
                });
            }
        }

        private Thickness _padding = OperatingSystem.IsWindows() ? NormalScreenPadding : FullSceenPadding;
        public Thickness Padding
        {
            get => _padding;
            set => SetProperty(ref _padding, value);
        }

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(nameof(Param_Episode), out ILink node))
            {
                Param_Episode = node;
            }

            if (parameters.TryGetValue(nameof(Param_Title), out string title))
            {
                Title = title;
            }
        }

        protected override  void OnLoad()
        {
            ShowPrevious = false;
            ShowNext = false;

            if (Param_Episode == null)
            {
                _notify.Show($"Miss {nameof(Param_Episode)}");
                return;
            }

            var videoUrl = Param_Episode;

            switch (Param_Episode.SrcType)
            {
                case LinkType.Video:
                    break;
                default:
                    _notify.Show($"not support media [{Param_Episode.SrcType}]");
                    return;
            }

            if (videoUrl == null || string.IsNullOrEmpty(videoUrl.Src))
            {
                _notify.Show("No resource");
                return;
            }
            
            if (MediaPlayer == null)
            {
                MediaPlayer = new MediaPlayer(_libVLC);
            }

            try
            {
                MediaPlayer.Play(new Media(_libVLC, new Uri(videoUrl.Src)));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        protected override void OnUnload()
        {
            try
            {
                if (FullScreen)
                {
                    FullScreen = false; 
                }

                var mp = MediaPlayer;
                MediaPlayer = null;
                if (mp == null)
                {
                    return;
                }
                Task.Run(() =>
                {
                    mp.Stop();
                    mp.Dispose();
                });
            }
            catch { }

        }

        private void SetFullScreen()
        {
            Messenger.Send(FullScreenState.FullScreen);
            Padding = FullSceenPadding;
            //_topLevel.FullScreen();
        }

        private void ExitFullScreen()
        {
            Messenger.Send(FullScreenState.Normal);
            Padding = NormalScreenPadding;
            //_topLevel.ExitFullScreen();
        }
    }
}
