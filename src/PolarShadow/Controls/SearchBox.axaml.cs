using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<ICommand> SearchCommandProperty = AvaloniaProperty.Register<SearchBox, ICommand>(nameof(SearchCommand));
        public ICommand SearchCommand
        {
            get => GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> ClearCommandProperty = AvaloniaProperty.Register<SearchBox, ICommand>(nameof(ClearCommand));
        public ICommand ClearCommand
        {
            get => GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        public static readonly StyledProperty<string> SearchTextProperty = AvaloniaProperty.Register<SearchBox, string>(nameof(SearchText), string.Empty);
        public string SearchText
        {
            get => GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        private void KeyInput(object sender, KeyEventArgs arg)
        {
            if(arg.Key == Key.Enter)
            {
                ExecuteCommand();
            }
        }

        private void ClearClick(object sender, RoutedEventArgs arg)
        {
            SearchText = string.Empty;
        }

        private void ExecuteCommand()
        {
            if (SearchCommand == null) return;
            if (!SearchCommand.CanExecute(SearchText)) return;
            SearchCommand.Execute(SearchText);
        }
    }
}
