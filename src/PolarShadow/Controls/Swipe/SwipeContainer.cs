using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    [TemplatePart("PART_RefreshIndicator", typeof(Control))]
    [TemplatePart("PART_RfreshIndicatorIcon", typeof(Control))]
    [PseudoClasses(":pull", ":refreshing", ":loadMore", ":loading", ":normal", ":noMore", ":reset")]
    public partial class SwipeContainer : ContentControl
    {
        private static readonly double resetScale = 0.2;
        private static readonly LinearEasing resetEasing = new LinearEasing();

        private ScrollGestureRecognizer _scrollGestureRecognizer;
        private Vector _offset;
        private SwipeDirection _currentDirection;
        private bool _canOffset = false;

        public static readonly StyledProperty<SwipeDirection> SwipeDirectionProperty = AvaloniaProperty.Register<SwipeContainer, SwipeDirection>(nameof(SwipeDirection), SwipeDirection.None);
        public SwipeDirection SwipeDirection
        {
            get => GetValue(SwipeDirectionProperty);
            set => SetValue(SwipeDirectionProperty, value);
        }

        public static readonly DirectProperty<SwipeContainer, int> SwipeStartDistanceProperty = ScrollGestureRecognizer.ScrollStartDistanceProperty.AddOwner<SwipeContainer>(g => g.SwipeStartDistance, (s, v) => s.SwipeStartDistance = v);
        public int SwipeStartDistance
        {
            get => _scrollGestureRecognizer.ScrollStartDistance;
            set
            {
                var old = _scrollGestureRecognizer.ScrollStartDistance;
                if (SetAndRaise(SwipeStartDistanceProperty, ref old, value))
                {
                    _scrollGestureRecognizer.ScrollStartDistance = value;
                }
            }
        }

        static SwipeContainer()
        {
            Gestures.ScrollGestureEvent.AddClassHandler<SwipeContainer>((s, e) => s.ScrollGestureEvent(e));
            Gestures.ScrollGestureEndedEvent.AddClassHandler<SwipeContainer>((s, e) => s.ScrollGestureEndEvent(e));
        }

        public SwipeContainer()
        {
            _scrollGestureRecognizer = new() { CanHorizontallyScroll = false, CanVerticallyScroll = false };
            GestureRecognizers.Add(_scrollGestureRecognizer);
            ResetState();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _refreshIndicator = e.NameScope.Find<Control>("PART_RefreshIndicator");
            _refreshIndicatorIcon = e.NameScope.Find<Control>("PART_RefreshIndicatorIcon");

            InitRefresh();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == SwipeDirectionProperty)
            {
                SwipeDirectionPropertyChanged(change);
            }
            else if (change.Property == RefreshProperty)
            {
                RefreshPropertyChanged(change);
            }
            else if (change.Property == LoadingProperty)
            {
                LoadingPropertyChanged(change);
            }
            else if (change.Property == CanLoadMoreProperty)
            {
                CanLoadMorePropertyChanged(change);
            }
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            ResetState();
            ResetRefreshIndicator();
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            ResetRefreshVisual();
        }

        private void SwipeDirectionPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var val = e.GetNewValue<SwipeDirection>();
            switch (val)
            {
                case SwipeDirection.TopToBottom:
                case SwipeDirection.BottomToTop:
                    _scrollGestureRecognizer.CanVerticallyScroll = true;
                    break;
                case SwipeDirection.LeftToRight:
                case SwipeDirection.RightToLeft:
                    _scrollGestureRecognizer.CanHorizontallyScroll = true;
                    break;
                default:
                    _scrollGestureRecognizer.CanHorizontallyScroll = _scrollGestureRecognizer.CanVerticallyScroll = false;
                    break;
            }
        }

        private void ScrollGestureEvent(ScrollGestureEventArgs e)
        {
            if (SwipeDirection == SwipeDirection.None) return;

            var x = e.Delta.X;
            var y = e.Delta.Y;
            var x_abs = Math.Abs(x);
            var y_abs = Math.Abs(y);

            if (!_canOffset)
            {
                if (SwipeDirection.HasFlag(SwipeDirection.TopToBottom)
                && x_abs >= y_abs
                && y < 0)
                {
                    _canOffset = true;
                    _currentDirection = SwipeDirection.TopToBottom;
                }
                else if (SwipeDirection.HasFlag(SwipeDirection.BottomToTop)
                    && x_abs >= y_abs
                    && y > 0)
                {
                    _canOffset = true;
                    _currentDirection = SwipeDirection.BottomToTop;
                }
                else if (SwipeDirection.HasFlag(SwipeDirection.LeftToRight)
                    && x_abs < y_abs
                    && x < 0)
                {
                    _canOffset = true;
                    _currentDirection = SwipeDirection.LeftToRight;
                }
                else if (SwipeDirection.HasFlag(SwipeDirection.RightToLeft)
                    && x_abs < y_abs
                    && x > 0)
                {
                    _canOffset = true;
                    _currentDirection = SwipeDirection.RightToLeft;
                }
            }

            if (!_canOffset)
            {
                return;
            }

            _offset -= e.Delta;
            OnScroll(e.Delta);
        }

        private void ScrollGestureEndEvent(ScrollGestureEndedEventArgs e)
        {
            OnScrollEnd(_offset);
            _offset = Vector.Zero;
            _currentDirection = SwipeDirection.None;
            _canOffset = false;
        }

        private void OnScroll(Vector delta)
        {
            if (_currentDirection == SwipeDirection.TopToBottom)
            {
                TopToBottom(delta.Y);
            }
            else if (_currentDirection == SwipeDirection.BottomToTop)
            {
                if (IsRefreshState())
                {
                    TopToBottom(delta.Y);
                }
                else if(IsNormalState())
                {
                    BottomToTopEnd(delta.Y);
                }
            }
        }

        private void OnScrollEnd(Vector offset)
        {
            switch (_currentDirection)
            {
                case SwipeDirection.TopToBottom:
                    TopToBottomEnd();
                    break;
                case SwipeDirection.BottomToTop:
                    if (IsRefreshState())
                    {
                        TopToBottomEnd();
                    }
                    else
                    {
                        BottomToTopEnd(_offset.Y);
                    }
                    break;
                case SwipeDirection.LeftToRight:
                    break;
                case SwipeDirection.RightToLeft:
                    break;
                default:
                    break;
            }
        }

        private bool IsNormalState() => PseudoClasses.Contains(":normal");

        private void ResetState()
        {
            PseudoClasses.Set(":reset", true);
            PseudoClasses.Set(":normal", false);
            PseudoClasses.Set(":pull", false);
            PseudoClasses.Set(":refreshing", false);
            PseudoClasses.Set(":loadMore", false);
            PseudoClasses.Set(":loading", false);
        }

        private void NormalState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":pull", false);
            PseudoClasses.Set(":refreshing", false);
            PseudoClasses.Set(":loadMore", false);
            PseudoClasses.Set(":loading", false);
        }
    }
}
