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
            MediaControllerProperty.Changed.AddClassHandler<MediaPlayerController>((o,e)=>o.MediaControllerPropertyChanged(e));
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

        public static readonly StyledProperty<IMediaController> MediaControllerProperty = AvaloniaProperty.Register<MediaPlayerController, IMediaController>(nameof(MediaController));
        public IMediaController MediaController
        {
            get => GetValue(MediaControllerProperty);
            set => SetValue(MediaControllerProperty, value);
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

        private void MediaControllerPropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            var controller = arg.Sender as MediaPlayerController;

            if (arg.OldValue is IMediaController old && old.Controller != null)
            {
                old.Controller.Error -= controller.Media_Error;
                old.PropertyChanged -= ControllerPropertyChanged;
            }

            if (arg.NewValue is IMediaController newVal && newVal.Controller != null)
            {
                newVal.Controller.Error += controller.Media_Error;
                newVal.PropertyChanged += ControllerPropertyChanged;
            }
        }

        private void ControllerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IMediaController.Tip)))
            {
                if (!string.IsNullOrEmpty(MediaController.Tip))
                {
                    ShowTip(MediaController.Tip);
                }
            }
            else if (e.PropertyName.Equals(nameof(IMediaController.MediaMode)))
            {
                SetPlayMode(MediaController.MediaMode);
            }
            else if (e.PropertyName.Equals(nameof(IMediaController.FullScreen)))
            {
                if (!MediaController.FullScreen && _deviceService != null)
                {
                    _deviceService.Brightness = _deviceService.SystemBrightness;
                }
            }
        }

        private void Media_Error(object sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                MediaController.IsLoading = false;
                MediaController.Tip = "播放错误!";
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
                    MediaController.Tip = string.Empty;
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
            (MediaController as MediaController)?.OnPreviousClick();
        }

        private void NextBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            (MediaController as MediaController)?.OnNextClick();
        }

        private void PlayBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PlayPause();
        }

        private async void PlayPause()
        {
            if (MediaController?.Controller == null)
            {
                return;
            }
            if (MediaController?.Controller.IsPlaying == true)
            {
                await MediaController.Controller.PauseAsync();
            }
            else
            {
                await MediaController.Controller.PlayAsync();
            }
        }

        private void FullScreenBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MediaController.FullScreen = !MediaController.FullScreen;
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
            if (this.MediaController == null) return;

            var halfWidth = e.NewSize.Width / 2;
            _leftRect = new Rect(0, 0, halfWidth, e.NewSize.Height);

            if(e.NewSize.Width < 500)
            {
                this.MediaController.MediaMode = MediaMode.Min;
            }
            else if (e.NewSize.Width >= 500 && e.NewSize.Width < 800)
            {
                this.MediaController.MediaMode = MediaMode.Simple;
            }
            else
            {
                this.MediaController.MediaMode = MediaMode.Normal;
            }
        }
    }
}
