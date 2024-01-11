using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class PSPlayer : TemplatedControl
    {
        static PSPlayer()
        {
            ControllerProperty.Changed.AddClassHandler<PSPlayer>((s, e) => s.ControllerChanged(e));
        }

        public static readonly StyledProperty<IVideoViewController> ControllerProperty = AvaloniaProperty.Register<PSPlayer, IVideoViewController>(nameof(Controller));
        public IVideoViewController Controller
        {
            get => GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<MediaPlayerController, string>(nameof(Title));
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<bool> FullScreenProperty = AvaloniaProperty.Register<MediaPlayerController, bool>(nameof(FullScreen));
        public bool FullScreen
        {
            get => GetValue(FullScreenProperty);
            set => SetValue(FullScreenProperty, value);
        }

        private void ControllerChanged(AvaloniaPropertyChangedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("PSPlayer: controller changed");
        }
    }
}
