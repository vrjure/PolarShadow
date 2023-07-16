using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using PolarShadow.Core;
using PolarShadow.Tool.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace PolarShadow.Tool
{
    public partial class MainWindowViewModel : ObservableObject, IReferenceUI
    {
        private readonly IPolarShadow _polarShadow;
        private string _importPath;
        public MainWindowViewModel(IPolarShadow polarShadow)
        {
            _polarShadow = polarShadow;
            RefreshEnable = false;
        }

        public FrameworkElement UI { get; set; }

        [ObservableProperty]
        private bool refreshEnable;

        [RelayCommand]
        public void ImportConfig()
        {
            var fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == false)
            {
                return;
            }

            try
            {
                _importPath = fileDialog.FileName;
                _polarShadow.LoadJsonFileSource(_importPath, true);

                UI.Navigate<MainPage>("content");

                RefreshEnable = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }

        [RelayCommand]
        public void Refresh()
        {
            try
            {
                _polarShadow.LoadJsonFileSource(_importPath);
                UI.Refresh("content");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
