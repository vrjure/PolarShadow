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
        private static readonly string _layout_horizontal = "layout_horizontal";
        private static readonly string _layout_vertical = "layout_vertical";
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

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);

            if (e.NewSize.Width < 640)
            {
                part_root.Classes.Remove(_layout_horizontal);
                if (!part_root.Classes.Contains(_layout_vertical))
                {
                    part_root.Classes.Add(_layout_vertical);

                    part_root.Children.Remove(part_desc);
                    if (part_desc.Parent == null)
                    {
                        part_bottom.Children.Insert(0, part_desc);
                    }
                }
            }
            else
            {
                part_root.Classes.Remove(_layout_vertical);
                if (!part_root.Classes.Contains(_layout_horizontal))
                {
                    part_root.Classes.Add(_layout_horizontal);

                    part_bottom.Children.Remove(part_desc);
                    if (part_desc.Parent == null)
                    {
                        part_root.Children.Add(part_desc);
                    }
                }
            }
        }
    }
}
