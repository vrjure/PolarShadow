using Avalonia.Controls.Notifications;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Storage;
using PolarShadow.Views;
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

        private readonly INavigationService _nav;
        private readonly IDbContextFactory<PolarShadowDbContext> _dbFactory;
        private readonly INotificationManager _notify;
        private readonly IPolarShadow _polar;
        public TopLayoutViewModel(INavigationService nav, IDbContextFactory<PolarShadowDbContext> dbFactory, INotificationManager notify, IPolarShadow polar)
        {
            _nav = nav;
            _dbFactory = dbFactory;
            _notify = notify;
            _polar = polar;
        }

        private bool _isInitialized = false;
        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        public override async void OnLoad()
        {
            IsInitialized = false;
            try
            {
                using var db = _dbFactory.CreateDbContext();
                await db.Database.EnsureCreatedAsync();

                var dbSource = new DbConfigurationSource()
                {
                    DbCreater = () => _dbFactory.CreateDbContext()
                };

                await _polar.LoadAsync(dbSource, true);

                _nav.Navigate<MainView>(NavigationName);
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

            IsInitialized = true;
        }
    }
}
