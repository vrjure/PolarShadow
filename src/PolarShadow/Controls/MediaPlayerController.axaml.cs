using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LibVLCSharp.Shared;
using System;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public partial class MediaPlayerController : UserControl
    {
        static MediaPlayerController()
        {
            MediaPlayerProperty.Changed.Subscribe(MediaPlayerPropertyChanged);
        }
        public MediaPlayerController()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<LibVLCSharp.Shared.MediaPlayer> MediaPlayerProperty = AvaloniaProperty.Register<MediaPlayerController, LibVLCSharp.Shared.MediaPlayer>(nameof(MediaPlayer));
        public LibVLCSharp.Shared.MediaPlayer MediaPlayer
        {
            get => GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<MediaPlayerController, string>(nameof(Title));
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<bool> BackButtonProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(BackButton));
        public bool BackButton
        {
            get => GetValue(BackButtonProperty);
            set => SetValue(BackButtonProperty, value);
        }

        public static readonly StyledProperty<TimeSpan> LengthProperty = AvaloniaProperty.Register<MediaPlayerController, TimeSpan>(nameof(Length));
        public TimeSpan Length
        {
            get => GetValue(LengthProperty);
            set => SetValue(LengthProperty, value);
        }

        public static readonly StyledProperty<TimeSpan> TimeProperty = AvaloniaProperty.Register<MediaPlayerController, TimeSpan>(nameof(Time));
        public TimeSpan Time
        {
            get => GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly StyledProperty<bool> IsPlayingProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(IsPlaying));
        public bool IsPlaying
        {
            get => GetValue(IsPlayingProperty);
            set => SetValue(IsPlayingProperty, value);
        }

        public static readonly StyledProperty<ICommand> PreviousCommandProperty = AvaloniaProperty.Register<MediaPlayerController, ICommand>(nameof(PreviousCommand));
        public ICommand PreviousCommand
        {
            get => GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> NextCommandProperty = AvaloniaProperty.Register<MediaPlayerController, ICommand>(nameof(NextCommand));
        public ICommand NextCommand
        {
            get => GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> PlayPauseCommandProperty = AvaloniaProperty.Register<MediaPlayerController, ICommand>(nameof(PlayPauseCommand));
        public ICommand PlayPauseCommand
        {
            get => GetValue(PlayPauseCommandProperty);
            set => SetValue(PlayPauseCommandProperty, value);
        }

        private void PreviousClick(object sender, RoutedEventArgs arg)
        {
            if (PreviousCommand == null)
            {
                return;
            }
            if(PreviousCommand.CanExecute(default))
            PreviousCommand.Execute(default);
        }

        private void NextClick(object sender, RoutedEventArgs arg)
        {
            if (NextCommand == null)
            {
                return;
            }
            if (NextCommand.CanExecute(default))
            NextCommand.Execute(default);
        }

        private void PlayPauseClick(object sender, RoutedEventArgs arg)
        {
            if (MediaPlayer == null)
            {
                return;
            }
            if (MediaPlayer.IsPlaying)
            {
                if (MediaPlayer.CanPause)
                {
                    MediaPlayer.Pause();
                }
            }
            else
            {
                MediaPlayer.Play();
            }

            if (PlayPauseCommand == null)
            {
                return;
            }

            if (PlayPauseCommand.CanExecute(IsPlaying))
            {
                PlayPauseCommand.Execute(IsPlaying);
            }
        }

        private static void MediaPlayerPropertyChanged(AvaloniaPropertyChangedEventArgs<LibVLCSharp.Shared.MediaPlayer> arg)
        {
            var controller = arg.Sender as MediaPlayerController;

            if (arg.OldValue.HasValue && arg.OldValue.Value != null)
            {
                var old = arg.OldValue.Value;
                old.MediaChanged -= controller.MediaChanged;
                old.Playing -= controller.Media_Playing;
                old.Paused -= controller.Media_Paused;
            }

            if (arg.NewValue.HasValue && arg.NewValue.Value != null)
            {
                var newVal = arg.NewValue.Value;

                newVal.MediaChanged += controller.MediaChanged;
                newVal.Playing += controller.Media_Playing;
                newVal.Paused += controller.Media_Paused;
            }
        }

        private void Media_Playing(object sender, EventArgs e)
        {
            IsPlaying = true;
        }

        private void Media_Paused(object sender, EventArgs e)
        {
            IsPlaying = false;
        }

        private void MediaChanged(object sender, LibVLCSharp.Shared.MediaPlayerMediaChangedEventArgs e)
        {
            var mediaPlayer = sender as LibVLCSharp.Shared.MediaPlayer;

            var length = mediaPlayer.Length > 0 ? mediaPlayer.Length : 0;
            var time = mediaPlayer.Time > 0 ? mediaPlayer.Time : 0;

            this.Length = TimeSpan.FromMilliseconds(length);
            this.Time = TimeSpan.FromMilliseconds(time);
        }
    }
}
