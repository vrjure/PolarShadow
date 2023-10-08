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
            ContentProperty.Changed.AddClassHandler<VideoView>((s, e) =>
            {
                s.SetOverlayContent();
            });

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
            this.SetOverlayContent();

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

        private void SetOverlayContent()
        {
            if (this.Handler == null)
            {
                return;
            }

            this.Handler.PlatformView.OverlayContent = Content;
        }

    }
}
