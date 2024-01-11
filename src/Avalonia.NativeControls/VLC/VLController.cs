using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public class VLController : IVideoViewController
    {
        private object _lock = new object();
        private bool _disposed;
        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get
            {
                lock (_lock)
                {
                    if (_mediaPlayer == null && !_disposed)
                    {
                        _mediaPlayer = new MediaPlayer(NativeControls.GetHandler<LibVLC>());
                        _mediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
                        _mediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
                        _mediaPlayer.Playing += MediaPlayer_Playing;
                        _mediaPlayer.Paused += MediaPlayer_Paused;
                    }
                }
                return _mediaPlayer;
            }
        }

        public TimeSpan Length => TimeSpan.FromMilliseconds(MediaPlayer.Length);

        public TimeSpan Time
        {
            get => TimeSpan.FromMilliseconds(MediaPlayer.Time);
            set => MediaPlayer.Time = (long)value.TotalMilliseconds;
        }

        public bool IsPlaying => MediaPlayer.IsPlaying;

        public event EventHandler<TimeSpan> LengthChanged;
        public event EventHandler<TimeSpan> TimeChanged;
        public event EventHandler Playing;
        public event EventHandler Paused;

        public void Stop()
        {
            MediaPlayer.Stop();
        }

        public void Pause()
        {
            MediaPlayer.Pause();
        }

        public void Play()
        {
            MediaPlayer.Play();
        }

        public void Play(Uri uri)
        {
            MediaPlayer.Play(new LibVLCSharp.Shared.Media(NativeControls.GetHandler<LibVLC>(), uri));
        }

        private void MediaPlayer_Paused(object sender, EventArgs e)
        {
            this.Paused?.Invoke(sender, e);
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            this.Playing?.Invoke(sender, e);
        }

        private void MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            this.TimeChanged?.Invoke(sender, TimeSpan.FromMilliseconds(e.Time));
        }

        private void MediaPlayer_LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            this.LengthChanged?.Invoke(sender, TimeSpan.FromMilliseconds(e.Length));
        }

        public void Dispose()
        {
            lock(_lock)
            {
                if (_mediaPlayer != null)
                {
                    _mediaPlayer.Dispose();
                    _mediaPlayer = null;
                }
                _disposed = true;
            }

        }
    }
}
