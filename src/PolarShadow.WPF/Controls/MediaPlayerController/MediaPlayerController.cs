using CommunityToolkit.Mvvm.DependencyInjection;
using PolarShadow.Essentials;
using PolarShadow.WPF.Views;
using PolarShadows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    [TemplatePart(Name ="PART_Slider", Type = typeof(Slider))]
    [TemplatePart(Name ="PART_TOP", Type = typeof(UIElement))]
    [TemplatePart(Name ="PART_Bottom", Type = typeof(UIElement))]
    class MediaPlayerController : ContentControl
    {
        private static TimeSpan TimelineStep = TimeSpan.FromSeconds(5);
        private static double MouseMoveDistance = 15;
        private Point _lastMousPoint = new Point(0, 0);
        private CancellationTokenSource _tcs_AutoHideUI;

        public static readonly DependencyProperty ControllerProperty = DP.Register<MediaPlayerController, IVideoViewController>(nameof(Controller), PropertyChanged);
        public IVideoViewController Controller
        {
            get => (IVideoViewController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DP.Register<MediaPlayerController, string>(nameof(Title));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty FullScreenProperty = DP.Register<MediaPlayerController, bool>(nameof(FullScreen));
        public bool FullScreen
        {
            get => (bool)GetValue(FullScreenProperty);
            set => SetValue(FullScreenProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty = DP.Register<MediaPlayerController, bool>(nameof(IsLoading));
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            private set => SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty MediaModeProperty = DP.Register<MediaPlayerController, MediaMode>(nameof(MediaMode));
        public MediaMode MediaMode
        {
            get => (MediaMode)GetValue(MediaModeProperty);
            set => SetValue(MediaModeProperty, value);
        }

        public static readonly DependencyProperty TipProperty = DP.Register<MediaPlayerController, string>(nameof(Tip));
        public string Tip
        {
            get => (string)GetValue(TipProperty);
            set => SetValue(TipProperty, value);
        }

        public static readonly DependencyProperty IsPlayingProperty = DP.Register<MediaPlayerController, bool>(nameof(IsPlaying));
        public bool IsPlaying
        {
            get => (bool)GetValue(IsPlayingProperty);
            private set => SetValue(IsPlayingProperty, value);
        }

        public static readonly DependencyProperty VolumeProperty = DP.Register<MediaPlayerController, int>(nameof(Volume));
        public int Volume
        {
            get => (int)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public static readonly DependencyProperty PreviousCommandProperty = DP.Register<MediaPlayerController, ICommand>(nameof(PreviousCommand));
        public ICommand PreviousCommand
        {
            get => (ICommand)GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        public static readonly DependencyProperty NextCommandProperty = DP.Register<MediaPlayerController, ICommand>(nameof(NextCommand));
        public ICommand NextCommand
        {
            get => (ICommand)GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public static readonly DependencyProperty PlayPauseCommandProperty = DP.Register<MediaPlayerController, ICommand>(nameof(PlayPauseCommand));
        public ICommand PlayPauseCommand
        {
            get => (ICommand)GetValue(PlayPauseCommandProperty);
            set => SetValue(PlayPauseCommandProperty, value);
        }

        public static RoutedCommand Previous { get; private set; }
        public static RoutedCommand Next { get; private set; }
        public static RoutedCommand PlayPause { get; private set; }
        public static RoutedCommand FullScreenChanged { get; private set; }

        static MediaPlayerController()
        {
            InitializeCommands();
            
        }

        public MediaPlayerController()
        {
            this.CommandBindings.Add(new CommandBinding(Previous, PreviousExecuted, CanPreviousExecuted));
            this.CommandBindings.Add(new CommandBinding(Next, NextExecuted, CanNextExecuted));
            this.CommandBindings.Add(new CommandBinding(PlayPause, PlayPauseExecuted, CanPlayPauseExecuted));
            this.CommandBindings.Add(new CommandBinding(FullScreenChanged, FullScreenExecuted, CanFullScreenExecuted));
            
        }

        private Track _track;
        private UIElement _part_top;
        private UIElement _part_bottom;

        private UIElement PART_Top
        {
            get => _part_top;
            set
            {
                if (_part_top != null)
                {
                    _part_top.MouseUp -= PartTopBottomMouseUp;
                }
                _part_top = value;
                if(_part_top != null)
                {
                    _part_top.MouseUp += PartTopBottomMouseUp;
                }
            }
        }

        private UIElement PART_Bottom
        {
            get => _part_bottom;
            set
            {
                if (_part_bottom != null)
                {
                    _part_bottom.MouseUp -= PartTopBottomMouseUp;
                }
                _part_bottom = value;
                if (_part_bottom != null)
                {
                    _part_bottom.MouseUp += PartTopBottomMouseUp;
                }
            }
        }

        private void PartTopBottomMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var slider = (Slider)GetTemplateChild("PART_Slider");
            slider.Loaded += Slider_Loaded;
            slider.MouseMove += Slider_MouseMove;

            PART_Top = (UIElement)GetTemplateChild("PART_Top");
            PART_Bottom = (UIElement)GetTemplateChild("PART_Bottom");
        }

        private static void InitializeCommands()
        {
            Previous = new RoutedCommand(nameof(Previous), typeof(MediaPlayerController));
            Next = new RoutedCommand(nameof(Next), typeof(MediaPlayerController));
            PlayPause = new RoutedCommand(nameof(Next), typeof(MediaPlayerController));
            FullScreenChanged = new RoutedCommand(nameof(FullScreenChanged), typeof(MediaPlayerController));
        }

        private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as MediaPlayerController;
            if (e.Property == ControllerProperty)
            {
                element.SetBinding(IsPlayingProperty, new Binding(nameof(Controller.IsPlaying)) { Source = e.NewValue, Mode = BindingMode.OneWay});
                if (e.OldValue is IVideoViewController old)
                {
                    old.Buffering -= element.Buffering;
                }

                if (e.NewValue is IVideoViewController n)
                {
                    n.Buffering += element.Buffering;
                }
            }
        }

        private void PreviousExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PreviousCommand?.Execute(e.Parameter);
        }
        private void CanPreviousExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PreviousCommand != null && PreviousCommand.CanExecute(e.Parameter);
        }

        private void NextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            NextCommand?.Execute(e.Parameter);
        }
        private void CanNextExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NextCommand != null && NextCommand.CanExecute(e.Parameter);
        }

        private void PlayPauseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PlayPauseCommand?.Execute(e.Parameter);
        }
        private void CanPlayPauseExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PlayPauseCommand != null && PlayPauseCommand.CanExecute(e.Parameter);
        }

        private void CanFullScreenExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void FullScreenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FullScreen = !FullScreen;
            var top = Ioc.Default.GetRequiredService<TopLayoutView>();
            if (FullScreen)
            {
                top.FullScreen();
            }
            else
            {
                top.NormalScreen();
            }
        }

        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            var slider = sender as Slider;
            _track = (Track)slider.Template.FindName("PART_Track", slider);
        }

        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            var slider = sender as Slider;
            if (_track == null) return;
            ToolTipService.SetToolTip(slider, TimeSpan.FromSeconds(_track.ValueFromPoint(e.GetPosition(slider))).ToString(@"hh\:mm\:ss"));
        }

        private void Buffering(object sender, float e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsLoading = e < 100;
            });
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (sizeInfo.WidthChanged)
            {
                MediaMode = sizeInfo.NewSize.Width switch
                {
                    < 500 => MediaMode.Min,
                    >=500 and <800 => MediaMode.Simple,
                    _ => MediaMode.Normal
                };
            }
        }

        protected override async void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                if (FullScreen)
                {
                    FullScreen = false;
                    var top = Ioc.Default.GetRequiredService<TopLayoutView>();
                    top.NormalScreen();
                }
            }
            else if (Controller != null)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        Controller.Time -= TimelineStep;
                        break;
                    case Key.Right:
                        Controller.Time += TimelineStep;
                        break;
                    case Key.Up:
                        Controller.Volume += 1;
                        break;
                    case Key.Down:
                        Controller.Volume -= 1;
                        break;
                    case Key.Space:
                        if (Controller.IsPlaying)
                            await Controller.PauseAsync();
                        else
                            await Controller.PlayAsync();
                        break;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var point = e.GetPosition(this);
            var delta = point - _lastMousPoint;
            if (Math.Abs(delta.X) > MouseMoveDistance || Math.Abs(delta.Y) > MouseMoveDistance)
            {
                _lastMousPoint = point;
                ShowUI();
                CancelAutoHideUI();
            }

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsUIShow)
            {
                HideUI();
            }
            else
            {
                ShowUI();
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (IsUIShow) 
            {
                AutoHideUI();
            }
        }

        private void HideUI()
        {
            _part_top.Visibility = _part_bottom.Visibility = Visibility.Collapsed;
            this.Cursor = Cursors.None;
        }

        private void ShowUI()
        {
            _part_top.Visibility = _part_bottom.Visibility = Visibility.Visible;
            this.Cursor = Cursors.Arrow;
        }

        private bool IsUIShow => _part_top.Visibility == Visibility.Visible;

        private async void AutoHideUI()
        {
            if (_tcs_AutoHideUI == null || _tcs_AutoHideUI.IsCancellationRequested)
            {
                _tcs_AutoHideUI?.Dispose();
                _tcs_AutoHideUI = new CancellationTokenSource();
            }
            try
            {
                await Task.Delay(5000, _tcs_AutoHideUI.Token);
                HideUI();
            }
            catch { }
        }
        
        private void CancelAutoHideUI()
        {
            if (_tcs_AutoHideUI != null)
            {
                _tcs_AutoHideUI.Cancel();
                _tcs_AutoHideUI.Dispose();
                _tcs_AutoHideUI = null;
            }
        }
    }
}
