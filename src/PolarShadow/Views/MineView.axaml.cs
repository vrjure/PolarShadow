using Avalonia.Controls;
using PolarShadow.ViewModels;

namespace PolarShadow.Views
{
    public partial class MineView : UserControl
    {
        public MineView()
        {
            InitializeComponent();
        }

        public MineView(MineViewModel vm) : this()
        {
            this.DataContext = vm;
        }
    }
}
