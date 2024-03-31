using CommunityToolkit.Mvvm.ComponentModel;
using PolarShadow.Navigations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels
{
    public class ViewModelBase : ObservableRecipient, INavigationNotify
    {
        private object _lock = new object();
        private bool _loaded = false;
        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    IsLoadingChanged();
                }
            }
        }

        private bool _hasData = true;
        public bool HasData
        {
            get => _hasData;
            set => SetProperty(ref _hasData, value);
        }

        private CancellationTokenSource _cts;
        public CancellationTokenSource Cancellation
        {
            get
            {
                lock (_lock)
                {
                    if (_cts == null) return _cts = new CancellationTokenSource();
                    if (_cts.IsCancellationRequested)
                    {
                        _cts.Dispose();
                        return _cts = new CancellationTokenSource();
                    }
                }
                return _cts;
            }
        }

        protected override void OnActivated()
        {
            System.Diagnostics.Trace.WriteLine($"{this.GetType().Name} load");
            if (_loaded) return;
            _loaded = true;

            _cts = new CancellationTokenSource();
            Load();
            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            System.Diagnostics.Trace.WriteLine($"{this.GetType().Name} unload");
            if (!_loaded) return;
            _loaded = false;

            Unload();

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            base.OnDeactivated();
        }

        public void Load()
        {
            OnLoad();
        }

        public void Unload()
        {
            OnUnload();
        }

        protected virtual void OnLoad()
        {

        }

        protected virtual void OnUnload()
        {

        }

        protected virtual void IsLoadingChanged()
        {

        }
    }
}
