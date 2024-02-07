using Android.Content;
using Android.Runtime;
using Android.Views;
using LibVLCSharp.Shared;
using Org.Videolan.Libvlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    internal class AVideoView : LibVLCSharp.Platforms.Android.VideoView
    {
        private int _trackid;
        private bool _isPlaying;
        private bool _surfaceDestroyed;
        public AVideoView(Context context) : base(context)
        {
        }

        public override void OnSurfacesCreated(IVLCVout vout)
        {
            base.OnSurfacesCreated(vout);
            _surfaceDestroyed = false;
        }

        public override void OnSurfacesDestroyed(IVLCVout vout)
        {
            base.OnSurfacesDestroyed(vout);
            _surfaceDestroyed = true;
        }

        protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            if (!_surfaceDestroyed || MediaPlayer == null) return;

            if (visibility == ViewStates.Visible)
            {
                var cache = MediaPlayer;
                MediaPlayer = null;
                MediaPlayer = cache;
                MediaPlayer.SetVideoTrack(_trackid);
                if (_isPlaying)
                {
                    MediaPlayer.Play();
                }
            }
            else
            {
                _trackid = MediaPlayer.VideoTrack;
                MediaPlayer.SetVideoTrack(-1);

                _isPlaying = MediaPlayer.IsPlaying;
                if (_isPlaying)
                {
                    MediaPlayer.Pause();
                }
            }
        }
    }
}
