﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using LibVLCSharp.Shared;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal class VLController : ObservableObject, IVLController
    {
        private bool _disposed;
        private MediaPlayer _mediaPlayer;
        private static readonly TimeSpan _ignore = TimeSpan.FromSeconds(1);
        private readonly IDispatcherUI _dispatcherUI;
        public VLController()
        {
            _dispatcherUI = Ioc.Default.GetRequiredService<IDispatcherUI>();
            _mediaPlayer = new MediaPlayer(Ioc.Default.GetRequiredService<LibVLC>());
            _mediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
            _mediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
            _mediaPlayer.Playing += MediaPlayer_Playing;
            _mediaPlayer.Paused += MediaPlayer_Paused;
            _mediaPlayer.Stopped += MediaPlayer_Stopped;
            _mediaPlayer.Buffering += MediaPlayer_Buffering;
            //_mediaPlayer.VolumeChanged += MediaPlayer_VolumeChanged;//problem on android; cause crash when stop or disposed.
            _mediaPlayer.MediaChanged += MediaPlayer_MediaChanged;
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
            _mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
        }

        public MediaPlayer MediaPlayer => _mediaPlayer;

        private TimeSpan _length = TimeSpan.Zero;
        public TimeSpan Length
        {
            get => _length;
            private set
            {
                if (_dispatcherUI.CheckAccess())
                {
                    SetProperty(ref _length, value);
                }
                else
                {
                    _dispatcherUI.Post(()=> SetProperty(ref _length, value));
                }
            }
        }

        private TimeSpan _time = TimeSpan.Zero;
        public TimeSpan Time
        {
            get => _time;
            set
            {
                if (_dispatcherUI.CheckAccess())
                {
                    if(SetProperty(ref _time, value))
                    {
                        TryToSeekTime(TimeSpan.FromMilliseconds(MediaPlayer.Time), value);
                    }
                }
                else
                {
                    _dispatcherUI.Post(()=> SetProperty(ref _time, value));
                }
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_dispatcherUI.CheckAccess())
                {
                    SetProperty(ref _isPlaying, value);
                }
                else
                {
                    _dispatcherUI.Post(() => SetProperty(ref _isPlaying, value));
                }
            }
        }

        public float Speed
        {
            get =>  _mediaPlayer?.Rate ?? 1f;
            set
            {
                var speed = _mediaPlayer?.Rate ?? 1f;
                if(SetProperty(ref speed, value))
                {
                    _mediaPlayer.SetRate(value);
                }
            }
        }

        public int Volume
        {
            get
            {
                if (_mediaPlayer == null)
                {
                    return 0;
                }
                return _mediaPlayer.Volume;
            }
            set
            {
                if(_mediaPlayer != null)
                {
                    var vol = _mediaPlayer.Volume;
                    if (SetProperty(ref vol, value))
                    {
                        _mediaPlayer.Volume = value;
                    }
                }
            }
        }

        public event EventHandler<TimeSpan> LengthChanged;
        public event EventHandler<TimeSpan> TimeChanged;
        public event EventHandler Playing;
        public event EventHandler Paused;
        public event EventHandler Stopped;
        public event EventHandler Error;
        public event EventHandler Ended;
        public event EventHandler MediaChanged;
        public event EventHandler<float> Buffering;

        public async void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            MediaPlayer cache = _mediaPlayer;
            _mediaPlayer = null;
            if (cache != null)
            {
                await Task.Run(() =>
                {
                    //TODO 在安卓上 网络视频加载不会马上停止，非常浪费流量
                    //可能：https://code.videolan.org/videolan/LibVLCSharp/-/issues/214
                    cache.Stop();
                    if (cache.Media != null)
                    {
                        cache.Media.ParseStop();
                        cache.Media.Dispose();
                        cache.Media = null;
                    }
                    cache.Dispose();
                });
            }
        }

        public void Play()
        {
            MediaPlayer?.Play();
        }

        public void Play(Uri uri)
        {
            MediaPlayer?.Play(new LibVLCSharp.Shared.Media(Ioc.Default.GetRequiredService<LibVLC>(), uri));
        }

        public void Pause()
        {
            MediaPlayer?.Pause();
        }

        public void Stop()
        {
            MediaPlayer?.Stop();
            MediaPlayer.Media = null;
        }

        public Task StopAsync()
        {
            return Task.Run(() =>
            {
                MediaPlayer?.Stop();
                MediaPlayer.Media = null;

            });
        }

        public Task PauseAsync()
        {
            return Task.Run(() => MediaPlayer?.Pause());
        }

        public Task PlayAsync()
        {
            return Task.Run(() => MediaPlayer?.Play());
        }

        public Task PlayAsync(Uri uri)
        {
            return Task.Run(() => MediaPlayer?.Play(new LibVLCSharp.Shared.Media(Ioc.Default.GetRequiredService<LibVLC>(), uri)));
        }

        private void MediaPlayer_Paused(object sender, EventArgs e)
        {
            IsPlaying = false;
            this.Paused?.Invoke(this, e);
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            IsPlaying = true;
            this.Playing?.Invoke(this, e);
        }

        private void MediaPlayer_Stopped(object sender, EventArgs e)
        {
            IsPlaying = false;
            this.Stopped?.Invoke(this, e);
        }

        private void MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            var c = e.Time / 1000 * 1000;
            if (Math.Abs(c - (long)_time.TotalMilliseconds) >= 1000)
            {
                Time = TimeSpan.FromMilliseconds(c);
                this.TimeChanged?.Invoke(this, Time);
            }
        }

        private void MediaPlayer_LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            Length = TimeSpan.FromMilliseconds(e.Length);
            this.LengthChanged?.Invoke(this, Length);
        }

        private void MediaPlayer_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            this.Buffering?.Invoke(this, e.Cache);
        }

        private void MediaPlayer_VolumeChanged(object sender, MediaPlayerVolumeChangedEventArgs e)
        {

        }

        private void MediaPlayer_MediaChanged(object sender, MediaPlayerMediaChangedEventArgs e)
        {
            this.MediaChanged?.Invoke(this, e);
        }

        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            this.Ended?.Invoke(this, e);
        }

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            this.Error?.Invoke(this, e);
        }

        private void TryToSeekTime(TimeSpan old, TimeSpan newVal)
        {
            if (Math.Abs(newVal.TotalMilliseconds - old.TotalMilliseconds) > _ignore.TotalMilliseconds)
            {
                MediaPlayer.Time = (long)newVal.TotalMilliseconds;
            }
        }
    }
}
