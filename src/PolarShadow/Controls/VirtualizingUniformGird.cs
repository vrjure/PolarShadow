using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow.Controls
{
    /// <summary>
    /// TODO
    /// </summary>
    public class VirtualizingUniformGird : VirtualizingPanel
    {
        public static readonly StyledProperty<int> RowsProperty =
            AvaloniaProperty.Register<VirtualizingUniformGird, int>(nameof(Rows));

        public static readonly StyledProperty<int> ColumnsProperty =
            AvaloniaProperty.Register<VirtualizingUniformGird, int>(nameof(Columns));

        public static readonly StyledProperty<int> FirstColumnProperty =
            AvaloniaProperty.Register<VirtualizingUniformGird, int>(nameof(FirstColumn));

        public int Rows
        {
            get => GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public int Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public int FirstColumn
        {
            get => GetValue(FirstColumnProperty);
            set => SetValue(FirstColumnProperty, value);
        }


        protected override Control ContainerFromIndex(int index)
        {
            throw new NotImplementedException();
        }

        protected override IInputElement GetControl(NavigationDirection direction, IInputElement from, bool wrap)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<Control> GetRealizedContainers()
        {
            throw new NotImplementedException();
        }

        protected override int IndexFromContainer(Control container)
        {
            throw new NotImplementedException();
        }

        protected override Control ScrollIntoView(int index)
        {
            throw new NotImplementedException();
        }
    }
}
