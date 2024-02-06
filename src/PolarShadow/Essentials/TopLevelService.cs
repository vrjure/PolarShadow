using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal class TopLevelService : ObservableObject, ITopLevelService
    {
        private TopLevel _topLevel;
        private Func<Visual> _visualFactory;

        public TopLevel GetTopLevel()
        {
            var topLevel = _topLevel;
            if (topLevel == null)
                topLevel = TopLevel.GetTopLevel(_visualFactory?.Invoke());

            return topLevel;
        }

        public void SetTopLevel(TopLevel topLevel)
        {
            _topLevel = topLevel;
        }

        public void SetTopLevelFactory(Func<Visual> factory)
        {
            _visualFactory = factory;
        }

        public static TopLevel TopLevel => Ioc.Default.GetService<ITopLevelService>()?.GetTopLevel();

        private bool _fullScreen = false;
        public bool FullScreen
        {
            get => _fullScreen;
            set
            {
                if (SetProperty(ref _fullScreen, value))
                {
                    if (OperatingSystem.IsWindows())
                    {
                        if (value)
                        {
                            (TopLevel as Window).WindowState = WindowState.FullScreen;
                        }
                        else
                        {
                            (TopLevel as Window).WindowState = WindowState.Normal;
                        }
                    }
                    else if (OperatingSystem.IsAndroid())
                    {
                        if (value)
                        {
                            TopLevel.InsetsManager.DisplayEdgeToEdge = true;
                            TopLevel.InsetsManager.IsSystemBarVisible = false;
                        }
                        else
                        {
                            TopLevel.InsetsManager.DisplayEdgeToEdge = false;
                            TopLevel.InsetsManager.IsSystemBarVisible = true;
                        }
                    }
                }
               
            }
        }

        private static bool GetFullScreenState()
        {
            if (OperatingSystem.IsWindows())
            {
                return (TopLevel as Window).WindowState == WindowState.FullScreen;
            }
            else if (OperatingSystem.IsAndroid())
            {
                return TopLevel.InsetsManager.DisplayEdgeToEdge == true;
            }
            return false;
        }
    }
}
