using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using PolarShadow.ViewModels;
using System.Linq;

namespace PolarShadow.Views
{
    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
            Episodes.Selection.SelectionChanged += Selection_SelectionChanged;
        }

        public DetailView(DetailViewModel vm) : this()
        {
            this.DataContext = vm;
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
        }

        private void Selection_SelectionChanged(object sender, Avalonia.Controls.Selection.SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                var container = Episodes.ContainerFromIndex(e.SelectedIndexes.First());
                if (container != null)
                {
                    FlyoutBase.ShowAttachedFlyout((container as ListBoxItem).Presenter.Child);
                }
            }
        }

        private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var ctl = sender as Control;

            VM.UpdateWebAnalysisSites();
            if (ctl.Parent != null && VM.WebAnalysisSites?.Count > 0)
            {
                FlyoutBase.ShowAttachedFlyout(ctl.Parent as Control);
            }
        }
    }
}
