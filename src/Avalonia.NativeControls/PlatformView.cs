using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public abstract class PlatformView : IPlatformView
    {
        public IntPtr Handle { get; private set; }

        public string HandleDescriptor { get; private set; }

        public IPlatformHandle CreateControl(IPlatformHandle parent, Func<IPlatformHandle> createDefault)
        {
            var handle = createDefault();

            this.Handle = handle.Handle;
            this.HandleDescriptor = handle.HandleDescriptor;

            CreateControl();
            return handle;
        }


        public void DestroyControl(IPlatformHandle handler)
        {
            Handle = IntPtr.Zero;
            HandleDescriptor = string.Empty;
            DestroyControl();
        }
        protected abstract void CreateControl();
        protected abstract void DestroyControl();
    }
}
