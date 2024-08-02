using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.DependencyInjection;
using PolarShadow.Controls.Converters;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public partial class MediaPlayerController : ContentControl
    {
        private Button _previousBtn;
        private Button PreviousBtn
        {
            get => _previousBtn;
            set
            {
                if (_previousBtn != null)
                {
                    _previousBtn.Click -= PreviousBtn_Click;
                }

                _previousBtn = value;
                if (_previousBtn != null)
                {
                    _previousBtn.Click += PreviousBtn_Click;
                }
            }
        }

        private Button _playBtn;
        private Button PlayBtn
        {
            get => _playBtn;
            set
            {
                if (_playBtn != null)
                {
                    _playBtn.Click -= PlayBtn_Click;
                }

                _playBtn = value;
                if (_playBtn != null)
                {
                    _playBtn.Click += PlayBtn_Click;
                }
            }
        }

        private Button _nextBtn;
        private Button NextBtn
        {
            get => _nextBtn;
            set
            {
                if (_nextBtn != null)
                {
                    _nextBtn.Click -= NextBtn_Click;
                }

                _nextBtn = value;
                if (_nextBtn != null)
                {
                    _nextBtn.Click += NextBtn_Click;
                }
            }
        }

        private TextBlock _timeText;
        private TextBlock TimeText
        {
            get => _timeText;
            set => _timeText = value;
        }

        private TextBlock _lengthText;
        private TextBlock LengthText
        {
            get => _lengthText;
            set => _lengthText = value;
        }

        private Button _fullScreenBtn;
        private Button FullScreenBtn
        {
            get => _fullScreenBtn;
            set
            {
                if (_fullScreenBtn != null)
                {
                    _fullScreenBtn.Click -= FullScreenBtn_Click;
                }

                _fullScreenBtn = value;
                if (_fullScreenBtn != null)
                {
                    _fullScreenBtn.Click += FullScreenBtn_Click;
                }
            }
        }

        private Slider _part_slider;
        private Slider PartSlider
        {
            get => _part_slider;
            set
            {
                if (_part_slider != null)
                {
                    _part_slider.PointerMoved -= _part_slider_PointerMoved;
                    _part_slider.TemplateApplied -= _part_slider_TemplateApplied;
                }

                _part_slider = value;
                if (_part_slider != null)
                {
                    _part_slider.PointerMoved += _part_slider_PointerMoved;
                    _part_slider.TemplateApplied += _part_slider_TemplateApplied;
                }
            }
        }

        private Track _sliderTrack;
        private Border _part_root;
        private Panel _part_root_panel;

        private Border _part_tip;
        private TextBlock _part_tip_text;
        private bool _isShow;

        private readonly IDeviceService _deviceService;

        static MediaPlayerController()
        {
            ControllerProperty.Changed.AddClassHandler<MediaPlayerController>((o,e)=>o.OnPropertyChanged(e));
            TipProperty.Changed.AddClassHandler<MediaPlayerController>((o, e) => o.OnPropertyChanged(e));
            HoldingEvent.AddClassHandler<MediaPlayerController>((s, e) => s.OnHolding(e));
            Gestures.ScrollGestureEvent.AddClassHandler<MediaPlayerController>((s, e) => s.OnScroll(e));
            Gestures.ScrollGestureEndedEvent.AddClassHandler<MediaPlayerController>((s, e) => s.OnScrollEnd(e));

            if (OperatingSystem.IsWindows())
            {
                
            }
            else if (OperatingSystem.IsAndroid())
            {
                VirtualView.NativePointerReleasedEvent.AddClassHandler<MediaPlayerController>((s, e) => s.OnNativePointerReleased(e));
                VirtualView.NativePointerPressedEvent.AddClassHandler<MediaPlayerController>((s, e) => s.OnNativePointerPressed(e));
            }
        }

        public MediaPlayerController()
        {
            this.GestureRecognizers.Add(new ScrollGestureRecognizer() { CanHorizontallyScroll = true, CanVerticallyScroll = true });

            _deviceService = Ioc.Default.GetService<IDeviceService>();
        }

        public static readonly StyledProperty<IVideoViewController> ControllerProperty = AvaloniaProperty.Register<MediaPlayerController, IVideoViewController>(nameof(ControllerProperty));
        public IVideoViewController Controller
        {
            get => GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<MediaPlayerController, string>(nameof(Title));
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<string> TipProperty = AvaloniaProperty.Register<MediaPlayerController, string>(nameof(Tip));
        public string Tip
        {
            get => GetValue(TipProperty);
            set => SetValue(TipProperty, value);
        }

        public static readonly StyledProperty<bool> FullScreenProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(FullScreen));
        public bool FullScreen
        {
            get => GetValue(FullScreenProperty);
            set => SetValue(FullScreenProperty, value);
        }

        public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(IsLoading));
        public bool IsLoading
        {
            get => GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly StyledProperty<MediaMode> MediaModeProperty = AvaloniaProperty.Register<MediaPlayerController, MediaMode>(nameof(MediaMode));
        public MediaMode MediaMode
        {
            get => GetValue(MediaModeProperty);
            set => SetValue(MediaModeProperty, value);
        }

        public static StyledProperty<ICommand> PreviousCommandProperty = AvaloniaProperty.Register<MediaPlayerController, ICommand>(nameof(PreviousCommand));
        public ICommand PreviousCommand
        {
            get => GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        public static StyledProperty<ICommand> NextCommandProperty = AvaloniaProperty.Register<MediaPlayerController, ICommand>(nameof(NextCommand));
        public ICommand NextCommand
        {
            get => GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public static StyledProperty<ICommand> PlayPauseCommandProperty = AvaloniaProperty.Register<MediaPlayerController, ICommand>(nameof(PlayPauseCommand));
        public ICommand PlayPauseCommand
        {
            get => GetValue(PlayPauseCommandProperty);
            set => SetValue(PlayPauseCommandProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            PreviousBtn = e.NameScope.Get<Button>("Part_Previous");
            PlayBtn = e.NameScope.Get<Button>("Part_PlayPause");
            NextBtn = e.NameScope.Get<Button>("Part_Next");
            FullScreenBtn = e.NameScope.Get<Button>("Part_FullScreen");
            PartSlider = e.NameScope.Get<Slider>("Part_Slider");
            _part_root = e.NameScope.Get<Border>("Part_Root");
            TimeText = e.NameScope.Get<TextBlock>("Part_Time");
            LengthText = e.NameScope.Get<TextBlock>("Part_Length");
            _part_tip_text = e.NameScope.Get<TextBlock>("part_tip_text");
            _part_tip = e.NameScope.Get<Border>("part_tip");
            _part_root_panel = e.NameScope.Get<Panel>("Part_Root_Panel");

            Show();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == ControllerProperty)
            {
                ControllerPropertyChanged(change);
            }
            else if (change.Property == TipProperty)
            {
                var val = change.GetNewValue<string>();
                if (!string.IsNullOrEmpty(val))
                {
                    ShowTip(val);
                }
            }
        }

        private void ControllerPropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            var controller = arg.Sender as MediaPlayerController;

            if (arg.OldValue is IVideoViewController old)
            {
                old.Error -= controller.Media_Error;
            }

            if (arg.NewValue is IVideoViewController newVal)
            {
                newVal.Error += controller.Media_Error;
            }
        }

        private void Media_Error(object sender, EventArgs e)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                IsLoading = false;
                Tip = "播放错误!";
            });
        }
        public void ShowTip(string tip)
        {
            if (_part_tip_text != null)
            {
                _part_tip_text.Text = tip;
            }
            if (_part_tip != null && !string.IsNullOrEmpty(tip))
            {
                _part_tip.Opacity = 1;
                DispatcherTimer.RunOnce(() => 
                {
                   _part_tip.Opacity = 0;
                   Tip = string.Empty;
                }, TimeSpan.FromSeconds(3));
            }
        }

        public void OnPressed()
        {
            if (_part_root == null) return;

            if (IsShow())
            {
                Hide();
            }
            else
            {
                Show();

            }
        }

        private void Hide()
        {
            _part_root_panel.Cursor = new Cursor(StandardCursorType.None);
            _part_root.Opacity = 0;
            _isShow = false;
        }

        private void Show()
        {
            _part_root_panel.Cursor = Cursor.Default;
            _part_root.Opacity = 1;
            _isShow = true;
        }

        private bool IsShow() => _isShow;

        private void PreviousBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PreviousCommand?.Execute(null);
        }

        private void NextBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            NextCommand?.Execute(null);
        }

        private void PlayBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PlayPause();
        }

        private async void PlayPause()
        {
            PlayPauseCommand?.Execute(null);
            if (Controller == null)
            {
                return;
            }
            if (Controller.IsPlaying == true)
            {
                await Controller.PauseAsync();
            }
            else
            {
                await Controller.PlayAsync();
            }
        }

        private void FullScreenBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            FullScreen = !FullScreen;
            var topLevel = Ioc.Default.GetRequiredService<ITopLevelService>();
            topLevel.FullScreen = FullScreen;
        }

        private void _part_slider_TemplateApplied(object sender, TemplateAppliedEventArgs e)
        {
            _part_slider.TemplateApplied -= _part_slider_TemplateApplied;

            _sliderTrack = e.NameScope.Find<Track>("PART_Track");
        }

        private void _part_slider_PointerMoved(object sender, PointerEventArgs e)
        {
            if (_sliderTrack == null)
            {
                return;
            }

            var value = _sliderTrack.ValueFromPoint(e.GetCurrentPoint(_sliderTrack).Position);
            ToolTip.SetTip(_part_slider, TimeSpanToStringConverter.Instance.Convert(TimeSpanToDoubleConverter.Instance.ConvertBack(value, typeof(TimeSpan), default,default), typeof(string), default, default));
        }


        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            if (this.Controller == null) return;

            var halfWidth = e.NewSize.Width / 2;
            _leftRect = new Rect(0, 0, halfWidth, e.NewSize.Height);

            if(e.NewSize.Width < 500)
            {
                MediaMode = MediaMode.Min;
            }
            else if (e.NewSize.Width >= 500 && e.NewSize.Width < 800)
            {
                MediaMode = MediaMode.Simple;
            }
            else
            {
                MediaMode = MediaMode.Normal;
            }
        }
    }
}
