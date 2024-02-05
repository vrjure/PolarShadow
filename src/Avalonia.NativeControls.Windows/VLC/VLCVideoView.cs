using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.Controls.Windows
{
    internal class VLCVideoView : PlatformView, IPlatformVideoView
    {
        private Window _floatingContent;
        private bool _isAttached;

        private readonly IVirtualView VirtualView;

        public VLCVideoView(IVirtualView virtualView)
        {
            this.VirtualView = virtualView;

            this.VirtualView.AsHost().AttachedToVisualTree += VirtualView_AttachedToVisualTree;
            this.VirtualView.AsHost().DetachedFromVisualTree += VirtualView_DetachedFromVisualTree;
            this.VirtualView.AsHost().DetachedFromLogicalTree += VirtualView_DetachedFromLogicalTree;
            this.VirtualView.AsHost().LayoutUpdated += VirtualView_LayoutUpdated;
            this.VirtualView.AsHost().SizeChanged += VirtualView_SizeChanged;
        }

        private IRenderRoot _visualRoot;
        private IRenderRoot VisualRoot => _visualRoot ??= VirtualView.AsHost().GetVisualRoot();
        private Rect Bounds => VirtualView.AsHost().Bounds;
        private bool IsEffectivelyVisible => VirtualView.AsHost().IsEffectivelyVisible;

        public IVideoViewController Controller { get; set; }

        private MediaPlayer MediaPlayer => (Controller as VLController)?.MediaPlayer;

        private object _overlayContent;
        public object OverlayContent
        {
            get => _overlayContent;
            set
            {
                _overlayContent = value;
                if (_floatingContent != null)
                {
                    _floatingContent.Content = _overlayContent;
                }
            }
        }

        private bool _fullScreen;

        public event EventHandler PlatformClick;

        public bool FullScreen
        {
            get => _fullScreen;
            set => _fullScreen = value;
        }

        private TopLevel _topLevel => TopLevel.GetTopLevel(VirtualView as Visual);


        protected override IPlatformHandle OnCreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            if (this.MediaPlayer == null)
            {
                return createDefault();
            }

            var handler = createDefault();
            this.MediaPlayer.Hwnd = handler.Handle;
            return handler;
        }

        protected override void DestroyControl()
        {
            this.VirtualView.AsHost().AttachedToVisualTree -= VirtualView_AttachedToVisualTree;
            this.VirtualView.AsHost().DetachedFromVisualTree -= VirtualView_DetachedFromVisualTree;
            this.VirtualView.AsHost().DetachedFromLogicalTree -= VirtualView_DetachedFromLogicalTree;
            this.VirtualView.AsHost().LayoutUpdated -= VirtualView_LayoutUpdated;
            this.VirtualView.AsHost().SizeChanged -= VirtualView_SizeChanged;
            if (VisualRoot is Window root)
            {
                root.PositionChanged -= Window_PositionChanged;
            }

            if (MediaPlayer == null)
            {
                return;
            }

            MediaPlayer.Hwnd = IntPtr.Zero;
        }

        private void InitializeNativeOverlay()
        {
            if (VirtualView == null) return;
            if (!VirtualView.AsHost().IsAttachedToVisualTree()) return;

            InitializeDesktopNativeOverlay();
            UpdateOverlayPosition();
            ShowNativeOverlay(IsEffectivelyVisible);
        }

        private void InitializeDesktopNativeOverlay()
        {
            if (_floatingContent == null && OverlayContent != null)
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
                    DataContext = VirtualView.AsHost().DataContext,
                };

                if (OverlayContent != null)
                {
                    _floatingContent.Content = OverlayContent;
                }

                if (VisualRoot is Window root)
                {
                    root.PositionChanged += Window_PositionChanged;
                }
            }
        }

        private void Window_PositionChanged(object sender, PixelPointEventArgs e)
        {
            UpdateOverlayPosition();
        }

        private void UpdateOverlayPosition()
        {
            if (_floatingContent == null) return;

            if (_floatingContent.Width != Bounds.Width
                || _floatingContent.Height != Bounds.Height)
            {
                _floatingContent.Width = Bounds.Width;
                _floatingContent.Height = Bounds.Height;

                _floatingContent.MaxWidth = Bounds.Width;
                _floatingContent.MaxHeight = Bounds.Height;
            }

            PixelPoint newPosition;
            if (VisualRoot is Window root && root.WindowState == WindowState.FullScreen)
            {
                newPosition = new PixelPoint(0, 0);
            }
            else
            {
                newPosition = VirtualView.AsHost().PointToScreen(this.Bounds.Position);
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

        private void VirtualView_LayoutUpdated(object sender, EventArgs e)
        {
            if (!this.VirtualView.AsHost().IsLoaded)
            {
                UpdateOverlayPosition();
            }
        }

        private void VirtualView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOverlayPosition();
        }

        private void VirtualView_DetachedFromLogicalTree(object sender, LogicalTree.LogicalTreeAttachmentEventArgs e)
        {
            _floatingContent?.Close();
            _floatingContent = null;
        }

        private void VirtualView_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _isAttached = false;
            ShowNativeOverlay(false);
        }

        private void VirtualView_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _isAttached = true;
            InitializeNativeOverlay();
        }
    }
}
