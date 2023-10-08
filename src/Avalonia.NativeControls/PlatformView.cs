using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public abstract class PlatformView : IPlatformView
    {
        public IPlatformHandle Handle { get; set; }

        public IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            Handle = CreateControl(parent) ?? createDefault();
            return Handle;
        }


        public void DestroyControl(IPlatformHandle handler)
        {
            DestroyControl();
            Handle = null;
        }
        protected abstract IPlatformHandle CreateControl(IPlatformHandle parent);
        protected abstract void DestroyControl();
    }
}
