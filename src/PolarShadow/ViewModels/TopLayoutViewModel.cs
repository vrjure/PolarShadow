using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class TopLayoutViewModel : ViewModelBase, IRecipient<LoadingState>
    {
        public static string NavigationName = "TopLayoutContent";
        public static string RightTitleBarContainer = "RightTitleBarContianer";
        public static string CenterTitleBarContainer = "CenterTItleBarContainer";
        public static string Overlayer = "Overlayer";

        private readonly INavigationService _nav;
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        private readonly INotificationManager _notify;
        private readonly IPolarShadow _polar;
        private readonly ITopLevelService _topLevel;
        public TopLayoutViewModel(INavigationService nav, IDbContextFactory<PolarShadowDbContext> dbFactory, INotificationManager notify, IPolarShadow polar, ITopLevelService toplevel)
        {
            _nav = nav;
            _dbFactory = dbFactory;
            _notify = notify;
            _polar = polar;
            _topLevel = toplevel;
        }

        private bool _ShowTitleBar = true;
        public bool ShowTitleBar
        {
            get => _ShowTitleBar;
            set => SetProperty(ref _ShowTitleBar, value);
        }

        private bool _isDesktop = true;
        public bool IsDesktop
        {
            get => _isDesktop;
            set => SetProperty(ref _isDesktop, value);
        }

        protected override async void OnLoad()
        {
            _topLevel.GetTopLevel().BackRequested += App_BackRequested;
            _topLevel.PropertyChanged += TopLevel_PropertyChanged;
            IsDesktop = OperatingSystem.IsWindows();

            IsLoading = true;
            try
            {
                InitLibVlc();

                await Task.Run(() =>
                {
                    using var db = _dbFactory.CreateDbContext();
                    db.Database.EnsureCreated();

                    _polar.Load();
                });
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

            IsLoading = false;
            _nav.Navigate<MainViewModel>(NavigationName);

        }

        private void InitLibVlc()
        {
            Task.Run(() => Ioc.Default.GetService<LibVLC>());
        }

        protected override void OnUnload()
        {
            if (_topLevel.GetTopLevel() != null)
            {
                _topLevel.GetTopLevel().BackRequested -= App_BackRequested;
            }
            _topLevel.PropertyChanged -= TopLevel_PropertyChanged;
        }

        void IRecipient<LoadingState>.Receive(LoadingState message)
        {
            IsLoading = message.IsLoading;
        }

        private void App_BackRequested(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {           
            if (_nav.CanBack(NavigationName))
            {
                if (_topLevel.FullScreen)
                {
                    _topLevel.FullScreen = false;
                }
                else
                {
                    _nav.Back(NavigationName);
                }
                e.Handled = true;
            }
        }

        private void TopLevel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ITopLevelService.FullScreen)))
            {
                ShowTitleBar = !_topLevel.FullScreen;
            }
        }
    }
}
