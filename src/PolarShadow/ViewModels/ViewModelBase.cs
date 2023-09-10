using CommunityToolkit.Mvvm.ComponentModel;
using PolarShadow.Navigations;
using System.Threading.Tasks;

namespace PolarShadow.ViewModels;

public class ViewModelBase : ObservableObject, INavigationNotify
{
    public virtual void OnLoad()
    {
        
    }

    public virtual Task OnLoadAsync()
    {
        return Task.CompletedTask;
    }

    public virtual void OnUnload()
    {
        
    }
}
