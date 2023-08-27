using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public interface INavigationService    
    {
        void Navigate(string container, Type viewType, INavigationParameters parameters);
        
    }
}
