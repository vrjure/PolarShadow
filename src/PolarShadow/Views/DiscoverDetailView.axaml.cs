using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using PolarShadow.Controls;
using PolarShadow.ViewModels;
using System;

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
