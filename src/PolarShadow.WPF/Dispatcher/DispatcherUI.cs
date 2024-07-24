using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolarShadow.Dispatcher
{
    internal class DispatcherUI : IDispatcherUI
    {
        public bool CheckAccess()
        {
            return Application.Current?.Dispatcher == System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        public void Post(Action action)
        {
            Application.Current?.Dispatcher.Invoke(action);
        }
    }
}
