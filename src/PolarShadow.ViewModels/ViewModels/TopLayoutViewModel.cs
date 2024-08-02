using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using LibVLCSharp.Shared;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Essentials;
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
    public class TopLayoutViewModel : ViewModelBase
    {
        public static string NavigationName = "TopLayoutContent";
        public static string RightTitleBarContainer = "RightTitleBarContianer";
        public static string CenterTitleBarContainer = "CenterTItleBarContainer";
        public static string Overlayer = "Overlayer";

        private readonly INavigationService _nav;
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        private readonly IMessageService _notify;
        private readonly IPolarShadow _polar;
        public TopLayoutViewModel(INavigationService nav, IDbContextFactory<PolarShadowDbContext> dbFactory, IMessageService notify, IPolarShadow polar)
        {
            _nav = nav;
            _dbFactory = dbFactory;
            _notify = notify;
            _polar = polar;
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
  
        }
    }
}
