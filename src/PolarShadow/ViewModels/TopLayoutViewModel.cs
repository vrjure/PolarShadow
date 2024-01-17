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
    public class TopLayoutViewModel : ViewModelBase, IRecipient<LoadingState>, IRecipient<FullScreenState>
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
            IsDesktop = OperatingSystem.IsWindows();

            IsLoading = true;
            try
            {
                // async 方法貌似会阻塞线程，动画不会播放？？
                //using var db = _dbFactory.CreateDbContext();
                //await db.Database.EnsureCreatedAsync();

                //var dbSource = new DbConfigurationSource()
                //{
                //    DbCreater = () => _dbFactory.CreateDbContext()
                //};
                //await _polar.LoadAsync(dbSource, true);

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
            _topLevel.GetTopLevel().BackRequested -= App_BackRequested;
        }

        void IRecipient<LoadingState>.Receive(LoadingState message)
        {
            IsLoading = message.IsLoading;
        }

        void IRecipient<FullScreenState>.Receive(FullScreenState message)
        {
            ShowTitleBar = !message.IsFullScreen;
            if (message.IsFullScreen)
            {
                _topLevel.FullScreen();
            }
            else
            {
                _topLevel.ExitFullScreen();
            }
        }

        private void App_BackRequested(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {           
            if (_nav.CanBack(NavigationName))
            {
                _nav.Back(NavigationName);
                e.Handled = true;
            }
        }
    }
}
