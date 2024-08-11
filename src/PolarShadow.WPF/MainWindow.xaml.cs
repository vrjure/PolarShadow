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
using PolarShadow.Essentials;
using PolarShadow.Navigations;
using PolarShadow.ViewModels;

namespace PolarShadow.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INavigationService _nav;
        private readonly IFileCache _fileCache;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        public MainWindow(MainWindowViewModel vm, INavigationService nav, IFileCache fileCache = null) : this()
        {
            this.DataContext = vm;
            _nav = nav;
            _fileCache = fileCache;
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _nav.Navigate<TopLayoutViewModel>(MainWindowViewModel.NavigationName);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            _fileCache?.Flush();
        }
    }
}