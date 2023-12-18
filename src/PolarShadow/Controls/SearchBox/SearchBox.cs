using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.LogicalTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public partial class SearchBox : TemplatedControl
    {
        private TextBox _searchTextBox;
        private TextBox SearchTextBox
        {
            get => _searchTextBox;
            set
            {
                if (_searchTextBox != null)
                {
                    _searchTextBox.KeyUp -= TextBox_KeyUp;
                }
                _searchTextBox = value;
                if (_searchTextBox != null)
                {
                    _searchTextBox.KeyUp += TextBox_KeyUp;
                }
            }
        }
        private Button _clearButton;
        private Button ClearButton
        {
            get => _clearButton;
            set
            {
                if (_clearButton != null)
                {
                    _clearButton.Click -= ClearButton_Click;
                }

                _clearButton = value;
                if (_clearButton != null)
                {
                    _clearButton.Click += ClearButton_Click;
                }
            }
        }

        public static readonly StyledProperty<ICommand> SearchCommandProperty = AvaloniaProperty.Register<SearchBox, ICommand>(nameof(SearchCommand));
        public ICommand SearchCommand
        {
            get => GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public static readonly StyledProperty<string> SearchTextProperty = AvaloniaProperty.Register<SearchBox, string>(nameof(SearchText), string.Empty);
        public string SearchText
        {
            get => GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            SearchTextBox = e.NameScope.Find<TextBox>("Part_TextBox");
            ClearButton = e.NameScope.Find<Button>("Part_ClearButton");
        }

        private void ClearButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (SearchTextBox != null)
            {
                SearchTextBox.Text = string.Empty;
            }
        }

        private void TextBox_KeyUp(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteCommand();
            }
        }

        private void ExecuteCommand()
        {
            if (SearchCommand == null || !SearchCommand.CanExecute(SearchText)) return;
            SearchCommand.Execute(SearchText);
        }
    }
}
