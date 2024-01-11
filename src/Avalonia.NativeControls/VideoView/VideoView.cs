using System;

namespace Avalonia.Controls
{
    /// <summary>
    /// <see cref="https://github.com/radiolondra/AvaVLCWindow/blob/main/LibVLCSharp.Avalonia.Unofficial/VideoView.cs"/>
    /// </summary>
    public class VideoView : VirtualView, IVideoView
    {
        public VideoView() : base(NativeControls.GetHandler<IVideoViewHandler>())
        {

        }

        public static readonly DirectProperty<VideoView, bool> FullScreenProperty = AvaloniaProperty.RegisterDirect<VideoView, bool>(nameof(FullScreen), o => o.FullScreen, (o, v) => o.FullScreen = v);

        public bool FullScreen
        {
            get => this.Handler.PlatformView.FullScreen;
            set => this.Handler.PlatformView.FullScreen = value;
        }

        public static readonly DirectProperty<VideoView, IVideoViewController> ControllerProperty = AvaloniaProperty.RegisterDirect<VideoView, IVideoViewController>(nameof(Controller), o => o.Controller, (o, v) => o.Controller = v, defaultBindingMode : Data.BindingMode.OneWayToSource);
        public IVideoViewController Controller
        {
            get => this.Handler.PlatformView.Controller;
            set => this.Handler.PlatformView.Controller = value;
        }

        public event EventHandler PlatformClick;

        public void OnPlatformClick()
        {
            PlatformClick?.Invoke(this, EventArgs.Empty);
        }

        public new IVideoViewHandler Handler => base.Handler as IVideoViewHandler;
    }
}
