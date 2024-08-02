using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Essentials;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Resources;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class BookshelfViewModel : ViewModelBase
    {
        private readonly INavigationService _nav;
        private readonly IMineResourceService _mineResourceService;
        private readonly IMessageService _notify;
        private readonly IPolarShadow _polar;

        private CancellationTokenSource _refreshCts;
        public BookshelfViewModel(INavigationService nav, IMineResourceService mineResourceService, IMessageService notify, IPolarShadow polar)
        {
            _nav = nav;
            _mineResourceService = mineResourceService;
            _notify = notify;
            _polar = polar;
        }

        private ObservableCollection<ResourceModelRefreshItem> _mineResource;
        public ObservableCollection<ResourceModelRefreshItem> MineResource
        {
            get => _mineResource;
            set => SetProperty(ref _mineResource, value);
        }

        private ResourceModelRefreshItem _selectedValue;
        public ResourceModelRefreshItem SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (SetProperty(ref _selectedValue, value))
                {
                    if (_selectedValue != null)
                    {
                        ToDetail(_selectedValue.Data);
                    }
                }
            }
        }

        private IRelayCommand _searchCommand;
        public IRelayCommand SearchCommand => _searchCommand ??= new RelayCommand(() =>
        {
            try
            {
                _nav.Navigate<SearchViewModel>(TopLayoutViewModel.NavigationName, canBack: true);
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        });

        private IRelayCommand _refreshCommand;
        public IRelayCommand RefreshCommand => _refreshCommand ??= new RelayCommand(RefreshAction);

        private IRelayCommand _refreshCancelCommand;
        public IRelayCommand RefreshCancelCommand => _refreshCancelCommand ??= new RelayCommand(RefreshCancel); 

        private bool _refresh;
        public bool Refresh
        {
            get => _refresh;
            set
            {
                if(SetProperty(ref _refresh, value))
                {
                    if (_refresh)
                    {
                        RefreshAction();
                    }
                    else
                    {
                        RefreshCancel();
                    }
                }
            }
        }

        protected override async void OnLoad()
        {
            try
            {
                MineResource?.Clear();
                var savedResource = await _mineResourceService.GetRootResourcesAsync();
                if (savedResource?.Count == 0)
                {
                    return;
                }

                MineResource ??= new ObservableCollection<ResourceModelRefreshItem>();

                foreach (var item in savedResource)
                {
                    MineResource.Add(new ResourceModelRefreshItem(item));
                }
                
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }

        private void ToDetail(ResourceModel res)
        {
            if (res == null)
            {
                return;
            }

            _nav.Navigate<DetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                {nameof(DetailViewModel.Param_Link), res }
            }, true);
        }

        private async void RefreshAction()
        {
            if (MineResource == null || MineResource.Count == 0) return;

            var tasks = new List<Task>();
            _refreshCts = new CancellationTokenSource();
            foreach (var item in MineResource)
            {
                if(_polar.TryGetSite(PolarShadowItems.VideoSites, item.Data.Site, out ISite site))
                {
                    item.IsRefresh = true;
                    var task = site.ExecuteAsync<ResourceTree>(_polar, Requests.Detail, builder => 
                    {
                        builder.AddObjectValue(item.Data);
                    }, _refreshCts.Token).ContinueWith( async (t) =>
                    {
                        item.IsRefresh = false;
                        var result = t.Result;
                        var children = result?.Children;
                        if (children != null)
                        {
                            var total = 0;
                            foreach (var item in children)
                            {
                                total += item.Children?.Count ?? 0;
                            }
                            var count = await _mineResourceService.GetRootChildrenCountAsync(item.Data.Id, 2).ConfigureAwait(false);
                            if (total != count)
                            {
                                item.IsNew = true;

                                var rtn = result.ToResourceTreeNode();
                                rtn.Id = item.Data.Id;
                                await _mineResourceService.SaveResourceAsync(rtn).ConfigureAwait(false);
                            }
                        }
                    });
                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);

            Refresh = false;
        }

        private void RefreshCancel()
        {
            _refreshCts?.Cancel();
            _refreshCts?.Dispose();
            _refreshCts = null;
        }

    }
}
