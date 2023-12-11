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
using Avalonia.Input;

namespace Avalonia.NativeControls
{
    /// <summary>
    /// <see cref="https://github.com/radiolondra/AvaVLCWindow/blob/main/LibVLCSharp.Avalonia.Unofficial/VideoView.cs"/>
    /// </summary>
    public class VideoView : VirtualView, IVLCVirtualView
    {
        public VideoView() : base(NativeControlHandlers.GetHandler<IVLCHandler>())
        {

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
                if (this.Handler != null && this.Handler.PlatformView != null)
                {
                    this.Handler.PlatformView.MediaPlayer = value;
                }
            }
        }

        public static readonly DirectProperty<VideoView, bool> FullScreenProperty = AvaloniaProperty.RegisterDirect<VideoView, bool>(nameof(FullScreen),
            o => o.FullScreen,
            (o, v) => o.FullScreen = v,
            defaultBindingMode: BindingMode.OneWay);

        public event EventHandler PlatformClick;

        public bool FullScreen
        {
            get => this.Handler.PlatformView.FullScreen;
            set => this.Handler.PlatformView.FullScreen = value;
        }
        public new IVLCHandler Handler => base.Handler as IVLCHandler;

        public void OnPlatformClick()
        {
            PlatformClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
