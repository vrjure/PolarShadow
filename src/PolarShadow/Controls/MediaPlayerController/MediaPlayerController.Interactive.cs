using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public partial class MediaPlayerController
    {
        private static Point _zero = new Point(0, 0);
        private static int _pointerActiveStartDistance = 30;
        private static int _autoHideDelaySec = 5;

        private Point _cursorPoint;
        private bool _isScrolling;
        private int _scrollChangeTime;

        private IDisposable _hideDisposable;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled || e.KeyModifiers != KeyModifiers.None || _part_slider == null) return;

            var target = MediaController.Controller.Time;
            var handled = false;
            var seek = false;
            switch (e.Key)
            {
                case Key.Left:
                    target = target - TimeSpan.FromSeconds(_part_slider.SmallChange);
                    seek = handled = true;
                    break;
                case Key.Right:
                    target = target + TimeSpan.FromSeconds(_part_slider.SmallChange);
                    seek = handled = true;
                    break;
                case Key.Space:
                    PlayPause();
                    handled = true;
                    break;
            }

            if (handled && seek)
            {
                if (target < TimeSpan.Zero)
                {
                    target = TimeSpan.Zero;
                }
                else if (target > MediaController.Controller.Length)
                {
                    target = MediaController.Controller.Length;
                }

                MediaController.Controller.Time = target;
            }

            e.Handled = handled;

            base.OnKeyDown(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_part_root == null || _cursorPoint == _zero)
            {
                return;
            }
            var current = e.GetPosition(this);
            var dis = Point.Distance(_cursorPoint, current);
            if (dis > _pointerActiveStartDistance)
            {
                if (!IsShow())
                {
                    ShowAndAutoHide();
                }
            }
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            AutoHide();
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);

            CancelAutoHide();

            if (!IsShow())
            {
                Show();
            }
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            OnReleased(e.GetPosition(this));
        }

        private void OnHolding(HoldingRoutedEventArgs e)
        {
            if (MediaController?.Controller == null)
            {
                return;
            }

            if (e.HoldingState == HoldingState.Started)
            {
                MediaController.Controller.Speed = 2;
            }
            else
            {
                MediaController.Controller.Speed = 1;
            }

            //System.Diagnostics.Trace.WriteLine($"Play speed: {MediaController.Controller.Speed}");
        }

        private void OnScroll(ScrollGestureEventArgs e)
        {
            if (MediaController?.Controller == null)
            {
                return;
            }
            //System.Diagnostics.Trace.WriteLine($"Scroll: {e.Delta}");

            if (_isScrolling)
            {
                _scrollChangeTime -= (int)e.Delta.X;
            }
            else
            {
                _scrollChangeTime = 0;
                _isScrolling = true;
            }
        }

        private void OnScrollEnd(ScrollGestureEndedEventArgs e)
        {
            if (MediaController?.Controller == null) return;
            _isScrolling = false;
            //System.Diagnostics.Trace.WriteLine($"Scroll change: {_scrollChangeTime}");

            if (Math.Abs(_scrollChangeTime) <= 1)
            {
                return;
            }

            MediaController.Controller.Time += TimeSpan.FromSeconds(_scrollChangeTime);
            AutoHide();
        }

        private void OnNativePointerReleased(NativePointerPointEventArgs e)
        {
            OnReleased(e.Point);
        }

        private void OnReleased(Point point)
        {
            if (_part_root == null) return;

            if (IsShow())
            {
                Hide();
                _cursorPoint = point;
            }
            else
            {
                _cursorPoint = point;
                ShowAndAutoHide();
            }
        }

        private void ShowAndAutoHide()
        {
            Show();
            AutoHide();
        }

        private void AutoHide()
        {
            CancelAutoHide();

            if (IsShow())
            {
                _hideDisposable = DispatcherTimer.RunOnce(Hide, TimeSpan.FromSeconds(_autoHideDelaySec));
            }        
        }

        private void CancelAutoHide()
        {
            if (_hideDisposable != null)
            {
                _hideDisposable.Dispose();
                _hideDisposable = null;
            }
        }
    }
}
