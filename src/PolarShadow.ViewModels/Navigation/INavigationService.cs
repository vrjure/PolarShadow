using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigation
{
    public interface INavigationService    
    {
        bool CanBack(string container);
        void Back(string container);
        void Navigate(string container, Type vmType, IDictionary<string, object> parameters, bool canBack);
        
    }
}
