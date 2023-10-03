using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PolarShadow.Models;
using PolarShadow.Navigations;
using System;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels;

public class ViewModelBase : ObservableRecipient, INavigationNotify
{
    private bool _loaded = false;


    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _hasData = true;
    public bool HasData
    {
        get => _hasData;
        set => SetProperty(ref _hasData, value);
    }
    protected override void OnActivated()
    {
        System.Diagnostics.Trace.WriteLine($"{this.GetType().Name} load");
        if (_loaded) return;
        _loaded = true;

        Load();
        base.OnActivated();
    }

    protected override void OnDeactivated()
    {
        System.Diagnostics.Trace.WriteLine($"{this.GetType().Name} unload");
        if (!_loaded) return;
        _loaded = false;

        Unload();
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
}
