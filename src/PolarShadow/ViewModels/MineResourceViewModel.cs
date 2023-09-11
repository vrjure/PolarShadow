using PolarShadow.Cache;
using PolarShadow.Models;
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

        private ObservableCollection<ResourceViewData> _mineResource;
        public ObservableCollection<ResourceViewData> MineResource
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
