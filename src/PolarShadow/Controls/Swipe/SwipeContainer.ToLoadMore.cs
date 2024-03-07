using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    public partial class SwipeContainer
    {
        private static readonly double _minValidLoadMoreSwipeDistance = 150;

        private TaskCompletionSource _loadMoreTCS;

        public static readonly StyledProperty<bool> CanLoadMoreProperty = AvaloniaProperty.Register<SwipeContainer, bool>(nameof(CanLoadMore));
        public bool CanLoadMore
        {
            get => GetValue(CanLoadMoreProperty);
            set => SetValue(CanLoadMoreProperty, value);
        }

        public static readonly StyledProperty<bool> LoadMoreFinishedProperty = AvaloniaProperty.Register<SwipeContainer, bool>(nameof(LoadMoreFinished));
        public bool LoadMoreFinished
        {
            get => GetValue(LoadMoreFinishedProperty);
            set => SetValue(LoadMoreFinishedProperty, value);
        }

        public static readonly StyledProperty<ICommand> LoadMoreCommandProperty = AvaloniaProperty.Register<SwipeContainer, ICommand>(nameof(LoadMoreCommand));
        public ICommand LoadMoreCommand
        {
            get => GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        public void RequestLoadMore()
        {
            LoadMoreCommand?.Execute(default);
        }

        private void LoadMoreFinishedPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var val = e.GetOldAndNewValue<bool>();
            if (val.newValue && _loadMoreTCS != null)
            {
                _loadMoreTCS.TrySetResult();
            }
        }

        private void CanLoadMorePropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var val = e.GetOldAndNewValue<bool>();
            if (val.newValue)
            {
                LoadMoreState();
            }
            else
            {
                NoMoreState();
            }
        }

        private async void BottomToTopEnd(double y)
        {
            if (!CanLoadMore || Math.Abs(y) < _minValidLoadMoreSwipeDistance)
            {
                return;
            }

            if (_loadMoreTCS != null)
            {
                return;
            }

            LoadMoreFinished = false;
            _loadMoreTCS = new TaskCompletionSource();
            try
            {
                LoadingState();
                RequestLoadMore();
                await _loadMoreTCS.Task;
            }
            catch { }
            finally
            {
                _loadMoreTCS = null;
                LoadMoreState();
            }
        }

        private void LoadMoreState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":loadMore", true);
            PseudoClasses.Set(":loading", false);
            PseudoClasses.Set(":noMore", false);
        }

        private void LoadingState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":loadMore", false);
            PseudoClasses.Set(":loading", true);
            PseudoClasses.Set(":noMore", false);
        }

        private void NoMoreState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":loadMore", false);
            PseudoClasses.Set(":loading", false);
            PseudoClasses.Set(":noMore", true);
        }
    }
}
