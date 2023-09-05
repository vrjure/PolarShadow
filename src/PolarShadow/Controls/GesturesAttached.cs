using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public class GesturesAttached
    {
        static GesturesAttached()
        {
            ScrollEndCommandProperty.Changed.Subscribe(ScrollEndCommandPropertyChanged);
            PullEndCommandProperty.Changed.Subscribe(PullEndCommandPropertyChanged);
        }

        public static readonly AttachedProperty<ICommand> ScrollEndCommandProperty = AvaloniaProperty.RegisterAttached<GesturesAttached, InputElement, ICommand>("ScrollEndCommand");
        public static ICommand GetScrollEndCommand(InputElement control)
        {
            return control.GetValue(ScrollEndCommandProperty);
        }
        public static void SetScrollEndCommand(InputElement control, ICommand value)
        {
            control.SetValue(ScrollEndCommandProperty, value);
        }

        public static readonly AttachedProperty<ICommand> PullEndCommandProperty = AvaloniaProperty.RegisterAttached<GesturesAttached, InputElement, ICommand>("PullEndCommand");
        public static ICommand GetPullEndCommand(InputElement input)
        {
            return input.GetValue(PullEndCommandProperty);
        }

        public static void SetPullEndCommand(InputElement input, ICommand value)
        {
            input.SetValue(PullEndCommandProperty, value);
        }

        private static void ScrollEndCommandPropertyChanged(AvaloniaPropertyChangedEventArgs<ICommand> arg)
        {
            var control = arg.Sender as InputElement;
            if (!arg.NewValue.HasValue)
            {
                control.RemoveHandler(Gestures.ScrollGestureEndedEvent, ScrollEndHandler);
            }
            else
            {
                control.AddHandler(Gestures.ScrollGestureEndedEvent, ScrollEndHandler);
            }

            static void ScrollEndHandler(object sender, ScrollGestureEndedEventArgs arg)
            {
                var command = GetScrollEndCommand(sender as InputElement);
                if (command == null) return;

                if (command.CanExecute(arg))
                {
                    command.Execute(arg);
                }
            }
        }

        private static void PullEndCommandPropertyChanged(AvaloniaPropertyChangedEventArgs<ICommand> arg)
        {
            var control = arg.Sender as InputElement;
            if (!arg.NewValue.HasValue)
            {
                control.RemoveHandler(Gestures.PullGestureEndedEvent, PullEndHandler);
            }
            else
            {
                control.AddHandler(Gestures.PullGestureEndedEvent, PullEndHandler);
            }

            static void PullEndHandler(object sender, PullGestureEndedEventArgs arg)
            {
                var command = GetPullEndCommand(sender as InputElement);
                if (command == null) return;

                if (command.CanExecute(arg))
                {
                    command.Execute(arg);
                }
            }
        }
    }
}
