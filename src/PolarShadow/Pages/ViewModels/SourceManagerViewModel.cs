using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PolarShadow.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadow.Pages.ViewModels
{
    public partial class SourceManagerViewModel : ObservableObject
    {
        private readonly IPolarShadowBuilder _builder;
        private readonly ILogger _logger;
        public SourceManagerViewModel(IPolarShadowBuilder builder, ILogger<SourceManagerViewModel> logger)
        {
            _builder = builder;
            _logger = logger;
            if (builder.Option.Sites == null)
            {
                Sites = new();
            }
            else
            {
                Sites = new ObservableCollection<SiteOption>(builder.Option.Sites);
            }
        }

        public ObservableCollection<SiteOption> Sites { get; set; }

        [RelayCommand]
        public async Task ImportClick()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions()
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[]{".json"} },
                        { DevicePlatform.Android, new[]{"application/json"} }
                    })
                });

                using var stream = await result.OpenReadAsync();
                var newOption = JsonSerializer.Deserialize<PolarShadowOption>(stream, JsonOption.DefaultSerializer);
                MauiProgram.SavePolarShadowOption(newOption);
                Sites.Clear();
                _builder.Option.Sites.Clear();
                foreach (var item in newOption.Sites)
                {
                    Sites.Add(item);
                    _builder.Option.Sites.Add(item);
                }
                _builder.Option.IsChanged = true;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "import error");
            }
        }
    }
}
