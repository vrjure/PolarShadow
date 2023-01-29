using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PolarShadow.Core;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Pages.ViewModels
{
    public partial class MyCollectionViewModel : ObservableObject
    {
        private readonly IMyCollectionService _myCollectionService;

        public MyCollectionViewModel(IMyCollectionService myCollectionService)
        {
            _myCollectionService = myCollectionService;
        }

        public ObservableCollection<VideoSummary> MyCollections { get; } = new();

        [RelayCommand]
        public async Task ItemSelected(VideoSummary videoSummary)
        {
            if (videoSummary == null)
            {
                return;
            }
            await Shell.Current.GoToAsync(nameof(VideoDetailPage), new Dictionary<string, object>
            {
                {VideoDetailViewModel.Key_VideoSummary, videoSummary }
            });
        }

        public async Task InitializeAsync()
        {
            var data = await _myCollectionService.GetMyCollectionAsync(1, 100);
            MyCollections.Clear();
            foreach (var item in data)
            {
                MyCollections.Add(item);
            }
        }
    }
}
