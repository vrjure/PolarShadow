using Avalonia.Controls;
using PolarShadow.ViewModels;

namespace PolarShadow.Views
{
    public partial class DiscoverView : UserControl
    {
        public DiscoverView()
        {
            InitializeComponent();
        }

        public DiscoverView(DiscoverViewModel vm) : this()
        {
            this.DataContext = vm;
        }
    }
}
