using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public abstract class VirtualView : NativeControlHost, IVirtualView
    {
        static VirtualView()
        {
            ContentProperty.Changed.AddClassHandler<VideoView>((s, e) =>
            {
                s.SetOverlayerContentIf();
            });
        }
        public VirtualView(IViewHandler handler)
        {
            this.Handler = handler;
            this.Handler.SetVirtualView(this);

        }
        public IViewHandler Handler { get; }

        public static readonly StyledProperty<object> ContentProperty = ContentControl.ContentProperty.AddOwner<VideoView>();
        [Content]
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            if (Handler == null || Handler.PlatformView == null)
            {
                return base.CreateNativeControlCore(parent);
            }

            SetOverlayerContentIf();

            return Handler.PlatformView.CreateControl(parent, () => base.CreateNativeControlCore(parent));
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            this.Handler?.PlatformView?.DestroyControl(control);
            this.Handler?.DisconnectHandler();
            base.DestroyNativeControlCore(control);
        }

        private void SetOverlayerContentIf()
        {
            if (Handler.PlatformView is IOverlayerContent overlayer)
            {
                overlayer.OverlayContent = Content;
            }
        }
    }
}
