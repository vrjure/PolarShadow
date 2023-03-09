using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal class NavigationState
    {
        public NavigationState(string toUrl) 
        { 
            ToUrl = toUrl;
        }

        public string ToUrl { get; set; }
        public string FromUrl { get; set; }
        public bool CanBack { get; set; }
        public IDictionary<string, object> States { get; set; }
        public IDictionary <string, object> Parameters { get; set; }
    }
}
