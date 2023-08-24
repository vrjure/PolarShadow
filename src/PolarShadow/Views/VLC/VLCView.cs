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
        public static LibVLC LibVLC => _libVlc;

        public readonly static BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer), typeof(MediaPlayer), typeof(VLCView));
        public MediaPlayer MediaPlayer
        {
            get => (MediaPlayer)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

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
