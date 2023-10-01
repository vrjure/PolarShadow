using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public abstract class ViewHandler: IViewHandler
    {
        public IVirtualView VirtualView { get; private set; }

        public IPlatformView PlatformView { get; protected set; }

        protected abstract IPlatformView OnCreatePlatformView();

        public virtual void SetVirtualView(IVirtualView virtualView)
        {
            _ = virtualView ?? throw new ArgumentNullException(nameof(virtualView));

            if (VirtualView == virtualView)
            {
                return;
            }

            VirtualView = virtualView;
            PlatformView = OnCreatePlatformView();

            if (virtualView.Handler != this)
            {
                virtualView.Handler = this;
            }
        }
    }

    public abstract class ViewHandler<TVirtualView, TPlatformView> : ViewHandler where TVirtualView : IVirtualView where TPlatformView : IPlatformView
    {
        public new TVirtualView VirtualView => (TVirtualView)base.VirtualView;
        public new TPlatformView PlatformView
        {
            get => (TPlatformView)base.PlatformView;
            set =>  base.PlatformView = value;
        }
    }
}
