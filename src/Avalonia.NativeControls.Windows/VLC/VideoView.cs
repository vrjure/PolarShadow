using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls.Windows
{
    internal class VideoView : PlatformView, IVLCPlatformView
    {
        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            set
            {
                if (ReferenceEquals(_mediaPlayer, value))
                {
                    return;
                }

                _mediaPlayer = value;

                if (_mediaPlayer != null)
                {
                    _mediaPlayer.Hwnd = Handle;
                }
            }
        }
        protected override void CreateControl()
        {
            if (MediaPlayer == null)
            {
                return;
            }

            MediaPlayer.Hwnd = Handle;
        }

        protected override void DestroyControl()
        {
            if (MediaPlayer == null)
            {
                return;
            }

            MediaPlayer.Hwnd = IntPtr.Zero;
        }
    }
}
