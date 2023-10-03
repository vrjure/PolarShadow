using Avalonia.Controls;
using PolarShadow.ViewModels;

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
    }
}
