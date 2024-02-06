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
        private int _scrollChangedX;
        private bool? _scrollHorizontal;
        private Rect _leftRect;
        private bool _pressedOnLeft;
        private double _volumeDelta;
        private double _brightnessDelta;

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
                if (!_scrollHorizontal.HasValue)
                {
                    if (Math.Abs(e.Delta.X) > Math.Abs(e.Delta.Y))
                    {
                        _scrollHorizontal = true;
                    }
                    else
                    {
                        _scrollHorizontal = false;
                    }
                }

                if (_scrollHorizontal == true)
                {
                    OnHorizontalScroll(e);
                }
                else
                {
                    OnVerticalScroll(e);
                }
            }
            else
            {
                _scrollChangedX = 0;
                _scrollHorizontal = null;
                _isScrolling = true;
            }
        }

        private void OnHorizontalScroll(ScrollGestureEventArgs e)
        {
            _scrollChangedX -= (int)Math.Round(e.Delta.X);
        }

        private void OnVerticalScroll(ScrollGestureEventArgs e)
        {
            if (MediaController?.FullScreen == true)
            {
                if (_deviceService != null)
                {
                    if (_pressedOnLeft)
                    {
                        _brightnessDelta += e.Delta.Y;
                        if (Math.Abs(_brightnessDelta) >= 1)
                        {
                            _deviceService.Brightness += (int)_brightnessDelta;
                            _brightnessDelta = 0;
                        }
                    }
                    else
                    {
                        _volumeDelta += e.Delta.Y;
                        if (Math.Abs(_volumeDelta) >= 1)
                        {
                            _deviceService.Volume += (int)_volumeDelta;
                            _volumeDelta = 0;
                        }
                    }
                }
            }
        }

        private void OnScrollEnd(ScrollGestureEndedEventArgs e)
        {
            if (MediaController?.Controller == null) return;
            _isScrolling = false;
            //System.Diagnostics.Trace.WriteLine($"Scroll change: {_scrollChangeTime}");

            if (_scrollHorizontal == true)
            {
                if (Math.Abs(_scrollChangedX) <= 1)
                {
                    return;
                }

                MediaController.Controller.Time += TimeSpan.FromSeconds(_scrollChangedX);
            }

            AutoHide();
        }

        private void OnNativePointerPressed(NativePointerPointEventArgs e)
        {
            if (_leftRect.ContainsExclusive(e.Point))
            {
                _pressedOnLeft = true;
            }
            else
            {
                _pressedOnLeft = false;
            }
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
