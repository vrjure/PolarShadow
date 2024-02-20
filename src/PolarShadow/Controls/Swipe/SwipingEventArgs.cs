using Avalonia;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class SwipingEventArgs : RoutedEventArgs
    {
        public SwipingEventArgs(SwipeDirection direction, double delta) :base(SwipeAttached.SwipingEvent)
        {
            Direction = direction;
            Delta = delta;
        }

        public SwipeDirection Direction { get; }
        public double Delta { get; }
    }
}
