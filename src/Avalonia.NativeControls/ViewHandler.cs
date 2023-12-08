using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.NativeControls
{
    public abstract class ViewHandler : IViewHandler
    {
        public IVirtualView VirtualView { get; private set; }

        public IPlatformView PlatformView { get; protected set; }

        public void DisconnectHandler()
        {
            if (this.PlatformView == null)
            {
                return;
            }
            DisconnectHandler(this.PlatformView);
            this.PlatformView = null;
            this.VirtualView = null;
        }

        protected abstract IPlatformView OnCreatePlatformView();

        private protected virtual void ConnectHandler(IPlatformView platformView)
        {
            
        }

        private protected virtual void DisconnectHandler(IPlatformView platformView)
        {
            
        }

        public void SetVirtualView(IVirtualView virtualView)
        {
            _ = virtualView ?? throw new ArgumentNullException(nameof(virtualView));

            if (VirtualView == virtualView)
            {
                return;
            }

            VirtualView = virtualView;
            PlatformView = OnCreatePlatformView();

            if (PlatformView != null)
            {
                ConnectHandler(this.PlatformView);
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


        protected virtual void ConnectHandler(TPlatformView platformView)
        {

        }

        protected virtual void DisconnectHandler(TPlatformView platformView)
        {

        }

        private protected override void ConnectHandler(IPlatformView platformView) => ConnectHandler((TPlatformView)platformView);

        private protected override void DisconnectHandler(IPlatformView platformView) => DisconnectHandler((TPlatformView)platformView);
    }
}
