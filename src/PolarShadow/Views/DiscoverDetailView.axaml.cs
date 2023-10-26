using Avalonia.Controls;
using Avalonia.Input;
using PolarShadow.ViewModels;

namespace PolarShadow.Views
{
    public partial class DiscoverDetailView : UserControl
    {
        public DiscoverDetailView()
        {
            InitializeComponent();
        }

        public DiscoverDetailView(DiscoverDetailViewModel vm) : this()
        {
            this.DataContext = vm;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
        }
        private void ScrollTapped(object sender, TappedEventArgs e)
        {
        }
    }
}
