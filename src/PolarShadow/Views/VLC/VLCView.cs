using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Views
{
    internal class VLCView : View, IVLCView
    {
        private static LibVLC _libVlc;
        private static object _lock = new object();

        public MediaPlayer MediaPlayer
        {
            get => (Handler?.PlatformView as IVideoView)?.MediaPlayer;
        }

        public static LibVLC LibVLC => _libVlc;

        public static void InitializeLibVlc(params string[] options)
        {
            lock (_lock)
            {
                if (_libVlc != null) return;
#if DEBUG
                _libVlc = new LibVLC(true, options);
#else
                _libVlc = new LibVLC(options);
#endif
            }
        }
    }
}
