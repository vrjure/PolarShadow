using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Pages.ViewModels
{
    public class ConfigPageViewModel : ObservableObject
    {

        public bool UserServer
        {
            get => PolarShadowSetting.UseServer;
            set
            {
                SetProperty(PolarShadowSetting.UseServer, value, callback =>
                {
                    PolarShadowSetting.UseServer = callback;

                });
            }
        }

        public string ServerAddress
        {
            get => PolarShadowSetting.Server;
            set
            {
                SetProperty(PolarShadowSetting.Server, value, callback =>
                {
                    PolarShadowSetting.Server = callback;
                });
            }
        }
    }
}
