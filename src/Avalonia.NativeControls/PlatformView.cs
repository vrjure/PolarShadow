using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public abstract class PlatformView : IPlatformView
    {
        protected IPlatformHandle Handle { get; private set; }

        public IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            Handle = OnCreateControl(parent, createDefault) ?? createDefault();
            return Handle;
        }

        public void DestroyControl(IPlatformHandle handler)
        {
            DestroyControl();
            Handle = null;
        }

        protected virtual IPlatformHandle OnCreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            return null;
        }
        protected virtual void DestroyControl()
        {

        }
    }
}
