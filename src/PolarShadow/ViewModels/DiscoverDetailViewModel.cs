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

        private ObservableCollection<ResourceTree> _categories;
        public ObservableCollection<ResourceTree> Categories
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

        private ISelectionModel _categorySelection;
        public ISelectionModel CategorySelection
        {
            get => _categorySelection;
            set 
            { 
                if(SetProperty(ref _categorySelection, value))
                {
                    _categorySelection.SelectionChanged += CategorySelection_SelectionChanged;
                }
            }
        }

        private ISelectionModel _categoryChildrenSelection;
        public ISelectionModel CategoryChildrenSelection
        {
            get => _categoryChildrenSelection;
            set
            {
                if (SetProperty(ref _categoryChildrenSelection, value))
                {
                    _categoryChildrenSelection.SelectionChanged += CategorySelection_SelectionChanged;
                }
            }
        }

        private ResourceTree _currentCategory;


        private bool _showLoadMore = false;
        public bool ShowLoadMore
        {
            get => _showLoadMore;
            set => SetProperty(ref _showLoadMore, value);
        }

        private IAsyncRelayCommand _loadMoreCommand;
        public IAsyncRelayCommand LoadMoreCommand => _loadMoreCommand ??= new AsyncRelayCommand(LoadMore);

        protected override async void OnLoad()
        {
            if (Param_Site == null)
            {
                _notify.Show("Miss Parameter");
                return;
            }

            if (Categories != null)
            {
                return;
            }

            try
            {
                IsLoading = true;
                var categories = await Param_Site.ExecuteAsync<ICollection<ResourceTree>>(_polar, Requests.Categories, Cancellation.Token);
                IsLoading = false;
                if (categories == null)
                {
                    Categories?.Clear();
                    HasData = false;
                    return;
                }

                Categories = new ObservableCollection<ResourceTree>(categories);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
        }

        protected override void IsLoadingChanged()
        {
            if(_loading != null && !IsLoading)
            {
                _loading.TrySetResult();
            }
        }
        private void CategorySelection_SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                CategoryValueChanged(e.SelectedItems[0] as ResourceTree);

                if (sender == CategorySelection && _currentCategory != null && CategoryChildrenSelection != null)
                {
                    CategoryChildrenSelection.SelectedItem = _currentCategory;
                }
            }
        }


        private async void CategoryValueChanged(ResourceTree category)
        {
            if (category == null || string.IsNullOrEmpty(category.Src) || _currentCategory == category)
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
                var resources = await Param_Site.ExecuteAsync<ICollection<ResourceTree>>(_polar, category.Request, builder =>
                {
                    builder.AddObjectValue(category);
                    builder.AddObjectValue(_filter);
                }, Cancellation.Token);

                _currentCategory = category;

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
                if (_currentCategory == null)
                {
                    return;
                }
                var categoryValue = _currentCategory;

                var handler = Param_Site.CreateRequestHandler(_polar, categoryValue.Request);

                var canPage = true;
                if(handler.TryGetParameter(SearchParams.CanPage, out bool val))
                {
                    canPage = val;
                }

                if (!canPage) 
                {
                    ShowLoadMore = false;
                    return;
                }

                _filter.Page++;
                var resources = await handler.ExecuteAsync<ICollection<ResourceTree>>(builder =>
                {
                    builder.AddObjectValue(categoryValue);
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

        private void ResourceSelected(ResourceTree res)
        {
            if (res == null)
            {
                return;
            }

            _nav.Navigate<DetailViewModel>(TopLayoutViewModel.NavigationName, new Dictionary<string, object>
            {
                { nameof(DetailViewModel.Param_Link), res }
            }, true);

        }

        protected override void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
        {
            if (e.SelectedItems.Count > 0)
            {
                ResourceSelected(e.SelectedItems[0] as ResourceTree);
                SelectionModel.Deselect(e.SelectedIndexes[0]);
            }
        }
    }
}
