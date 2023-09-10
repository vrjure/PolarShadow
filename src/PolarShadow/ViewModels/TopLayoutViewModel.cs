using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using PolarShadow.Core;
using PolarShadow.Models;
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
    public class TopLayoutViewModel : ViewModelBase, IRecipient<LoadingState>
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

            WeakReferenceMessenger.Default.Register(this);
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public override async void OnLoad()
        {
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

                await Task.Run(() =>
                {
                    using var db = _dbFactory.CreateDbContext();
                    db.Database.EnsureCreated();

                    var dbSource = new DbConfigurationSource()
                    {
                        DbCreater = () => _dbFactory.CreateDbContext()
                    };

                    _polar.Load(dbSource, true);
                });

            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

            IsLoading = false;
            _nav.Navigate<MainView>(NavigationName);

        }

        void IRecipient<LoadingState>.Receive(LoadingState message)
        {
            IsLoading = message.IsLoading;
        }
    }
}
