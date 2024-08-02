using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using Avalonia.Metadata;
using PolarShadow.Core;
using PolarShadow.Essentials;
using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public class PSPlayer : Decorator
    {

        private VideoView _videoView;
        private VideoView VideoView
        {
            get => _videoView; 
            set => _videoView = value;
        }

        public static readonly StyledProperty<IVideoViewController> ControllerProperty = AvaloniaProperty.Register<PSPlayer, IVideoViewController>(nameof(Controller));
        public IVideoViewController Controller
        {
            get => GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        static PSPlayer()
        {
            ControllerProperty.Changed.AddClassHandler<PSPlayer>((s, e) => s.ControllerPropertyChanged(e));
        }

        private void ControllerPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            EnsureCreateVideoView();
            VideoView.Controller = e.GetNewValue<IVideoViewController>();
        }

        [Content]
        public new Control Child
        {
            get => base.Child;
            set
            {
                EnsureCreateVideoView();
                if (OperatingSystem.IsWindows())
                {
                    VideoView.Content = value;
                    base.Child = VideoView;
                }
                else if (OperatingSystem.IsAndroid())
                {
                    if (value is ContentControl content)
                    {
                        content.Content = VideoView;
                    }
                    base.Child = value;
                }
            }
        }

        private void EnsureCreateVideoView()
        {
            if (VideoView == null)
            {
                VideoView = new VideoView();
            }
        }
    }
}
