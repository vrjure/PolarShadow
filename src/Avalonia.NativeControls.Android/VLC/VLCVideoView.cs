﻿using Android.Views;
using Android.Widget;
using Avalonia.Android;
using Avalonia.Media;
using Avalonia.Platform;
using LibVLCSharp.Shared;
using System;

namespace Avalonia.Controls.Android
{
    internal class VLCVideoView : PlatformView, IPlatformVideoView
    {
        private AvaloniaView _overlayLayer;// new well be cause memoryleak? not sure. so try use static.
        private LibVLCSharp.Platforms.Android.VideoView _platformView;

        private readonly IVirtualView _virtualView;
        public VLCVideoView(IVirtualView virtualView)
        {
            _virtualView = virtualView;
        }

        public IVideoViewController Controller { get; set; }


        public MediaPlayer MediaPlayer => (Controller as VLController)?.MediaPlayer;

        private object _overlayContent;
        public object OverlayContent
        {
            get => _overlayContent;
            set
            {
                _overlayContent = value;
                if (_overlayContent is Visual ctl)
                {
                    ctl.DataContext = _virtualView.AsHost().DataContext;
                }
                if (_overlayLayer != null)
                {
                    _overlayLayer.Content = value;
                }
            }
        }

        private bool _fullScreen;
        public bool FullScreen
        {
            get => _fullScreen;
            set
            {
                if (_fullScreen == value)
                {
                    return;
                }
                _fullScreen = value;
                if (_fullScreen)
                {
                    SetFullScreen();
                }
                else
                {
                    ExitFullScreen();
                }
            }
        }

        private TopLevel _topLevelCache;

        public event EventHandler PlatformClick;

        private TopLevel _topLevel => _topLevelCache ??= TopLevel.GetTopLevel(_virtualView as Visual);
        private TopLevel _overLayerTopLevel => TopLevel.GetTopLevel(_overlayContent as Visual);


        protected override IPlatformHandle OnCreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            var context = (parent as AndroidViewControlHandle)?.View.Context ?? global::Android.App.Application.Context;

            _platformView = new LibVLCSharp.Platforms.Android.VideoView(context)
            {
                MediaPlayer = this.MediaPlayer
            };
            _platformView.Click += _platformView_Click;
            _platformView.KeepScreenOn = true;

            if (OverlayContent != null)
            {
                //TODO AvaloniaView can not transparent, but maybe it's not impossible, waitting... maybe?
                //see https://github.com/AvaloniaUI/Avalonia/issues/10807
                _overlayLayer = new AvaloniaView(context);
                _overlayLayer.Content = new Border
                {
                    Child = OverlayContent as Control,
                    Background = Brushes.Black
                };

                if (_overLayerTopLevel != null)
                {
                    _overLayerTopLevel.BackRequested += _overLayerTopLevel_BackRequested;
                }


                var container = new RelativeLayout(context);
                container.AddView(_platformView);
                var lp = new RelativeLayout.LayoutParams(WindowManagerLayoutParams.MatchParent, 120);
                lp.AddRule(LayoutRules.AlignParentBottom);
                container.AddView(_overlayLayer, lp);

                return new AndroidViewControlHandle(container);
            }

            return new AndroidViewControlHandle(_platformView);
        }

        private void _overLayerTopLevel_BackRequested(object sender, Interactivity.RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void _platformView_Click(object sender, EventArgs e)
        {
            PlatformClick?.Invoke(this, e);
            if (_overlayLayer == null)
            {
                return;
            }

            if (_overlayLayer.Visibility == ViewStates.Visible)
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
            if (_overLayerTopLevel != null)
            {
                _overLayerTopLevel.BackRequested -= _overLayerTopLevel_BackRequested;

                var type = _overLayerTopLevel.PlatformImpl.GetType();
                var closedPro = type.GetProperty("Closed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var closed = (Action)closedPro?.GetValue(_overLayerTopLevel.PlatformImpl);
                closed?.Invoke();
                _overLayerTopLevel.PlatformImpl.Dispose();
            }

            if (_overlayLayer != null)
            {
                _overlayLayer.Dispose();
                _overlayLayer = null;
            }
        }

        private void SetFullScreen()
        {       
            if (_topLevel == null) return;

            _topLevel.InsetsManager.IsSystemBarVisible = false;
            _topLevel.InsetsManager.DisplayEdgeToEdge = true;

            if (_overlayLayer != null && _overlayContent != null && _overLayerTopLevel != null)
            {
                _overLayerTopLevel.InsetsManager.IsSystemBarVisible = false;
                _overLayerTopLevel.InsetsManager.DisplayEdgeToEdge = true;
            }
        }

        private void ExitFullScreen()
        {
            if (_topLevel == null) return;

            _topLevel.InsetsManager.IsSystemBarVisible = true;
            _topLevel.InsetsManager.DisplayEdgeToEdge = false;

            if (_overlayLayer != null && _overlayContent != null && _overLayerTopLevel != null)
            {
                _overLayerTopLevel.InsetsManager.IsSystemBarVisible = true;
                _overLayerTopLevel.InsetsManager.DisplayEdgeToEdge = false;
            }
        }

    }
}