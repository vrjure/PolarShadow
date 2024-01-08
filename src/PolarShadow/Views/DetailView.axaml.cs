using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PolarShadow.Resources;
using PolarShadow.Services;
using PolarShadow.ViewModels;
using System.Linq;

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
            VM.PropertyChanged += VM_PropertyChanged;
        }

        public DetailViewModel VM => (DetailViewModel)this.DataContext;

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            if (Design.IsDesignMode) return;
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            if (Design.IsDesignMode) return;

            VM.PropertyChanged -= VM_PropertyChanged;
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(VM.FlyoutOptions))
            {
                return;
            }

            if (VM?.FlyoutOptions?.Count > 0 && VM.SelectionModel?.SelectedItem != null)
            {
                var container = Episodes.ContainerFromIndex(VM.SelectionModel.SelectedIndex);
                if (container != null)
                {
                    FlyoutBase.ShowAttachedFlyout((container as ListBoxItem).Presenter.Child);
                }
            }
        }
    }
}
