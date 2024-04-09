using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using PolarShadow.Navigations;
using PolarShadow.ViewModels;
using PolarShadow.WPF.Views;

namespace PolarShadow.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INavigationService _nav;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        public MainWindow(MainWindowViewModel vm, INavigationService nav) : this()
        {
            this.DataContext = vm;
            _nav = nav;
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _nav.Navigate<TopLayoutViewModel>(MainWindowViewModel.NavigationName);       
        }
    }
}