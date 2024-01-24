using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using PolarShadow.Core;
using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public class PSPlayer : TemplatedControl
    {

        private MediaPlayerController _mediaController;
        private MediaPlayerController Part_MediaController
        {
            get => _mediaController;
            set => _mediaController = value;
        }

        private VideoView _videoView;
        private VideoView Part_VideoView
        {
            get => _videoView; 
            set => _videoView = value;
        }

        public static readonly StyledProperty<IMediaController> MediaControllerProperty = AvaloniaProperty.Register<PSPlayer, IMediaController>(nameof(MediaController));
        public IMediaController MediaController
        {
            get => GetValue(MediaControllerProperty);
            set => SetValue(MediaControllerProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Part_MediaController = e.NameScope.Find<MediaPlayerController>("Part_MediaController");
            Part_VideoView = e.NameScope.Find<VideoView>("Part_VideoView");

            Part_MediaController.MediaController ??= new MediaController() { Controller = Part_VideoView.Controller };
            MediaController = Part_MediaController.MediaController;
        }
    }
}
