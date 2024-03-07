using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Rendering.Composition.Animations;

namespace PolarShadow.Controls
{
    public partial class SwipeContainer
    {
        private Control _refreshIndicator;
        private Control _refreshIndicatorIcon;
        private CompositionVisual _refreshIndicatorVisual;
        private CompositionVisual _refreshIndicatorIconVisual;
        private ImplicitAnimationCollection _refreshImplictAnimations;
        private TaskCompletionSource _refreshTCS;

        private double _resetTranslateY = -25;
        private double _maxRefreshSwipeDistance;
        private double _minValidRefreshSwipeDistance;

        public static readonly StyledProperty<ICommand> RefreshCommandProperty = AvaloniaProperty.Register<SwipeToRefreshContainer, ICommand>(nameof(RefreshCommand));
        public ICommand RefreshCommand
        {
            get => GetValue(RefreshCommandProperty);
            set => SetValue(RefreshCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> RefreshCancelCommandProperty = AvaloniaProperty.Register<SwipeToRefreshContainer, ICommand>(nameof(RefreshCancelCommand));
        public ICommand RefreshCancelCommand
        {
            get => GetValue(RefreshCancelCommandProperty);
            set => SetValue(RefreshCancelCommandProperty, value);
        }

        public static readonly StyledProperty<bool> RefreshFinishedProperty = AvaloniaProperty.Register<SwipeToRefreshContainer, bool>(nameof(RefreshFinished));
        public bool RefreshFinished
        {
            get => GetValue(RefreshFinishedProperty);
            set => SetValue(RefreshFinishedProperty, value);
        }

        public void RequestRefresh()
        {
            RefreshCommand?.Execute(default);
        }

        public void RequestCancelRefresh()
        {
            RefreshCancelCommand?.Execute(default);
        }

        private void InitRefresh()
        {
            _resetTranslateY = _refreshIndicator.Margin.Top;
            _maxRefreshSwipeDistance = 100 - _resetTranslateY;
            _minValidRefreshSwipeDistance = _maxRefreshSwipeDistance * 0.66;
        }

        private void ResetRefreshVisual()
        {
            _refreshIndicatorVisual = _refreshIndicatorIconVisual = null;
        }

        private void RefreshFinishedPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var val = e.GetOldAndNewValue<bool>();
            if (val.newValue && _refreshTCS != null)
            {
                _refreshTCS.TrySetResult();
            }
        }

        private void TopToBottom(double delta_y)
        {
            if (_refreshIndicatorVisual == null || _refreshIndicatorIconVisual == null)
            {
                return;
            }

            if (_refreshIndicatorVisual.ImplicitAnimations != null)
            {
                _refreshIndicatorVisual.ImplicitAnimations = null;
            }

            delta_y = _refreshIndicatorVisual.Offset.Y - delta_y;
            if (delta_y < _resetTranslateY)
            {
                return;
            }

            var scale = Math.Min(Math.Abs(_resetTranslateY - delta_y) / _minValidRefreshSwipeDistance, 1);

            var rotate = -(delta_y / _maxRefreshSwipeDistance) * 270;

            if (delta_y > _maxRefreshSwipeDistance)
            {
                return;
            }

            if (delta_y < _minValidRefreshSwipeDistance)
            {
                PullState();
            }

            RefreshIndicatorTransform(Math.Round(delta_y, 2), Math.Round(scale, 2), Math.Round(rotate, 2));
        }

        private async void TopToBottomEnd()
        {
            var translateY = _refreshIndicatorVisual.Offset.Y;
            if (translateY > _minValidRefreshSwipeDistance)
            {
                if (_refreshTCS != null)
                {
                    return;
                }
                _refreshTCS = new TaskCompletionSource();
                RefreshFinished = false;
                try
                {
                    RefreshingState();
                    RequestRefresh();
                    await _refreshTCS.Task;
                }
                catch { }
                finally
                {
                    _refreshTCS = null;
                    PullState();
                }
            }
            else
            {
                _refreshTCS?.TrySetCanceled();
                RequestCancelRefresh();
            }

            ResetRefreshIndicator(true);
            NormalState();
        }

        private void ResetRefreshIndicator(bool animation = false)
        {
            _refreshIndicatorVisual ??= ElementComposition.GetElementVisual(_refreshIndicator);
            _refreshIndicatorIconVisual ??= ElementComposition.GetElementVisual(_refreshIndicatorIcon);
            if (animation)
            {
                ResetRefreshIndicatorWidthAnimation(_resetTranslateY, resetScale);
            }
            else
            {
                RefreshIndicatorTransform(_resetTranslateY, resetScale);
            }
        }

        private void RefreshIndicatorTransform(double translateY = 0, double scale = 1, double rotate = 0)
        {
            _refreshIndicatorVisual.CenterPoint = new Vector3D(_refreshIndicatorVisual.Size.X * 0.5, _refreshIndicatorVisual.Size.Y * 0.5, 1);
            _refreshIndicatorIconVisual.CenterPoint = new Vector3D(_refreshIndicatorIconVisual.Size.X * 0.5, _refreshIndicatorIconVisual.Size.Y * 0.5, 1);
            _refreshIndicatorVisual.Offset = new Vector3D(_refreshIndicatorVisual.Offset.X, translateY, 1);
            _refreshIndicatorVisual.Scale = new Vector3D(scale, scale, 0);
            if (!IsRefeshing())
            {
                _refreshIndicatorIconVisual.RotationAngle = (float)(rotate / 180f * 3.14f);
            }
        }


        private void ResetRefreshIndicatorWidthAnimation(double translateY = 0, double scale = 1)
        {
            if (_refreshImplictAnimations == null)
            {
                var duration = TimeSpan.FromSeconds(0.3);
                var compositor = _refreshIndicatorVisual.Compositor;

                var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
                scaleAnimation.Duration = duration;
                scaleAnimation.InsertExpressionKeyFrame(1, "this.FinalValue", resetEasing);
                scaleAnimation.Target = nameof(CompositionVisual.Scale);

                var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
                offsetAnimation.Duration = duration;
                offsetAnimation.InsertExpressionKeyFrame(1, "this.FinalValue", resetEasing);
                offsetAnimation.Target = nameof(CompositionVisual.Offset);

                _refreshImplictAnimations = compositor.CreateImplicitAnimationCollection();
                _refreshImplictAnimations[nameof(CompositionVisual.Offset)] = offsetAnimation;
                _refreshImplictAnimations[nameof(CompositionVisual.Scale)] = scaleAnimation;
            }

            if (_refreshIndicatorVisual.ImplicitAnimations == null)
            {
                _refreshIndicatorVisual.ImplicitAnimations = _refreshImplictAnimations;
            }

            _refreshIndicatorVisual.Offset = new Vector3D(_refreshIndicatorVisual.Offset.X, translateY, 1);
            _refreshIndicatorVisual.Scale = new Vector3D(scale, scale, 1);
        }
        private bool IsRefeshing() => PseudoClasses.Contains(":refreshing");
        private bool IsRefreshState() => PseudoClasses.Contains(":pull") || PseudoClasses.Contains(":refreshing");

        private void PullState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":pull", true);
            PseudoClasses.Set(":refreshing", false);
        }

        private void RefreshingState()
        {
            PseudoClasses.Set(":reset", false);
            PseudoClasses.Set(":normal", true);
            PseudoClasses.Set(":pull", false);
            PseudoClasses.Set(":refreshing", true);
        }
    }
}
