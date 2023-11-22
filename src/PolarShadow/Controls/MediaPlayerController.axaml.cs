using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using SQLitePCL;
using System;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public partial class MediaPlayerController : UserControl
    {
        private static readonly TimeSpan _ignore = TimeSpan.FromSeconds(1);
        private long _lastTime = 0;
        static MediaPlayerController()
        {
            MediaPlayerProperty.Changed.Subscribe(MediaPlayerPropertyChanged);
            TimeProperty.Changed.Subscribe(TimePropertyChanged);
        }
        public MediaPlayerController()
        {
            InitializeComponent();
            Initialize();
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

        //public static readonly StyledProperty<bool> BackButtonProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(BackButton));
        //public bool BackButton
        //{
        //    get => GetValue(BackButtonProperty);
        //    set => SetValue(BackButtonProperty, value);
        //}

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
            private set => SetValue(IsPlayingProperty, value);
        }

        public static readonly StyledProperty<bool> ShowNextProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(ShowNext));
        public bool ShowNext
        {
            get => GetValue(ShowNextProperty);
            set => SetValue(ShowNextProperty, value);
        }

        public static readonly StyledProperty<bool> ShowPreviousProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(ShowPreviousProperty));
        public bool ShowPrevious
        {
            get => GetValue(ShowPreviousProperty);
            set => SetValue(ShowPreviousProperty, value);
        }

        public static readonly StyledProperty<bool> FullScreenProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(FullScreen));
        public bool FullScreen
        {
            get => GetValue(FullScreenProperty);
            set => SetValue(FullScreenProperty, value);
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

        private void FullScreenClick(object sender, RoutedEventArgs arg)
        {
            FullScreen = !FullScreen;
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
                old.LengthChanged -= controller.LengthChanged;
                old.TimeChanged -= controller.TimeChanged;
            }

            if (arg.NewValue.HasValue && arg.NewValue.Value != null)
            {
                var newVal = arg.NewValue.Value;

                newVal.MediaChanged += controller.MediaChanged;
                newVal.Playing += controller.Media_Playing;
                newVal.Paused += controller.Media_Paused;
                newVal.LengthChanged += controller.LengthChanged;
                newVal.TimeChanged += controller.TimeChanged;
            }
        }

        private static void TimePropertyChanged(AvaloniaPropertyChangedEventArgs<TimeSpan> arg)
        {
            var controller = arg.Sender as MediaPlayerController;
            if (controller == null || controller.MediaPlayer == null || !arg.NewValue.HasValue)
            {
                return;
            }

            var newValue = arg.NewValue.Value;
            if (Math.Abs((newValue.TotalMilliseconds - controller.MediaPlayer.Time)) < _ignore.TotalMilliseconds)
            {
                return;
            }
            controller.MediaPlayer.Time = (long)newValue.TotalMilliseconds;
        }

        private void TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            if (Math.Abs(e.Time - _lastTime) < _ignore.TotalMilliseconds)
            {
                return;
            }
            _lastTime = e.Time / 1000 * 1000;

            Dispatcher.UIThread.Post(() =>
            {
                this.Time = TimeSpan.FromMilliseconds(_lastTime);
            });
        }

        private void LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            var length = e.Length > 0 ? e.Length : 0;
            Dispatcher.UIThread.Post(() =>
            {
                this.Length = TimeSpan.FromMilliseconds(length);
            });
        }

        private void Media_Playing(object sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                IsPlaying = true;
                Unloading();
            });
        }

        private void Media_Paused(object sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                IsPlaying = false;
            });
        }

        private void MediaChanged(object sender, LibVLCSharp.Shared.MediaPlayerMediaChangedEventArgs e)
        {
            Loading();
        }

        private void Initialize()
        {
            part_slider.SmallChange = 5;
            part_slider.LargeChange = 15;

            if (OperatingSystem.IsWindows())
            {
                part_panel.Classes.Add("autoHide");
            }
            else if (OperatingSystem.IsAndroid())
            {
                part_panel.Classes.Add("android");
                part_panel.RowDefinitions = new RowDefinitions("auto, auto, auto");
                //part_panel.PointerReleased += Part_panel_PointerReleased;//TODO waitting...
                part_top.IsVisible = false;
            }
        }

        private void Part_panel_PointerReleased(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            if (part_panel.Opacity == 1)
            {
                part_panel.Opacity = 0;
            }
            else
            {
                part_panel.Opacity = 1;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled || e.KeyModifiers != KeyModifiers.None) return;

            var target = Time;
            var handled = false;
            switch (e.Key)
            {
                case Key.Left:
                    target = target - TimeSpan.FromSeconds(part_slider.SmallChange);
                    handled = true;
                    break;
                case Key.Right:
                    target = target + TimeSpan.FromSeconds(part_slider.SmallChange);
                    handled = true;
                    break;
            }

            e.Handled = handled;
            if (target < TimeSpan.Zero)
            {
                target = TimeSpan.Zero;
            }
            else if (target > Length)
            {
                target = Length;
            }

            Time = target;
        }

        private void Loading()
        {
            //part_loading.IsVisible = true;
            //part_loading.Classes.Add("spin");
        }

        private void Unloading()
        {
            //part_loading.IsVisible = false;
            //part_loading.Classes.Remove("spin");
        }
    }
}
