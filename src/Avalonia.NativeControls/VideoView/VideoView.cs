using Avalonia.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using PolarShadow.Essentials;
using System;
using System.Diagnostics;

namespace Avalonia.Controls
{
    /// <summary>
    /// <see cref="https://github.com/radiolondra/AvaVLCWindow/blob/main/LibVLCSharp.Avalonia.Unofficial/VideoView.cs"/>
    /// </summary>
    public class VideoView : VirtualView, IVideoView
    {
        public VideoView() : base(Ioc.Default.GetRequiredService<IVideoViewHandler>())
        {

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

        public new IVideoViewHandler Handler => base.Handler as IVideoViewHandler;
    }
}
