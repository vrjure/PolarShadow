using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    internal sealed class SwipeAttached
    {
        private static double _swipeMinDistance = 100;

        public static readonly AttachedProperty<double> SwipeDistanceProperty = AvaloniaProperty.RegisterAttached<SwipeAttached, Interactive, double>("SwipeDistance", 100);
        public static double GetSwipeDistance(Interactive input)
        {
            return input.GetValue(SwipeDistanceProperty);
        }
        public static void SetSwipeDistance(Interactive input, double value)
        {
            input.SetValue(SwipeDistanceProperty, value);
        }

        public static readonly AttachedProperty<SwipeDirection> SwipeDirectionProperty = AvaloniaProperty.RegisterAttached<SwipeAttached, Interactive, SwipeDirection>("SwipeDirection");
        public static SwipeDirection GetSwipeDirection(Interactive input)
        {
            return input.GetValue(SwipeDirectionProperty);
        }
        public static void SetSwipeDirection(Interactive input, SwipeDirection value)
        {
            input.SetValue(SwipeDirectionProperty, value);
        }

        public static readonly RoutedEvent<SwipingEventArgs> SwipingEvent = RoutedEvent.Register<SwipingEventArgs>("Swiping", RoutingStrategies.Bubble, typeof(SwipeAttached));
        public static void AddSwipingHandler(Interactive element, EventHandler<RoutedEvent<SwipingEventArgs>> handler)
        {
            element.AddHandler(SwipingEvent, handler);
        }
        public static void RemoveSwipingHandler(Interactive element, EventHandler<RoutedEvent<SwipingEventArgs>> handler)
        {
            element.RemoveHandler(SwipingEvent, handler);
        }

        public static readonly RoutedEvent<SwipedEventArgs> SwipedEvent = RoutedEvent.Register<SwipedEventArgs>("Swiped", RoutingStrategies.Bubble, typeof(SwipeAttached));
        public static void AddSwipedHandler(Interactive element, EventHandler<RoutedEvent<SwipedEventArgs>> handler)
        {
            element.AddHandler(SwipedEvent, handler);
        }
        public static void RemoveSwipedHandler(Interactive element, EventHandler<RoutedEvent<SwipedEventArgs>> handler)
        {
            element.RemoveHandler(SwipedEvent, handler);
        }

        public static void ScrollGestureHandler(Interactive element, ScrollGestureEventArgs e)
        {
            var distance = GetSwipeDistance(element);
            var direction = GetSwipeDirection(element);

            //System.Diagnostics.Trace.WriteLine(e.Delta);
            if (direction == SwipeDirection.None)
            {
                if (Math.Abs(e.Delta.X) > 10)
                {
                    return;
                }

                if (Math.Abs(e.Delta.X) > Math.Abs(e.Delta.Y))
                {
                    if (e.Delta.X > 0)
                    {
                        direction = SwipeDirection.RightToLeft;
                    }
                    else
                    {
                        direction = SwipeDirection.LeftToRight;
                    }
                }
                else
                {
                    if (e.Delta.Y > 0)
                    {
                        direction = SwipeDirection.BottomToTop;
                    }
                    else
                    {
                        direction = SwipeDirection.TopToBottom;
                    }
                }

                SetSwipeDirection(element, direction);
            }

            if (direction == SwipeDirection.None) return;

            switch (direction)
            {
                case SwipeDirection.TopToBottom:
                case SwipeDirection.BottomToTop:
                    distance -= e.Delta.Y;
                    SetSwipeDistance(element, distance);
                    break;
                case SwipeDirection.LeftToRight:
                case SwipeDirection.RightToLeft:
                    distance -= e.Delta.X;
                    SetSwipeDistance(element, distance);
                    break;
            }

            element.RaiseEvent(new SwipingEventArgs(direction, distance));
        }

        public static void ScrollGestureEndHandler(Interactive element, ScrollGestureEndedEventArgs e)
        {
            var direction = GetSwipeDirection(element);
            var distance = GetSwipeDistance(element);

            //System.Diagnostics.Trace.WriteLine($"direction:{direction}; distance:{distance}");

            if (direction == SwipeDirection.None || Math.Abs(distance) < _swipeMinDistance)
            {
                SetSwipeDirection(element, SwipeDirection.None);
                SetSwipeDistance(element, 0);
                return;
            }

            element.RaiseEvent(new SwipedEventArgs(direction));

            SetSwipeDirection(element, SwipeDirection.None);
            SetSwipeDistance(element, 0);
        }
    }
}
