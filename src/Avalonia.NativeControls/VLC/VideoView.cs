using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.NativeControls;
using Avalonia.Data;
using LibVLCSharp.Shared;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using Avalonia.Media;
using System.Runtime.InteropServices;
using Avalonia.LogicalTree;

namespace Avalonia.NativeControls
{
    /// <summary>
    /// <see cref="https://github.com/radiolondra/AvaVLCWindow/blob/main/LibVLCSharp.Avalonia.Unofficial/VideoView.cs"/>
    /// TODO For Android
    /// </summary>
    public class VideoView : NativeControlHost, IVLCVirtualView
    {
        private Window _floatingContent;
        private ICollection<IDisposable> _disposables;
        private bool _isAttached;

        static VideoView()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ContentProperty.Changed.AddClassHandler<VideoView>((s, e) =>
                {
                    s.InitializeNativeOverlay();
                    s.UpdateOverlayPosition();
                });
                BoundsProperty.Changed.AddClassHandler<VideoView>((s, e) => s.UpdateOverlayPosition());
            }
        }

        public static readonly DirectProperty<VideoView, MediaPlayer> MediaPlayerProperty = AvaloniaProperty.RegisterDirect<VideoView, MediaPlayer>(
        nameof(MediaPlayer),
        o => o.MediaPlayer,
        (o, v) => o.MediaPlayer = v,
        defaultBindingMode: BindingMode.TwoWay);

        private MediaPlayer _mediaPlayer = null;
        public MediaPlayer MediaPlayer
        {
            get { return _mediaPlayer; }
            set
            {
                if (ReferenceEquals(_mediaPlayer, value))
                {
                    return;
                }
                _mediaPlayer = value;
                this.Handler.PlatformView.MediaPlayer = value;
            }
        }

        public static readonly StyledProperty<object> ContentProperty = ContentControl.ContentProperty.AddOwner<VideoView>();
        [Content]
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public Control VirtualView => this;

        public IVLCHandler Handler { get; set; }

        IViewHandler IVirtualView.Handler
        {
            get => this.Handler;
            set => this.Handler = (IVLCHandler)value;
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            this.Handler = NativeControlHandlers.GetHandler<IVLCHandler>();
            if (this.Handler == null)
            {
                return base.CreateNativeControlCore(parent);
            }

            this.Handler.SetVirtualView(this);

            if (this.Handler.PlatformView == null)
            {
                return base.CreateNativeControlCore(parent);
            }
       
            return this.Handler.PlatformView.CreateControl(parent, () => base.CreateNativeControlCore(parent)) ?? base.CreateNativeControlCore(parent);
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            this.Handler?.PlatformView?.DestroyControl(control);

            base.DestroyNativeControlCore(control);
        }

        private void InitializeNativeOverlay()
        {
            if (!this.IsAttachedToVisualTree()) return;
            
            if (_floatingContent == null && Content != null)
            {
                _floatingContent = new Window
                {
                    SystemDecorations = SystemDecorations.None,
                    TransparencyLevelHint = new WindowTransparencyLevel[] { WindowTransparencyLevel.Transparent },
                    Background = Brushes.Transparent,
                    SizeToContent = SizeToContent.Manual,
                    CanResize = false,
                    ShowInTaskbar = false,
                    Opacity = 1,
                    ZIndex = int.MaxValue,
                    DataContext = this.DataContext
                };

                _disposables = new List<IDisposable>
                {
                    _floatingContent.Bind(Window.ContentProperty, this.GetObservable(ContentProperty))
                };

                if (VisualRoot is Window root)
                {
                    root.PositionChanged += Window_PositionChanged;
                }
            }

            ShowNativeOverlay(IsEffectivelyVisible);
        }

        private void Window_PositionChanged(object sender, PixelPointEventArgs e)
        {
            UpdateOverlayPosition();
        }

        private void UpdateOverlayPosition()
        {
            if (_floatingContent == null) return;

            _floatingContent.Width = Bounds.Width;
            _floatingContent.Height = Bounds.Height;

            _floatingContent.MaxWidth = Bounds.Width;
            _floatingContent.MaxHeight = Bounds.Height;

            PixelPoint newPosition;
            if (VisualRoot is Window root && root.WindowState == WindowState.FullScreen)
            {
                newPosition = new PixelPoint(0, 0);
            }
            else
            {
                newPosition = this.PointToScreen(this.Bounds.Position);
            }

            if (newPosition != _floatingContent.Position)
            {
                _floatingContent.Position = newPosition;
            }
        }


        private void ShowNativeOverlay(bool visible)
        {
            if (_floatingContent == null || _floatingContent.IsVisible == visible) return;

            if (_isAttached && visible)
            {
                _floatingContent.Show(VisualRoot as Window);
            }
            else
            {
                _floatingContent.Hide();
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            _isAttached = true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InitializeNativeOverlay();
            }

        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            _isAttached = false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ShowNativeOverlay(false);
            }
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (_disposables != null)
                {
                    foreach (var item in _disposables)
                    {
                        item.Dispose();
                    }
                }
                _disposables = null;

                _floatingContent?.Close();
                _floatingContent = null;
            }
        }
    }
}
