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
        private readonly IPolarShadowBuilder _builder;
        private string _importPath;
        public MainWindowViewModel(IPolarShadowBuilder builder)
        {
            _builder = builder;
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

            _importPath = fileDialog.FileName;
            using var fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read);
            var config = JsonSerializer.Deserialize<PolarShadowOption>(fs, JsonOption.DefaultSerializer);
            _builder.Option.AnalysisSources = config.AnalysisSources;
            _builder.Option.Sites = config.Sites;
            _builder.Option.IsChanged = true;

            UI.Navigate<MainPage>("content");

            RefreshEnable = true;
            
        }

        [RelayCommand]
        public void Refresh()
        {
            using var fs = new FileStream(_importPath, FileMode.Open, FileAccess.Read);
            var config = JsonSerializer.Deserialize<PolarShadowOption>(fs, JsonOption.DefaultSerializer);
            _builder.Option.AnalysisSources = config.AnalysisSources;
            _builder.Option.Sites = config.Sites;
            _builder.Option.IsChanged = true;
            UI.Refresh("content");
        }
    }
}
