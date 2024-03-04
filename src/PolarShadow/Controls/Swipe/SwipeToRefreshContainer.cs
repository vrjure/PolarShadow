using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Diagnostics;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.Transformation;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Styling;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolarShadow.Controls
{
    [TemplatePart("PART_Indicator", typeof(Control))]
    [TemplatePart("PART_IndicatorIcon", typeof(Control))]
    public class SwipeToRefreshContainer : ContentControl
    {
        private static readonly double resetScale = 0.2;
        private static readonly LinearEasing resetEasing = new LinearEasing();

        private double _resetTranslateY = -25;
        private double _maxTranslateY;
        private double _minValidTranslateY;
        private Control _indicator;
        private Control _indicatorIcon;
        private CompositionVisual _indicatorVisual;
        private CompositionVisual _indicatorIconVisual;
        private ImplicitAnimationCollection _implictAnimations;
        private TaskCompletionSource _refreshTCS;

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

        static SwipeToRefreshContainer()
        {
            Gestures.ScrollGestureEvent.AddClassHandler<SwipeToRefreshContainer>((s, e) => s.ScrollGestureEvent(e));
            Gestures.ScrollGestureEndedEvent.AddClassHandler<SwipeToRefreshContainer>((s, e) => s.ScrollGestureEndEvent(e));

            RefreshFinishedProperty.Changed.AddClassHandler<SwipeToRefreshContainer>((s, e) => s.RefreshFinishedChanged(e));
        }

        public SwipeToRefreshContainer()
        {
            GestureRecognizers.Add(new ScrollGestureRecognizer { CanHorizontallyScroll = false, CanVerticallyScroll = true });
        }

        public void RequestRefresh()
        {
            RefreshCommand?.Execute(default);
        }

        public void RequestCancelRefresh()
        {
            RefreshCancelCommand?.Execute(default);
        }

        private void RefreshFinishedChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var val = e.GetOldAndNewValue<bool>();
            if (val.newValue &&_refreshTCS != null)
            {
                _refreshTCS.TrySetResult();
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _indicator = e.NameScope.Find<Control>("PART_Indicator");
            _indicatorIcon = e.NameScope.Find<Control>("PART_IndicatorIcon");

            _resetTranslateY = _indicator.Margin.Top;
            _maxTranslateY = 100 - _resetTranslateY;
            _minValidTranslateY = _maxTranslateY * 0.66;
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            ResetIndicator();
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            _indicatorVisual = _indicatorIconVisual = null;
        }

        private void ScrollGestureEvent(ScrollGestureEventArgs e)
        {
            if (_indicatorVisual == null || _indicatorIconVisual == null)
            {
                return;
            }

            if (!_indicator.IsVisible)
            {
                _indicator.IsVisible = true;
            }

            if (_indicatorVisual.ImplicitAnimations != null)
            {
                _indicatorVisual.ImplicitAnimations = null;
            }

            var translateY = _indicatorVisual.Offset.Y - e.Delta.Y;

            if (translateY < _resetTranslateY)
            {
                return;
            }

            var scale = Math.Min(Math.Abs(_resetTranslateY - translateY) / _minValidTranslateY, 1);

            var rotate = -(translateY / _maxTranslateY) * 270;

            if (translateY > _maxTranslateY)
            {
                return;
            }

            if (translateY <= _minValidTranslateY)
            {
                IndicatorSpin(false);
            }

            IndicatorTransform(Math.Round(translateY, 2), Math.Round(scale, 2), Math.Round(rotate,2));
        }

        private async void ScrollGestureEndEvent(ScrollGestureEndedEventArgs e)
        {
            var translateY = _indicatorVisual.Offset.Y;
            if (translateY > _minValidTranslateY)
            {
                if (_refreshTCS != null)
                {
                    return;
                }
                _refreshTCS = new TaskCompletionSource();
                RefreshFinished = false;
                try
                {
                    IndicatorSpin(true);
                    RequestRefresh();
                    await _refreshTCS.Task;
                }
                catch { }
                finally
                {
                    _refreshTCS = null;
                    IndicatorSpin(false);
                }
            }
            else
            {
                _refreshTCS?.TrySetCanceled();
                RequestCancelRefresh();
            }

            ResetIndicator(true);
        }

        private void ResetIndicator(bool animation = false)
        {
            _indicatorVisual ??= ElementComposition.GetElementVisual(_indicator);
            _indicatorIconVisual ??= ElementComposition.GetElementVisual(_indicatorIcon);
            if (animation)
            {
                ResetIndicatorWidthAnimation(_resetTranslateY, resetScale);
            }
            else
            {
                IndicatorTransform(_resetTranslateY, resetScale);
            }
        }

        private void IndicatorTransform(double translateY = 0, double scale = 1, double rotate = 0)
        {
            _indicatorVisual.CenterPoint = new Vector3D(_indicatorVisual.Size.X * 0.5, _indicatorVisual.Size.Y * 0.5, 1);
            _indicatorIconVisual.CenterPoint = new Vector3D(_indicatorIconVisual.Size.X * 0.5, _indicatorIconVisual.Size.Y * 0.5, 1);
            _indicatorVisual.Offset = new Vector3D(_indicatorVisual.Offset.X, translateY, 1);
            _indicatorVisual.Scale = new Vector3D(scale, scale, 0);
            if (!IsIndicatorSpin())
            {
                _indicatorIconVisual.RotationAngle = (float)(rotate / 180f * 3.14f);
            }
        }

        private void IndicatorSpin(bool active)
        {
            PseudoClasses.Set(":spin", active);
        }

        private bool IsIndicatorSpin() => PseudoClasses.Contains(":spin");

        private void ResetIndicatorWidthAnimation(double translateY = 0, double scale = 1)
        {
            if (_implictAnimations == null)
            {
                var duration = TimeSpan.FromSeconds(0.3);
                var compositor = _indicatorVisual.Compositor;

                var scaleAnimation = compositor.CreateVector3DKeyFrameAnimation();
                scaleAnimation.Duration = duration;
                scaleAnimation.InsertExpressionKeyFrame(1, "this.FinalValue", resetEasing);
                scaleAnimation.Target = nameof(CompositionVisual.Scale);

                var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
                offsetAnimation.Duration = duration;
                offsetAnimation.InsertExpressionKeyFrame(1, "this.FinalValue", resetEasing);
                offsetAnimation.Target = nameof(CompositionVisual.Offset);

                _implictAnimations = compositor.CreateImplicitAnimationCollection();
                _implictAnimations[nameof(CompositionVisual.Offset)] = offsetAnimation;
                _implictAnimations[nameof(CompositionVisual.Scale)] = scaleAnimation;
            }

            if (_indicatorVisual.ImplicitAnimations == null)
            {
                _indicatorVisual.ImplicitAnimations = _implictAnimations;
            }

            _indicatorVisual.Offset = new Vector3D(_indicatorVisual.Offset.X, _resetTranslateY, 1);
            _indicatorVisual.Scale = new Vector3D(scale, scale, 1);
        }
    }
}
