using Avalonia.Controls.Notifications;
using Avalonia.Controls.Selection;
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
        private readonly INavigationService _nav;

        private PageFilter _filter;
        private TaskCompletionSource _loading;
        public DiscoverDetailViewModel(INotificationManager notify, IPolarShadow polar, INavigationService nav)
        {
            _notify = notify;
            _polar = polar;
            _nav = nav;
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

        private ResourceTree _selectedResource;
        public ResourceTree SelectedResource
        {
            get => _selectedResource;
            set
            {
                if (SetProperty(ref _selectedResource, value))
                {
                    ResourceSelected();
                }
            }
        }

        private ISelectionModel _selection;
        public ISelectionModel Selection
        {
            get => _selection;
            set => SetProperty(ref _selection, value);
        }

        private bool _showLoadMore = false;
        public bool ShowLoadMore
        {
            get => _showLoadMore;
            set => SetProperty(ref _showLoadMore, value);
        }

        private IAsyncRelayCommand _loadMoreCommand;
        public IAsyncRelayCommand LoadMoreCommand => _loadMoreCommand ??= new AsyncRelayCommand(LoadMore);

        protected override void IsLoadingChanged()
        {
            if(_loading != null && !IsLoading)
            {
                _loading.TrySetResult();
            }
        }

        private async void CategoryValueChanged()
        {
            if (CategoryValue == null)
            {
                return;
            }
            if (IsLoading)
            {
                _loading = new TaskCompletionSource();
                Cancellation.Cancel();
                await _loading.Task;
                _loading = null;
            }
            IsLoading = true;
            ShowLoadMore = false;
            try
            {
                ResourceList?.Clear();
                _filter ??= new PageFilter();
                _filter.Page = 1;
                _filter.PageSize = 10;
                var resources = await Param_Site.ExecuteAsync<ICollection<ResourceTree>>(_polar, CategoryValue.Request, builder =>
                {
                    builder.AddObjectValue(CategoryValue);
                    builder.AddObjectValue(_filter);
                }, Cancellation.Token);
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
            catch (OperationCanceledException) { }
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
                var resources = await Param_Site.ExecuteAsync<ICollection<ResourceTree>>(_polar, CategoryValue.Request, builder =>
                {
                    builder.AddObjectValue(CategoryValue);
                    builder.AddObjectValue(_filter);
                }, Cancellation.Token);
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
            catch (OperationCanceledException) { }
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
                var categories = await Param_Site.ExecuteAsync<ICollection<Resource>>(_polar, Requests.Categories, Cancellation.Token);
                IsLoading = false;
                if (categories == null)
                {
                    Categories?.Clear();
                    HasData = false;
                    return;
                }

                Categories = new ObservableCollection<Resource>(categories);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }

        private void ResourceSelected()
        {
            if (SelectedResource == null)
            {
                return;
            }

            _nav.Navigate<DetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                { nameof(DetailViewModel.Param_Link), SelectedResource }
            }, true);

            SelectedResource = null;
            Selection?.Clear();
        }
    }
}
