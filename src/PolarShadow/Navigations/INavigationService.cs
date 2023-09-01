﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public interface INavigationService    
    {
        bool CanBack(string container);
        void Back(string container);
        void Navigate(string container, Type viewType, IDictionary<string, object> parameters, bool canBack);
        
    }
}
