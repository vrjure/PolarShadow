using Avalonia.Controls;
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
    }
}
