using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class SwipedEventArgs : RoutedEventArgs
    {
        public SwipedEventArgs(SwipeDirection direction) : base(SwipeAttached.SwipedEvent)
        {
            Direction = direction;
        }

        public SwipeDirection Direction { get; }
    }
}
