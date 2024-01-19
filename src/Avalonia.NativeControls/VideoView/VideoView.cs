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

        private bool _fullSceen;
        public bool FullScreen
        {
            get => _fullSceen;
            set
            {
                if(SetAndRaise(FullScreenProperty, ref _fullSceen, value))
                {
                    this.Handler.PlatformView.FullScreen = _fullSceen;
                }
            }
        }

        public static readonly DirectProperty<VideoView, IVideoViewController> ControllerProperty = AvaloniaProperty.RegisterDirect<VideoView, IVideoViewController>(nameof(Controller), o => o.Controller, (o, v) => o.Controller = v);

        private IVideoViewController _controller;
        public IVideoViewController Controller
        {
            get => _controller;
            set
            {
                if(SetAndRaise(ControllerProperty, ref _controller, value))
                {
                    this.Handler.PlatformView.Controller = _controller;
                }
            }
        }

        public event EventHandler PlatformClick;

        public void OnPlatformClick()
        {
            PlatformClick?.Invoke(this, EventArgs.Empty);
        }

        public new IVideoViewHandler Handler => base.Handler as IVideoViewHandler;
    }
}
