using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

        public DetailViewModel VM => (DetailViewModel)this.DataContext;

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(DetailViewModel.WebAnalysisSites)))
            {
                return;
            }

            if (VM.WebAnalysisSites == null || VM.WebAnalysisSites.Count == 0)
            {
                return;
            }

            FlyoutBase.ShowAttachedFlyout(part_container);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            VM.PropertyChanged += Vm_PropertyChanged;
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            VM.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
