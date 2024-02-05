using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    public abstract class VirtualView : NativeControlHost, IVirtualView, INativeInteraction
    {

        private IDisposable _holdDispose;
        private bool _holding;
        private Point _lastPressedPoint;
        private bool _pressed;
        private Point _lastScrollPoint;
        private int _scrollStartDistance = 30;
        private bool _scrolling;
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

        public static readonly RoutedEvent<NativePointerPointEventArgs> NativePointerPressedEvent = RoutedEvent.Register<VirtualView, NativePointerPointEventArgs>(nameof(NativePointerPressed), RoutingStrategies.Bubble);
        public event EventHandler<NativePointerPointEventArgs> NativePointerPressed
        {
            add => AddHandler(NativePointerPressedEvent, value);
            remove => RemoveHandler(NativePointerPressedEvent, value);
        }

        public static readonly RoutedEvent<NativePointerPointEventArgs> NativePointerReleasedEvent = RoutedEvent.Register<VirtualView, NativePointerPointEventArgs>(nameof(NativePointerReleased), RoutingStrategies.Bubble);
        public event EventHandler<NativePointerPointEventArgs> NativePointerReleased
        {
            add => AddHandler(NativePointerReleasedEvent, value);
            remove => RemoveHandler(NativePointerReleasedEvent, value);
        }

        public static readonly RoutedEvent<NativePointerPointEventArgs> NativePointerMovedEvent = RoutedEvent.Register<VirtualView, NativePointerPointEventArgs>(nameof(NativePointerMoved), RoutingStrategies.Bubble);
        public event EventHandler<NativePointerPointEventArgs> NativePointerMoved
        {
            add => AddHandler(NativePointerMovedEvent, value);
            remove => RemoveHandler(NativePointerMovedEvent, value);
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

        public void OnNativePointerPoint(NativePointerPointEventArgs e)
        {
            if (e.Event == Input.Raw.RawPointerEventType.TouchBegin
                || e.Event == Input.Raw.RawPointerEventType.LeftButtonDown
                || e.Event == Input.Raw.RawPointerEventType.RightButtonDown)
            {
                OnNativePointerPressedInternal(e);
            }
            else if (e.Event == Input.Raw.RawPointerEventType.TouchEnd
                || e.Event == Input.Raw.RawPointerEventType.LeftButtonUp
                || e.Event == Input.Raw.RawPointerEventType.RightButtonDown)
            {
                OnNativePointerReleasedInternal(e);
            }
            else if (e.Event == Input.Raw.RawPointerEventType.TouchUpdate
                || e.Event == Input.Raw.RawPointerEventType.Move)
            {
                if (e.Event == Input.Raw.RawPointerEventType.TouchUpdate)
                {
                    var settings = ((IInputRoot)this.GetVisualRoot())?.PlatformSettings;
                    var size = settings.GetTapSize(PointerType.Touch);
                    var tabRect = new Rect(_lastPressedPoint, new Size())
                        .Inflate(new Thickness(size.Width, size.Height));

                    if (tabRect.ContainsExclusive(e.Point))
                    {
                        return;
                    }
                }

                OnNativePointerMovedInternal(e);
            }
        }

        private void OnNativePointerPressedInternal(NativePointerPointEventArgs e)
        {
            e.RoutedEvent = NativePointerPressedEvent;
            this.RaiseEvent(e);

            _lastScrollPoint = _lastPressedPoint = e.Point;
            _holding = false;
            _pressed = true;

            var settings = ((IInputRoot)this.GetVisualRoot())?.PlatformSettings;
            if (settings != null)
            {
                if (_holdDispose != null)
                {
                    _holdDispose.Dispose();
                }

                _holdDispose = DispatcherTimer.RunOnce(() =>
                {
                    _holding = true;
                    this.RaiseEvent(new HoldingRoutedEventArgs(HoldingState.Started, e.Point, e.Type));

                }, settings.HoldWaitDuration);
            }
        }

        private void OnNativePointerReleasedInternal(NativePointerPointEventArgs e)
        {
            _pressed = false;

            if (_holding)
            {
                _holding = false;
                this.RaiseEvent(new HoldingRoutedEventArgs(HoldingState.Completed, e.Point, e.Type));
                e.Handled = true;
            }
            else
            {
                _holdDispose?.Dispose();
                _holdDispose = null;
            }

            if (_scrolling)
            {
                _scrolling = false;
                this.RaiseEvent(new ScrollGestureEndedEventArgs(e.Id));
                e.Handled = true;
            }

            if (!e.Handled)
            {
                e.RoutedEvent = NativePointerReleasedEvent;
                this.RaiseEvent(e);
            }
        }

        private void OnNativePointerMovedInternal(NativePointerPointEventArgs e)
        {
            if (_holding)
            {
                _holding = false;
                this.RaiseEvent(new HoldingRoutedEventArgs(HoldingState.Cancelled, e.Point, e.Type));
                e.Handled = true;
            }
            else
            {
                _holdDispose?.Dispose();
                _holdDispose = null;
            }

            if (!_scrolling)
            {
                if (Math.Abs(_lastPressedPoint.X - e.Point.X) > _scrollStartDistance
                    || Math.Abs(_lastPressedPoint.Y - e.Point.Y) > _scrollStartDistance)
                {
                    _scrolling = true;
                    _lastScrollPoint = e.Point;
                }
            }

            if (_scrolling)
            {
                var delta = _lastScrollPoint - e.Point;
                _lastScrollPoint = e.Point;
                this.RaiseEvent(new ScrollGestureEventArgs(e.Id, delta));
                e.Handled = true;
            }

            if (!e.Handled)
            {
                e.RoutedEvent = NativePointerMovedEvent;
                this.RaiseEvent(e);
            }
        }
    }
}
