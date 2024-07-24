using PolarShadow.Controls;
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
    /// Interaction logic for DetailView.xaml
    /// </summary>
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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!PlayerController.FullScreen)
            {
                globalScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                border_player.MaxHeight = 350;

                if (sizeInfo.WidthChanged)
                {
                    if (sizeInfo.NewSize.Width <= 1000)
                    {
                        FlexPanel.SetFloat(border_info, false);
                        FlexPanel.SetFlexBasis(border_player, new FlexBasis("100%"));
                    }
                    else
                    {
                        FlexPanel.SetFlexBasis(border_player, new FlexBasis("40%"));
                        FlexPanel.SetFloat(border_info, true);
                    }
                }

                if (sizeInfo.HeightChanged)
                {
                    border_player.Height = sizeInfo.NewSize.Height;
                }
            }
            else
            {
                FlexPanel.SetFlexBasis(border_player, new FlexBasis("100%"));
                border_player.Height = border_player.MaxHeight = sizeInfo.NewSize.Height;
                globalScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }
    }
}
