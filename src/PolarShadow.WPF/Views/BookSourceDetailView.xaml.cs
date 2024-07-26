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
    /// Interaction logic for BookSourceDetailView.xaml
    /// </summary>
    public partial class BookSourceDetailView : UserControl
    {
        public BookSourceDetailView()
        {
            InitializeComponent();
        }

        public BookSourceDetailView(BookSourceDetailViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}
