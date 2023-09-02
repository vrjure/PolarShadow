using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Animations
{
    internal class ContentSlideOutFadeIn : IPageTransition
    {
        public double TranslateX { get; set; } = 0d;
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.3);

        private readonly ContentControl _parent;
        public ContentSlideOutFadeIn(ContentControl parent)
        {
            _parent = parent;
        }

        public async Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var translateX = TranslateTransform.XProperty;
            if (from != null)
            {
                var animation = new Animation()
                {
                    FillMode = FillMode.Forward,
                    Children =
                    {
                        new KeyFrame
                        {
                            Setters =
                            {
                                new Setter { Property = translateX, Value = 0d }
                            },
                            Cue=new Cue(0d)
                        },
                        new KeyFrame
                        {
                            Setters = {new Setter { Property = translateX, Value = TranslateX } },
                            Cue = new Cue(1d)
                        }
                    },
                    Duration = Duration
                };
                await animation.RunAsync(from, cancellationToken);
            }

            if (to != null)
            {
                to.Opacity = 0;
                var animation = new Animation
                {
                    FillMode = FillMode.Forward,
                    Children =
                    {
                        new KeyFrame
                        {
                            Setters =
                            {
                                new Setter { Property = Visual.OpacityProperty, Value = 0d }
                            },
                            Cue = new Cue(0d),
                        },
                        new KeyFrame
                        {
                            Setters =
                            {
                                new Setter { Property = Visual.OpacityProperty, Value = 1d }
                            },
                            Cue = new Cue(1d)
                        }
                    },
                    Duration = this.Duration
                };

                _parent.Content = to;
                await animation.RunAsync(to, cancellationToken);
            }
            else
            {
                _parent.Content = to;
            }
        }

        /// <summary>
        /// 获取两个控件的共同视觉父级。
        /// </summary>
        /// <param name="from">源控件。</param>
        /// <param name="to">目标控件。</param>
        /// <returns>共同的父级。</returns>
        /// <exception cref="ArgumentException">
        /// 两个控件没有共同的父级。
        /// </exception>
        /// <remarks>
        /// 任何一个参数可能为null，但不能都为null。
        /// </remarks>
        private static Visual GetVisualParent(Visual from, Visual to)
        {
            var p1 = (from ?? to)!.GetVisualParent();
            var p2 = (to ?? from)!.GetVisualParent();

            if (p1 != null && p2 != null && p1 != p2)
            {
                throw new ArgumentException(
                                    "Controls for PageSlide must have same parent.");
            }

            return p1 ?? throw new InvalidOperationException(
                                                    "Cannot determine visual parent.");
        }
    }
}
