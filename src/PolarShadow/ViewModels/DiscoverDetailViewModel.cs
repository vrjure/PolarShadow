using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Navigations;
using PolarShadow.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class DiscoverDetailViewModel : ViewModelBase, IParameterObtain
    {
        private readonly INotificationManager _notify;
        private readonly IPolarShadow _polar;

        private PageFilter _filter;

        public DiscoverDetailViewModel(INotificationManager notify, IPolarShadow polar)
        {
            _notify = notify;
            _polar = polar;
        }

        public ISite Param_Site { get; set; }

        private ObservableCollection<Resource> _categories;
        public ObservableCollection<Resource> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<ResourceTree> _resourceList;
        public ObservableCollection<ResourceTree> ResourceList
        {
            get => _resourceList;
            set => SetProperty(ref _resourceList, value);
        }

        private Resource _categoryValue;
        public Resource CategoryValue
        {
            get => _categoryValue;
            set
            {
                if (SetProperty(ref _categoryValue, value))
                {
                    CategoryValueChanged();
                }
            }
        }

        private bool _showLoadMore = false;
        public bool ShowLoadMore
        {
            get => _showLoadMore;
            set => SetProperty(ref _showLoadMore, value);
        }

        private IAsyncRelayCommand _loadMoreCommand;
        public IAsyncRelayCommand LoadMoreCommand => _loadMoreCommand ??= new AsyncRelayCommand(LoadMore);

        private async void CategoryValueChanged()
        {
            IsLoading = true;
            ShowLoadMore = false;
            try
            {
                ResourceList?.Clear();
                _filter ??= new PageFilter();
                _filter.Page = 1;
                _filter.PageSize = 10;
                var resources = await Param_Site.ExecuteAsync<ICollection<ResourceTree>>(CategoryValue.Request, builder =>
                {
                    builder.AddObjectValue(CategoryValue);
                    builder.AddObjectValue(_filter);
                });
                if (resources == null || resources.Count == 0)
                {
                    HasData = false;
                    return;
                }

                HasData = true;

                if (ResourceList == null)
                {
                    ResourceList = new ObservableCollection<ResourceTree>(resources);
                }
                else
                {
                    foreach (var item in resources)
                    {
                        ResourceList.Add(item);
                    }
                }

                ShowLoadMore = true;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadMore()
        {
            IsLoading = true;
            ShowLoadMore = false;
            try
            {
                _filter.Page++;
                var resources = await Param_Site.ExecuteAsync<ICollection<ResourceTree>>(CategoryValue.Request, builder =>
                {
                    builder.AddObjectValue(CategoryValue);
                    builder.AddObjectValue(_filter);
                });
                if (resources == null || resources.Count == 0)
                {
                    ShowLoadMore = false;
                    return;
                }

                foreach (var item in resources)
                {
                    ResourceList.Add(item);
                }
                ShowLoadMore = true;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(nameof(Param_Site), out ISite site))
            {
                Param_Site = site;
            }
        }

        protected override async void OnLoad()
        {
            if (Param_Site == null)
            {
                _notify.Show("Miss Parameter");
                return;
            }

            try
            {
                IsLoading = true;
                var categories = await Param_Site.ExecuteAsync<ICollection<Resource>>(Requests.Categories);
                IsLoading = false;
                if (categories == null)
                {
                    Categories?.Clear();
                    HasData = false;
                    return;
                }

                Categories = new ObservableCollection<Resource>(categories);
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }
    }
}
