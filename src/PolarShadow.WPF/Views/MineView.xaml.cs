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
    /// Interaction logic for MineView.xaml
    /// </summary>
    public partial class MineView : UserControl
    {
        public MineView()
        {
            InitializeComponent();
        }

        public MineView(MineViewModel vm) : this()
        {
            DataContext = vm;
            if (vm != null)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private MineViewModel VM => DataContext as MineViewModel;

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.Password))
            {
                passwordBox.Password = VM.Password?.Value;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as MineViewModel;
            if (VM == null)
            {
                return;
            }

            if (VM.Password == null)
            {
                VM.Password = new Models.ChangeValue<string>((sender as PasswordBox).Password);
            }
            else
            {
                VM.Password.Value = (sender as PasswordBox).Password;
            }
        }
    }
}
