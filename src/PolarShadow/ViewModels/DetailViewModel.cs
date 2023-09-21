﻿using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Caching.Memory;
using PolarShadow.Aria2;
using PolarShadow.Cache;
using PolarShadow.Core;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Options;
using PolarShadow.Resources;
using PolarShadow.Services;
using PolarShadow.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class DetailViewModel : ViewModelBase, IParameterObtain
    {
        private readonly IPolarShadow _polar;
        private readonly INotificationManager _notify;
        private readonly IMineResourceService _mineResourceService;
        private readonly IBufferCache _bufferCache;
        private readonly IPreference _preference;

        private ResourceModel _rootResourceInDb;
        public DetailViewModel(IPolarShadow polar, INotificationManager notify, IMineResourceService mineResourceService, IBufferCache bufferCache, IPreference preference)
        {
            _polar = polar;
            _notify = notify;
            _mineResourceService = mineResourceService;
            _bufferCache = bufferCache;
            _preference = preference;
        }

        public ILink Param_Link { get; set; }

        public IBufferCache ImageCache => _bufferCache;

        private ResourceTreeNode _resource;
        public ResourceTreeNode Resource
        {
            get => _resource;
            private set => SetProperty(ref _resource, value);
        }

        private bool _isSave;
        public bool IsSaved
        {
            get => _isSave;
            private set => SetProperty(ref _isSave, value);
        }

        private IAsyncRelayCommand _refreshCommand;
        public IAsyncRelayCommand RefreshCommand => _refreshCommand ??= new AsyncRelayCommand(LoadOnline);

        private IAsyncRelayCommand _resourceOperateCommand;
        public IAsyncRelayCommand ResourceOperateCommand => _resourceOperateCommand ??= new AsyncRelayCommand(ResourceOperate);

        private IAsyncRelayCommand _linkClickCommand;
        public IAsyncRelayCommand LinkClickCommand => _linkClickCommand ??= new AsyncRelayCommand<ResourceTreeNode>(LinkClick);

        public void ApplyParameter(IDictionary<string, object> parameters)
        {
            if (parameters.TryGetValue(nameof(Param_Link), out ILink link))
            {
                Param_Link = link;
            }
        }

        protected override async void OnLoad()
        {
            await LoadingDeatil();
        }

        private async Task LoadingDeatil()
        {
            if (Param_Link == null)
            {
                _notify.Show("Missing parameters", NotificationType.Warning);
                return;
            }

            _rootResourceInDb = await _mineResourceService.GetRootResourceAsync(Param_Link.Name);
            if (_rootResourceInDb != null)
            {
                IsSaved = true;
                await LoadLocal(_rootResourceInDb);
            }
            else
            {
                await LoadOnline();
            }            
        }

        private async Task LoadLocal(ResourceModel root)
        {
            var children = await _mineResourceService.GetRootChildrenAsync(root.Id);
            if (children.Count == 0)
            {
                return;
            }

            var list = children as List<ResourceModel>;
            if (list != null)
            {
                list.Insert(0, root);
            }
            else if (list == null)
            {
                list = new List<ResourceModel>(children.Count + 1);
                list.Add(root);
                list.AddRange(children);
            }

            var tree = children.BuildTree();
            this.Resource = tree;
        }

        private async Task LoadOnline()
        {
            if (!_polar.TryGetSite(Param_Link.Site, out ISite site))
            {
                _notify.Show($"Can not fond Site [{Param_Link.Site}]", NotificationType.Warning);
                return;
            }

            IsLoading = true;
            try
            {
                var result = await site.GetDetailAsync(this.Param_Link);
                if (result == null)
                {
                    _notify.Show("No Data");
                    return;
                }

                this.Resource = result.ToResourceTreeNode();
                if (_rootResourceInDb != null)
                {
                    this.Resource.Id = _rootResourceInDb.Id;
                }

                if (IsSaved)
                {
                    await SaveResource();
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }
            IsLoading = false;
        }

        private async Task ResourceOperate()
        {
            if (IsSaved)
            {
                await DeleteResource();
            }
            else
            {
                await SaveResource();
            }
        }

        private async Task SaveResource()
        {
            try
            {
                await _mineResourceService.SaveResourceAsync(this.Resource);

                IsSaved = true;

                if (!string.IsNullOrEmpty(this.Resource.ImageSrc))
                {
                    await _bufferCache.CacheFileIfExisedInMemory(BufferCache.SHA(this.Resource.ImageSrc));
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        private async Task DeleteResource()
        {
            try
            {
                await _mineResourceService.DeleteRootResourceAsync(this.Resource.Id);

                if (!string.IsNullOrEmpty(this.Resource.ImageSrc))
                {
                    _bufferCache.Remove(BufferCache.SHA(this.Resource.ImageSrc), BufferLocation.File);
                }
                IsSaved = false;
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        private async Task LinkClick(ResourceTreeNode node)
        {
            try
            {
                switch (node.SrcType)
                {
                    case LinkType.Magnet:
                        await StartMagnet(node);
                        break;
                    case LinkType.HTML:
                        break;
                    default:
                        _notify.Show($"Not support type:{node.SrcType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }

        private async Task StartMagnet(ResourceTreeNode node)
        {
            var rpc = await _preference.GetAsync(PreferenceOption.RPC, string.Empty);
            var downloadFolder = await _preference.GetAsync(PreferenceOption.DownloadPath, string.Empty);
            if (string.IsNullOrEmpty(rpc) || string.IsNullOrEmpty(downloadFolder))
            {
                _notify.Show("Aria2 configuration information is incomplete");
                return;
            }

            try
            {
                using (var aria2 = new Aria2Http(new Uri(rpc)))
                {
                    var request = aria2.CreateAddUri(new string[] { node.Src }, new Aria2InputFileOption
                    {
                        Dir = @$"{downloadFolder}\{Resource.Name}"
                    });

                    var result = await aria2.PostAsync(request);
                    if (result.IsOk())
                    {
                        _notify.Show($"{Resource.Name} downdload started");
                    }
                    else if (result.IsError())
                    {
                        _notify.Show(result.Error.Message, NotificationType.Error);
                    }
                    else
                    {
                        _notify.Show(result.Result);
                    }
                }
            }
            catch (Exception ex)
            {
                _notify.Show(ex);
            }

        }
    }
}
