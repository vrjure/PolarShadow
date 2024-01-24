using Avalonia.Controls;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PolarShadow.Models;
using PolarShadow.Navigations;
using PolarShadow.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels;

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

    private ISelectionModel _selectionModel;
    public ISelectionModel SelectionModel
    {
        get => _selectionModel;
        set
        {
            SetProperty(_selectionModel, value, n =>
            {
                if (_selectionModel != null)
                {
                    _selectionModel.SelectionChanged -= SelectionModel_SelectionChanged;
                }
                _selectionModel = n;
                _selectionModel.SelectionChanged += SelectionModel_SelectionChanged;
            });

        }
    }

    protected override void OnActivated()
    {
        if (Design.IsDesignMode) return;
        System.Diagnostics.Trace.WriteLine($"{this.GetType().Name} load");
        if (_loaded) return;
        _loaded = true;

        _cts = new CancellationTokenSource();
        Load();
        base.OnActivated();
    }

    protected override void OnDeactivated()
    {
        if (Design.IsDesignMode) return;

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

    protected virtual void OnSelectionChanged(SelectionModelSelectionChangedEventArgs e)
    {

    }

    private void SelectionModel_SelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
    {
        OnSelectionChanged(e);
    }

    protected Task WhenTaskCompleted(Func<bool> task, int millisecondsDelay = 100)
    {
        return Task.Run(async () =>
        {
            while (!task())
            {
                await Task.Delay(millisecondsDelay);
            }
        });
    }
}
