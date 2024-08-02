using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls.Windows
{
    internal class ForegroundWindow : Window
    {
        private Control _background;
        private Window _parentWindow;
        private static Point _zeroPoint = new Point(0, 0);

        public ForegroundWindow(Control background)
        {
            Title = "Avalonia.Foreground";
            Height = 300;
            Width = 300;
            SystemDecorations = SystemDecorations.None;
            TransparencyLevelHint = new WindowTransparencyLevel[] { WindowTransparencyLevel.Transparent };
            Background = Brushes.Transparent;
            SizeToContent = SizeToContent.Manual;
            CanResize = false;
            ShowInTaskbar = false;
            Opacity = 1;
            ZIndex = int.MaxValue;
            DataContext = background.DataContext;

            _background = background;
            _background.DataContextChanged += Background_DataContextChanged;
            _background.Loaded += Background_Loaded;
            _background.Unloaded += Background_Unloaded;
        }
        private void Background_DataContextChanged(object sender, EventArgs e)
        {
            DataContext = _background.DataContext;
        }
        private void Background_Loaded(object sender, Interactivity.RoutedEventArgs e)
        {
            if (_parentWindow != null && IsVisible)
            {
                return;
            }

            _parentWindow = GetTopLevel(_background) as Window;
            if (_parentWindow == null)
            {
                return;
            }

            Owner= _parentWindow;

            _parentWindow.Closing += ParentWindow_Closing;
            _parentWindow.PositionChanged += ParentWindow_PositionChanged;
            _background.LayoutUpdated += BackgroundWindow_LayoutUpdated;
            _background.SizeChanged += BackgroundWindow_SizeChanged;

            try
            {
                UpdateOverlayPosition();
                Show(_parentWindow);
                _parentWindow.Focus();
            }
            catch (Exception)
            {
                Hide();
                throw;
            }
        }

        private void Background_Unloaded(object sender, Interactivity.RoutedEventArgs e)
        {
            _background.LayoutUpdated += BackgroundWindow_LayoutUpdated;
            _background.SizeChanged += BackgroundWindow_SizeChanged;
            if (_parentWindow != null)
            {
                _parentWindow.Closing -= ParentWindow_Closing;
                _parentWindow.PositionChanged -= ParentWindow_PositionChanged;
            }
            Hide();
        }

        private void ParentWindow_Closing(object sender, WindowClosingEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            Close();
            _background.DataContextChanged -= Background_DataContextChanged;
            _background.Loaded -= Background_Loaded;
            _background.Unloaded -= Background_Unloaded;
        }

        private void BackgroundWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOverlayPosition();
        }

        private void ParentWindow_PositionChanged(object sender, PixelPointEventArgs e)
        {
            UpdateOverlayPosition();
        }

        private void BackgroundWindow_LayoutUpdated(object sender, EventArgs e)
        {
            UpdateOverlayPosition();
        }

        private void UpdateOverlayPosition()
        {
            if (_background == null)
            {
                return;
            }

            var startLoc = _background.PointToScreen(_zeroPoint);
            var width = _background.Bounds.Width;
            var height = _background.Bounds.Height;
            if (Math.Abs(startLoc.X - Position.X) + Math.Abs(startLoc.Y - Position.Y) > 0.5)
            {
                Position = startLoc;
            }
            if (Math.Abs(Bounds.Width - width) + Math.Abs(Height - height) > 0.5)
            {
                Width = width;
                Height = height;
            }
            
        }
    }
}
