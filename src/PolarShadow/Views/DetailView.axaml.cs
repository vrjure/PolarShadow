using Avalonia.Controls;
using Avalonia.Interactivity;
using LibVLCSharp.Shared;
using PolarShadow.ViewModels;

namespace PolarShadow.Views
{
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
        }

        public DetailView(DetailViewModel vm) : this()
        {
            this.DataContext = vm;
        }
    }
}
