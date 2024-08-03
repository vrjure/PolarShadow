using PolarShadow.Essentials;
using PolarShadows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PolarShadow.Controls
{
    [TemplatePart(Name = "PART_VideoView", Type = typeof(Decorator))]
    internal class PSPlayer : Decorator
    {
        public static DependencyProperty ControllerProperty = DP.Register<PSPlayer, IVideoViewController>(nameof(Controller), PropertyChanged);
        public IVideoViewController Controller
        {
            get => (IVideoViewController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public PSPlayer()
        {
            this.Unloaded += PSPlayer_Unloaded;
        }

        private void PSPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as PSPlayer;
            if (e.Property == ControllerProperty)
            {
                element.SwitchPlayer(e.NewValue as IVideoViewController);
            }
        }

        private ContentControl _videoView;
        private ContentControl VideoView
        {
            get => _videoView;
            set
            {
                _videoView = value;
                var child = Child;
                if(_videoView != null)
                {
                    Child = null;
                    if (IsPlayer(child))
                    {
                        _videoView.Content = (child as ContentControl)?.Content;
                    }
                    else
                    {
                        _videoView.Content = child;
                    }
                    Child = _videoView;
                }
            }
        }

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (VideoView == null || value == VideoView)
                {
                    base.Child = value;
                }
                else
                {
                    VideoView.Content = value;
                    base.Child = VideoView;
                }
            }
        }


        private void SwitchPlayer(IVideoViewController controller)
        {
            if (controller == null)
            {
                if (VideoView is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                return;
            }

            if (controller is IVLController vlc)
            {
                InitializeVLC(vlc);
            }
        }

        private void InitializeVLC(IVLController controller)
        {
            var videoView = new VideoView();
            videoView.MediaPlayer = controller.MediaPlayer;
            VideoView = videoView;
        }

        private bool IsPlayer(UIElement element)
        {
            return element is VideoView;
        }

        private void Dispose()
        {
            if (VideoView is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Controller?.Dispose();
        }
    }
}
