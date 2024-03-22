using CommunityToolkit.Mvvm.ComponentModel;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Models
{
    public class RefreshItem<T> : ObservableObject
    {
        public RefreshItem() { }
        public RefreshItem(T data) : this(false, false, data)
        {

        }
        public RefreshItem(bool isNew, bool isRefresh, T data)
        {
            IsNew = isNew;
            IsRefresh = isRefresh;
            Data = data;
        }

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }
        private bool _isRefresh;
        public bool IsRefresh
        {
            get => _isRefresh;
            set => SetProperty(ref _isRefresh, value);
        }

        private T _data;
        public T Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }
    }

    public class ResourceModelRefreshItem : RefreshItem<ResourceModel>
    {
        public ResourceModelRefreshItem() { }
        public ResourceModelRefreshItem(ResourceModel data) : base(data)
        {

        }
        public ResourceModelRefreshItem(bool isNew, bool isRefresh, ResourceModel data) : base(isNew, isRefresh, data)
        {

        }
    }
}
