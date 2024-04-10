using PolarShadow.Notification;
using PolarShadow.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for TopLayoutView.xaml
    /// </summary>
    public partial class TopLayoutView : UserControl
    {
        private readonly IMessageService _msgService;
        public TopLayoutView()
        {
            InitializeComponent();
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
        }

        public TopLayoutView(TopLayoutViewModel vm, IMessageService messageService) : this()
        {
            DataContext = vm;
            (messageService as NotificationContainer)?.Initialize(Root);
            _msgService = messageService;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow.WindowState == WindowState.Normal)
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
            else if (mainWindow.WindowState == WindowState.Maximized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            if(mainWindow.WindowState == WindowState.Maximized)
            {
                BtnMaximize.Content = this.FindResource<string>("maximize-restore");
            }
            else if (mainWindow.WindowState == WindowState.Normal)
            {
                BtnMaximize.Content = this.FindResource<string>("maximize");
            }
        }
    }
}
