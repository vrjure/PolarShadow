using Microsoft.UI.Xaml.Controls;
using PolarShadow.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Grid = Microsoft.UI.Xaml.Controls.Grid;

namespace PolarShadow.Platforms.Windows
{
    class VideoPlayer : Grid, IDisposable
    {
        MediaPlayerElement _mediaElement;
        PolarShadow.Controls.VideoPlayer _player;
        bool _isMediaPlayerAttached;

        public VideoPlayer(PolarShadow.Controls.VideoPlayer player)
        {
            _player = player;
            _mediaElement = new MediaPlayerElement();
            this.Children.Add(_mediaElement);
        }

        void OnMediaPlayerMediaOpened(MediaPlayer sender, object args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((IVideoController)_player).Duration = _mediaElement.MediaPlayer.NaturalDuration;
            });
        }

        public void UpdateTransportControlsEnabled()
        {
            _mediaElement.AreTransportControlsEnabled = _player.AreTransportControlsEnabled;
        }

        public async void UpdateSource()
        {
            bool hasSetSource = false;

            if (_player.Source is UriVideoSource)
            {
                string uri = (_player.Source as UriVideoSource).Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _mediaElement.Source = MediaSource.CreateFromUri(new Uri(uri));
                    hasSetSource = true;
                }
            }
            else if (_player.Source is FileVideoSource)
            {
                string filename = (_player.Source as FileVideoSource).File;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filename);
                    _mediaElement.Source = MediaSource.CreateFromStorageFile(storageFile);
                    hasSetSource = true;
                }
            }
            else if (_player.Source is ResourceVideoSource)
            {
                string path = "ms-appx:///" + (_player.Source as ResourceVideoSource).Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    _mediaElement.Source = MediaSource.CreateFromUri(new Uri(path));
                    hasSetSource = true;
                }
            }

            if (hasSetSource && !_isMediaPlayerAttached)
            {
                _isMediaPlayerAttached = true;
                _mediaElement.MediaPlayer.MediaOpened += OnMediaPlayerMediaOpened;
            }

            if (hasSetSource && _player.AutoPlay)
            {
                _mediaElement.AutoPlay = true;
            }
        }

        public void UpdateIsLooping()
        {
            if (_isMediaPlayerAttached)
                _mediaElement.MediaPlayer.IsLoopingEnabled = _player.IsLooping;
        }

        public void UpdatePosition()
        {
            if (_isMediaPlayerAttached)
            {
                if (Math.Abs((_mediaElement.MediaPlayer.Position - _player.Position).TotalSeconds) > 1)
                {
                    _mediaElement.MediaPlayer.Position = _player.Position;
                }
            }
        }

        public void UpdateStatus()
        {
            if (_isMediaPlayerAttached)
            {
                PlayerStatus status = PlayerStatus.NotReady;

                switch (_mediaElement.MediaPlayer.CurrentState)
                {
                    case MediaPlayerState.Playing:
                        status = PlayerStatus.Playing;
                        break;
                    case MediaPlayerState.Paused:
                    case MediaPlayerState.Stopped:
                        status = PlayerStatus.Paused;
                        break;
                }

                ((IVideoController)_player).Status = status;
                _player.Position = _mediaElement.MediaPlayer.Position;
            }
        }

        public void PlayRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                _mediaElement.MediaPlayer.Play();
                System.Diagnostics.Debug.WriteLine($"Video playback from {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        public void PauseRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                _mediaElement.MediaPlayer.Pause();
                System.Diagnostics.Debug.WriteLine($"Video paused at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        public void StopRequested(TimeSpan position)
        {
            if (_isMediaPlayerAttached)
            {
                // There's no Stop method so pause the video and reset its position
                _mediaElement.MediaPlayer.Pause();
                _mediaElement.MediaPlayer.Position = TimeSpan.Zero;
                System.Diagnostics.Debug.WriteLine($"Video stopped at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
            }
        }

        public void Dispose()
        {
            if (_isMediaPlayerAttached)
            {
                _mediaElement.MediaPlayer.MediaOpened -= OnMediaPlayerMediaOpened;
                _mediaElement.MediaPlayer.Dispose();
            }
            _mediaElement = null;
        }
    }
}
