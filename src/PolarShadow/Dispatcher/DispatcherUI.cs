using Avalonia.Threading;
using PolarShadow.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Dispatcher
{
    internal class DispatcherUI : IDispatcherUI
    {
        public bool CheckAccess()
        {
            return Avalonia.Threading.Dispatcher.UIThread.CheckAccess();
        }

        public void Post(Action action)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(action);
        }
    }
}
