using Avalonia.Controls;
using Avalonia.Interactivity;
using PolarShadow.ViewModels;
using System;

namespace PolarShadow.Views
{
    public partial class VideoPlayerView : UserControl
    {
        public VideoPlayerView()
        {
            InitializeComponent();
        }

        public VideoPlayerView(VideoPlayerViewModel vm) : this()
        {
            this.DataContext = vm;
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            if (OperatingSystem.IsAndroid())
            {
                Part_VideoView.PlatformClick += Part_VideoView_PlatformClick;
            }
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);

            if (OperatingSystem.IsAndroid())
            {
                Part_VideoView.PlatformClick -= Part_VideoView_PlatformClick;
            }
        }

        private void Part_VideoView_PlatformClick(object sender, System.EventArgs e)
        {
            if(Part_Controller.Opacity == 1)
            {
                Part_Controller.Opacity = 0;
            }
            else
            {
                Part_Controller.Opacity = 1;
            }
        }
    }
}
