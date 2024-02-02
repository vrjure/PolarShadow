﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
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
    public partial class MediaPlayerController : ContentControl
    {
        private static readonly TimeSpan _ignore = TimeSpan.FromSeconds(1);
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

        private Border _part_tip;
        private TextBlock _part_tip_text;

        static MediaPlayerController()
        {
            MediaControllerProperty.Changed.AddClassHandler<MediaPlayerController>((o,e)=>o.MediaControllerPropertyChanged(e));
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

            SetPlayMode(MediaMode.Simple);
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
        }

        private void Media_Error(object sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                MediaController.IsLoading = false;
                ShowTip("播放错误!");
            });
        }
        public void ShowTip(string tip)
        {
            if (_part_tip_text != null)
            {
                _part_tip_text.Text = tip;
            }
            if (_part_tip != null)
            {
                _part_tip.Opacity = 1;
                DispatcherTimer.RunOnce(() => _part_tip.Opacity = 0, TimeSpan.FromSeconds(3));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled || e.KeyModifiers != KeyModifiers.None || _part_slider == null) return;

            var target = MediaController.Controller.Time;
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
            else if (target > MediaController.Controller.Length)
            {
                target = MediaController.Controller.Length;
            }

            MediaController.Controller.Time = target;
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
            (MediaController as MediaController)?.OnPreviousClick();
        }

        private void NextBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            (MediaController as MediaController)?.OnNextClick();
        }

        private async void PlayBtn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
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
            if(e.NewSize.Width < 500)
            {
                this.MediaController.MediaMode = MediaMode.Min;
            }
            else if (e.NewSize.Width >= 500 && e.NewSize.Width < 1000)
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
