using CommunityToolkit.Mvvm.ComponentModel;
using PolarShadow.Navigations;

namespace PolarShadow.ViewModels;

public class ViewModelBase : ObservableObject, INavigationNotify
{
    public virtual void OnLoad()
    {
        
    }

    public virtual void OnUnload()
    {
        
    }
}
