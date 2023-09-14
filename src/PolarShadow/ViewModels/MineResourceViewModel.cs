using PolarShadow.Cache;
using PolarShadow.Models;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class MineResourceViewModel:ViewModelBase
    {
        public MineResourceViewModel(IBufferCache cache)
        {
            Cache = cache;
        }

        public IBufferCache Cache { get; }

        private ObservableCollection<ResourceModel> _mineResource;
        public ObservableCollection<ResourceModel> MineResource
        {
            get => _mineResource;
            set => SetProperty(ref _mineResource, value);
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }
    }
}
