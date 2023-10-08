using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Avalonia.Android;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using LibVLCSharp.Platforms.Android;
using LibVLCSharp.Shared;
using Org.Videolan.Libvlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.NativeControls.Android
{
    internal class VideoView : PlatformView, IVLCPlatformView
    {
        private LibVLCSharp.Platforms.Android.VideoView _platformView;
        private AvaloniaView _overlayLayer;

        private readonly IVirtualView _virtualView;
        public VideoView(IVirtualView virtualView)
        {
            _virtualView = virtualView;
        }

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

                if (_mediaPlayer != null && _platformView != null)
                {
                    _platformView.MediaPlayer = _mediaPlayer;
                }
            }
        }

        private object _overlayContent;
        public object OverlayContent
        {
            get => _overlayContent;
            set
            {
                _overlayContent = value;
                if (_overlayContent is Visual ctl)
                {
                    ctl.DataContext = _virtualView.VirtualView.DataContext;
                }
                if (_overlayLayer != null)
                {
                    _overlayLayer.Content = value;
                }
            }
        }

        protected override IPlatformHandle CreateControl(IPlatformHandle parent)
        {
            var context = (parent as AndroidViewControlHandle)?.View.Context ?? global::Android.App.Application.Context;

            _platformView = new LibVLCSharp.Platforms.Android.VideoView(context)
            {
                MediaPlayer = this.MediaPlayer
            };
            _platformView.Click += _platformView_Click;
            
            //TODO AvaloniaView can not transparent, but maybe it's not impossible, waitting... maybe?
            //see https://github.com/AvaloniaUI/Avalonia/issues/10807
            _overlayLayer = new AvaloniaView(context);
            _overlayLayer.SetBackgroundColor(global::Android.Graphics.Color.Black);

            if (OverlayContent != null)
            {
                _overlayLayer.Content = new Border
                {
                    Child = OverlayContent as Control,
                    Background = Brushes.Black
                };
            }

            var container = new RelativeLayout(context);
            container.AddView(_platformView);
            var lp = new RelativeLayout.LayoutParams(WindowManagerLayoutParams.MatchParent, 120);
            lp.AddRule(LayoutRules.AlignParentBottom);
            container.AddView(_overlayLayer, lp);

            return new AndroidViewControlHandle(container);
        }

        private void _platformView_Click(object sender, EventArgs e)
        {
            if (_overlayLayer == null)
            {
                return;
            }

            if(_overlayLayer.Visibility == ViewStates.Visible)
            {
                _overlayLayer.Visibility = ViewStates.Gone;
            }
            else
            {
                _overlayLayer.Visibility = ViewStates.Visible;
            }
        }

        protected override void DestroyControl()
        {
            var platformView = Handle as AndroidViewControlHandle;
            if (platformView != null)
            {
                platformView.Destroy();
            }
        }
    }
}
