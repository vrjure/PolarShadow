using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using PolarShadow.Controls.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public class MediaPlayerController : ContentControl, IMediaPlayerController
    {
        private static readonly TimeSpan _ignore = TimeSpan.FromSeconds(1);
        private long _lastTime = 0;
        private Point _cursorPoint;
        private IDisposable _hideDisposable;

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

        static MediaPlayerController()
        {
            ControllerProperty.Changed.AddClassHandler<MediaPlayerController>((o,e)=>o.ControllerPropertyChanged(e));
            TimeProperty.Changed.AddClassHandler<MediaPlayerController>((o, e) => o.TimePropertyChanged(e));
        }

        public static readonly StyledProperty<IVideoViewController> ControllerProperty = AvaloniaProperty.Register<MediaPlayerController, IVideoViewController>(nameof(Controller));
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

        public static readonly StyledProperty<TimeSpan> LengthProperty = AvaloniaProperty.Register<MediaPlayerController, TimeSpan>(nameof(Length));
        public TimeSpan Length
        {
            get => GetValue(LengthProperty);
            private set => SetValue(LengthProperty, value);
        }

        public static readonly StyledProperty<TimeSpan> TimeProperty = AvaloniaProperty.Register<MediaPlayerController, TimeSpan>(nameof(Time));
        public TimeSpan Time
        {
            get => GetValue(TimeProperty);
            private set => SetValue(TimeProperty, value);
        }

        public static readonly StyledProperty<bool> IsPlayingProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(IsPlaying));
        public bool IsPlaying
        {
            get => GetValue(IsPlayingProperty);
            private set => SetValue(IsPlayingProperty, value);
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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            //PreviousBtn = e.NameScope.Get<Button>("Part_Previous");
            PlayBtn = e.NameScope.Get<Button>("Part_PlayPause");
            //NextBtn = e.NameScope.Get<Button>("Part_Next");
            FullScreenBtn = e.NameScope.Get<Button>("Part_FullScreen");
            PartSlider = e.NameScope.Get<Slider>("Part_Slider");
            _part_root = e.NameScope.Get<Border>("Part_Root");          
        }

        private void ControllerPropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            var controller = arg.Sender as MediaPlayerController;

            if (arg.OldValue is IVideoViewController old)
            {
                old.Playing -= controller.Media_Playing;
                old.Paused -= controller.Media_Paused;
                old.LengthChanged -= controller.LengthChanged;
                old.TimeChanged -= controller.TimeChanged;
            }

            if (arg.NewValue is IVideoViewController newVal)
            {
                newVal.Playing += controller.Media_Playing;
                newVal.Paused += controller.Media_Paused;
                newVal.LengthChanged += controller.LengthChanged;
                newVal.TimeChanged += controller.TimeChanged;
            }
        }

        private void TimePropertyChanged(AvaloniaPropertyChangedEventArgs arg)
        {
            var controller = arg.Sender as MediaPlayerController;
            if (controller == null || controller.Controller == null || arg.NewValue is not TimeSpan)
            {
                return;
            }

            var newValue = (TimeSpan)arg.NewValue;
            if (Math.Abs(newValue.TotalMilliseconds - controller.Controller.Time.TotalMilliseconds) < _ignore.TotalMilliseconds)
            {
                return;
            }
            controller.Controller.Time = TimeSpan.FromMilliseconds((long)newValue.TotalMilliseconds);
        }

        private void TimeChanged(object sender, TimeSpan e)
        {
            if (Math.Abs(e.TotalMilliseconds - _lastTime) < _ignore.TotalMilliseconds)
            {
                return;
            }
            _lastTime = (long)e.TotalMilliseconds / 1000 * 1000;

            Dispatcher.UIThread.Post(() =>
            {
                this.Time = TimeSpan.FromMilliseconds(_lastTime);
            });
        }

        private void LengthChanged(object sender, TimeSpan e)
        {
            var length = e.TotalMilliseconds > 0 ? e.TotalMilliseconds : 0;
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
            });
        }

        private void Media_Paused(object sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                IsPlaying = false;
            });
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled || e.KeyModifiers != KeyModifiers.None || _part_slider == null) return;

            var target = Time;
            var handled = false;
            switch (e.Key)
            {
                case Key.Left:
                    target = target - TimeSpan.FromSeconds(_part_slider.SmallChange);
                    handled = true;
                    break;
                case Key.Right:
                    target = target + TimeSpan.FromSeconds(_part_slider.SmallChange);
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

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_part_root == null || _cursorPoint == new Point(0,0))
            {
                return;
            }
            var current = e.GetPosition(this);
            var dis = Point.Distance(_cursorPoint, current);
            if (dis > 10)
            {
                Show();
            }
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);

            if (IsShow())
            {
                _hideDisposable = DispatcherTimer.RunOnce(() =>
                {
                    Hide();
                }, TimeSpan.FromSeconds(5), DispatcherPriority.Default);
            }
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);

            if (_hideDisposable != null)
            {
                _hideDisposable.Dispose();
                _hideDisposable= null;
            }

            if (!IsShow())
            {
                Show();
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (_part_root == null) return;

            if (IsShow())
            {
                Hide();
                _cursorPoint = e.GetPosition(this);
            }
            else
            {
                Show();
                _cursorPoint = e.GetPosition(this);

            }
        }

        private void Hide()
        {
            _part_root.Cursor = new Cursor(StandardCursorType.None);
            _part_root.Opacity = 0;
        }

        private void Show()
        {
            _part_root.Cursor = Cursor.Default;
            _part_root.Opacity = 1;
        }

        private bool IsShow() => _part_root?.Opacity == 1;

        private void PreviousBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (PreviousCommand == null) return;
            if (PreviousCommand.CanExecute(default))
            {
                PreviousCommand.Execute(default);
            }
            
        }

        private void NextBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (NextCommand == null) return;
            if (NextCommand.CanExecute(default))
            {
                NextCommand.Execute(default);
            }
        }

        private void PlayBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Controller == null)
            {
                return;
            }
            if (Controller.IsPlaying)
            {
                Controller.Pause();
            }
            else
            {
                Controller.Play();
            }
        }

        private void FullScreenBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            FullScreen = !FullScreen;
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
    }
}
