using Android.Content;
using Android.Media;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using PolarShadow.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Android.Graphics.Color;
using Uri = Android.Net.Uri;

namespace PolarShadow.Platforms.Android
{
    internal class VideoPlayer : CoordinatorLayout, MediaPlayer.IOnPreparedListener
    {
        VideoView _playerView;
        MediaController _mediaController;
        bool _isPrepared;
        Context _context;
        PolarShadow.Controls.VideoPlayer _player;

        public VideoPlayer(Context context, PolarShadow.Controls.VideoPlayer player) : base(context)
        {
            _context = context;
            _player = player;

            SetBackgroundColor(Color.Black);

            // Create a RelativeLayout for sizing the video
            RelativeLayout relativeLayout = new RelativeLayout(_context)
            {
                LayoutParameters = new CoordinatorLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                {
                    Gravity = (int)GravityFlags.Center
                }
            };

            // Create a VideoView and position it in the RelativeLayout
            _playerView = new VideoView(context)
            {
                LayoutParameters = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
            };

            // Add to the layouts
            relativeLayout.AddView(_playerView);
            AddView(relativeLayout);

            _playerView.Prepared += OnVideoViewPrepared;
        }

        public void UpdateTransportControlsEnabled()
        {
            if (_player.AreTransportControlsEnabled)
            {
                _mediaController = new MediaController(_context);
                _mediaController.SetMediaPlayer(_playerView);
                _playerView.SetMediaController(_mediaController);
            }
            else
            {
                _playerView.SetMediaController(null);
                if (_mediaController != null)
                {
                    _mediaController.SetMediaPlayer(null);
                    _mediaController = null;
                }
            }
        }

        public void UpdateSource()
        {
            _isPrepared = false;
            bool hasSetSource = false;

            if (_player.Source is UriVideoSource)
            {
                string uri = (_player.Source as UriVideoSource).Uri;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _playerView.SetVideoURI(Uri.Parse(uri));
                    hasSetSource = true;
                }
            }
            else if (_player.Source is FileVideoSource)
            {
                string filename = (_player.Source as FileVideoSource).File;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    _playerView.SetVideoPath(filename);
                    hasSetSource = true;
                }
            }
            else if (_player.Source is ResourceVideoSource)
            {
                string package = Context.PackageName;
                string path = (_player.Source as ResourceVideoSource).Path;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string assetFilePath = "content://" + package + "/" + path;
                    _playerView.SetVideoPath(assetFilePath);
                    hasSetSource = true;
                }
            }

            if (hasSetSource && _player.AutoPlay)
            {
                _playerView.Start();
            }
        }

        public void UpdateIsLooping()
        {
            if (_player.IsLooping)
            {
                _playerView.SetOnPreparedListener(this);
            }
            else
            {
                _playerView.SetOnPreparedListener(null);
            }
        }

        public void UpdatePosition()
        {
            if (Math.Abs(_playerView.CurrentPosition - _player.Position.TotalMilliseconds) > 1000)
            {
                _playerView.SeekTo((int)_player.Position.TotalMilliseconds);
            }
        }

        public void UpdateStatus()
        {
            PlayerStatus status = PlayerStatus.NotReady;

            if (_isPrepared)
            {
                status = _playerView.IsPlaying ? PlayerStatus.Playing : PlayerStatus.Paused;
            }

            ((IVideoController)_player).Status = status;

            // Set Position property
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(_playerView.CurrentPosition);
            _player.Position = timeSpan;
        }

        public void PlayRequested(TimeSpan position)
        {
            _playerView.Start();
            System.Diagnostics.Debug.WriteLine($"Video playback from {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void PauseRequested(TimeSpan position)
        {
            _playerView.Pause();
            System.Diagnostics.Debug.WriteLine($"Video paused at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");
        }

        public void StopRequested(TimeSpan position)
        {
            // Stops and releases the media player
            _playerView.StopPlayback();
            System.Diagnostics.Debug.WriteLine($"Video stopped at {position.Hours:X2}:{position.Minutes:X2}:{position.Seconds:X2}.");

            // Ensure the video can be played again
            _playerView.Resume();
        }

        void OnVideoViewPrepared(object sender, EventArgs args)
        {
            _isPrepared = true;
            ((IVideoController)_player).Duration = TimeSpan.FromMilliseconds(_playerView.Duration);
        }

        public void OnPrepared(MediaPlayer mp)
        {
            mp.Looping = _player.IsLooping;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _playerView.Prepared -= OnVideoViewPrepared;
                _playerView.Dispose();
                _playerView = null;
            }
            base.Dispose(disposing);
        }
    }
}
