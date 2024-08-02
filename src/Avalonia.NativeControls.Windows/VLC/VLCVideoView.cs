using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using LibVLCSharp.Shared;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace Avalonia.Controls.Windows
{
    internal class VLCVideoView : PlatformView, IPlatformVideoView
    {
        private ForegroundWindow _floatingContent;
        private bool _isAttached;

        private readonly IVirtualView VirtualView;

        public VLCVideoView(IVirtualView virtualView)
        {
            this.VirtualView = virtualView;
            this.VirtualView.AsHost().AttachedToVisualTree += VirtualView_AttachedToVisualTree;
            this.VirtualView.AsHost().DetachedFromVisualTree += VirtualView_DetachedFromVisualTree;
            this.VirtualView.AsHost().DetachedFromLogicalTree += VirtualView_DetachedFromLogicalTree;
        }

        private IRenderRoot _visualRoot;
        private IRenderRoot VisualRoot => _visualRoot ??= VirtualView.AsHost().GetVisualRoot();
        private Rect Bounds => VirtualView.AsHost().Bounds;
        private bool IsEffectivelyVisible => VirtualView.AsHost().IsEffectivelyVisible;

        public IVideoViewController Controller { get; set; }

        private MediaPlayer MediaPlayer => (Controller as IVLController)?.MediaPlayer;

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

        public event EventHandler PlatformClick;

        protected override IPlatformHandle OnCreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            if (this.MediaPlayer == null)
            {
                return createDefault();
            }

            var handler = createDefault();
            this.MediaPlayer.Hwnd = handler.Handle;
            InitializeNativeOverlay();
            return handler;
        }

        protected override void DestroyControl()
        {
            if (MediaPlayer == null)
            {
                return;
            }

            MediaPlayer.Hwnd = IntPtr.Zero;
            if (_floatingContent != null)
            {
                _floatingContent.Close();
                _floatingContent = null;
            }
        }

        private void InitializeNativeOverlay()
        {
            if (VirtualView == null) return;
            if (!VirtualView.AsHost().IsAttachedToVisualTree()) return;

            InitializeDesktopNativeOverlay();
        }

        private void InitializeDesktopNativeOverlay()
        {
            if (_floatingContent == null && OverlayContent != null)
            {
                _floatingContent = new ForegroundWindow(VirtualView as Control);
                
                if (OverlayContent != null)
                {
                    _floatingContent.Content = OverlayContent;
                }
            }
        }

        private void VirtualView_DetachedFromLogicalTree(object sender, LogicalTree.LogicalTreeAttachmentEventArgs e)
        {
            _floatingContent?.Close();
            _floatingContent = null;
        }

        private void VirtualView_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _isAttached = false;
        }

        private void VirtualView_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _isAttached = true;
            //InitializeNativeOverlay();
        }
    }
}
