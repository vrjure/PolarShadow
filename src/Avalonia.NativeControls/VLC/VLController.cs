﻿using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public class VLController : ObservableObject, IVideoViewController
    {
        private object _lock = new object();
        private bool _disposed;
        private MediaPlayer _mediaPlayer;
        private static readonly TimeSpan _ignore = TimeSpan.FromSeconds(1);

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
                        _mediaPlayer.Stopped += MediaPlayer_Stopped;
                        _mediaPlayer.Buffering += MediaPlayer_Buffering;
                        _mediaPlayer.VolumeChanged += MediaPlayer_VolumeChanged;
                        _mediaPlayer.MediaChanged += MediaPlayer_MediaChanged;
                        _mediaPlayer.EndReached += MediaPlayer_EndReached;
                        _mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
                    }

                    if (_disposed)
                    {
                        _mediaPlayer = null;
                    }
                    return _mediaPlayer;
                }
            }
        }

        private TimeSpan _length;
        public TimeSpan Length
        {
            get => _length;
            set
            {
                if(Dispatcher.UIThread.CheckAccess())
                {
                    SetProperty(ref _length, value);
                }
                else
                {
                    Dispatcher.UIThread.Post(() => SetProperty(ref _length, value));
                }
            }
        }

        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set
            {
                if (_time == value)
                {
                    return;
                }
                if (Dispatcher.UIThread.CheckAccess())
                {
                    var old = _time;
                    if(SetProperty(ref _time, value))
                    {
                        TryToSeekTime(old, value);
                    }
                }
                else
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        if(SetProperty(ref _time, value))
                        {
                            TryToSeekTime(_time, value);
                        }
                    });
                }
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (Dispatcher.UIThread.CheckAccess())
                {
                    SetProperty(ref _isPlaying, value);
                }
                else
                {
                    Dispatcher.UIThread.Post(() => SetProperty(ref _isPlaying, value));
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
            return Task.Run(() => MediaPlayer?.Play(new LibVLCSharp.Shared.Media(NativeControls.GetHandler<LibVLC>(), uri)));
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
            Time = TimeSpan.FromMilliseconds(e.Time);
            this.TimeChanged?.Invoke(this, Time);
        }

        private void MediaPlayer_LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            Length = TimeSpan.FromMilliseconds(e.Length);
            this.LengthChanged?.Invoke(this, Length);
        }

        private void MediaPlayer_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(e.Cache);
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
            if (Math.Abs(newVal.TotalMilliseconds - old.TotalMilliseconds) >= _ignore.TotalMilliseconds)
            {
                MediaPlayer.Time = (long)newVal.TotalMilliseconds;
            }
        }

        public void Dispose()
        {
            lock(_lock)
            {
                _disposed = true;
                if (_mediaPlayer != null)
                {
                    var cache = _mediaPlayer;
                    _mediaPlayer = null;
                    Task.Run(() =>
                    {
                        cache.Hwnd = 0;
                        cache.Dispose();
                    });
                }
            }

        }
    }
}
