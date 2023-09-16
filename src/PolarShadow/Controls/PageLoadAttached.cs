using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    public class PageLoadAttached
    {
        static PageLoadAttached()
        {
            RegisterLoadProperty.Changed.Subscribe(RegisterLoadPropertyChanged);
        }

        public static readonly AttachedProperty<bool> RegisterLoadProperty = AvaloniaProperty.RegisterAttached<PageLoadAttached, ContentControl, bool>("RegisterLoad");
        public static bool GetRegisterLoad(ContentControl control)
        {
            return control.GetValue(RegisterLoadProperty);
        }

        public static void SetRegisterLoad(ContentControl control, bool value)
        {
            control.SetValue(RegisterLoadProperty, value);
        }

        private static void RegisterLoadPropertyChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            if (Design.IsDesignMode) return;

            var control = args.Sender as ContentControl;

            if (args.NewValue.HasValue)
            {
                if (args.NewValue.Value)
                {
                    control.AddHandler(ContentControl.LoadedEvent, ControlLoaded);
                    control.AddHandler(ContentControl.UnloadedEvent, ControlUnloaded);
                }
                else
                {
                    control.RemoveHandler(ContentControl.LoadedEvent, ControlLoaded);
                    control.RemoveHandler(ContentControl.UnloadedEvent, ControlUnloaded);
                }
            }

            static void ControlLoaded(object sender, RoutedEventArgs args)
            {
                var ctrl = sender as ContentControl;
                if (ctrl.DataContext is ObservableRecipient or)
                {
                    or.IsActive = true;
                }
            }

            static void ControlUnloaded(object sender, RoutedEventArgs args)
            {
                var ctrl = sender as ContentControl;
                if (ctrl.DataContext is ObservableRecipient or)
                {
                    or.IsActive = false;
                }
            }
        }
    }
}
