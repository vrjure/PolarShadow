using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Navigations
{
    public sealed partial class NavigationManager
    {
        private static readonly Dictionary<Type,Type> _vm_view = new Dictionary<Type,Type>();

        public static void Add(Type vmType, Type vType)
        {
            _vm_view.Add(vmType, vType);
        }

        public static bool TryGetView(Type vmType, out Type vType)
        {
            return _vm_view.TryGetValue(vmType, out vType);
        }
    }
}
