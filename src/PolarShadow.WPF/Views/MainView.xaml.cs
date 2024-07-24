using PolarShadow.Models;
using PolarShadow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolarShadow.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            this.Loaded += MainView_Loaded;
        }

        public MainView(MainViewModel vm) : this()
        {
            this.DataContext = vm;
        }

        public MainViewModel VM => DataContext as MainViewModel;

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            if (VM.MenuItems == null)
            {
                VM.MenuItems = new ObservableCollection<MenuIconItem>
                {
                    new() { Name = "main", Icon = this.FindResource<string>("home"), VMType = typeof(BookshelfViewModel) },
                    new() { Name = "discover", Icon = this.FindResource<string>("discover"), VMType = typeof(DiscoverViewModel) },
                    new() { Name = "source", Icon = this.FindResource<string>("source"), VMType = typeof(BookSourceViewModel) },
                    new() { Name = "user", Icon = this.FindResource<string>("user"), VMType = typeof(MineViewModel) },
                    //#if DEBUG
                    //        new(){ Name = "test", Icon=FindResource<string>("flask"), VMType = typeof(VideoPlayerViewModel)}
                    //#endif
                };

                VM.SelectedValue = VM.MenuItems.FirstOrDefault();
            }
            
        }
    }
}
