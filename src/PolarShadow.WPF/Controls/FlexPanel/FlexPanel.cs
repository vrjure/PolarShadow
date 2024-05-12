using Microsoft.Extensions.DependencyModel;
using PolarShadows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolarShadow.Controls
{
    class FlexPanel : Panel
    {
        private bool _isHorizontal => FlexDirection == FlexDirection.Row || FlexDirection == FlexDirection.RowReverse;
        private bool _isReverse => FlexDirection == FlexDirection.RowReverse || FlexDirection == FlexDirection.ColumnReverse;
        private int _lineCount = 0;
        private double _needTotalV = 0;


        public static readonly DependencyProperty FlexDirectionProperty = DP.Register<FlexPanel, FlexDirection>(nameof(FlexDirection), FlexDirection.Row, metadataCreator: () => new FrameworkPropertyMetadata { AffectsMeasure = true, AffectsArrange = true });
        public FlexDirection FlexDirection
        {
            get => (FlexDirection)GetValue(FlexDirectionProperty);
            set => SetValue(FlexDirectionProperty, value);
        }

        public static readonly DependencyProperty FlexWrapProperty = DP.Register<FlexPanel, FlexWrap>(nameof(FlexWrap), FlexWrap.Wrap, metadataCreator: () => new FrameworkPropertyMetadata { AffectsMeasure = true, AffectsArrange = true });
        public FlexWrap FlexWrap
        {
            get => (FlexWrap)GetValue(FlexWrapProperty);
            set => SetValue(FlexWrapProperty, value);
        }

        public static readonly DependencyProperty JustifyContentProperty = DP.Register<FlexPanel, FlexAlignContent>(nameof(JustifyContent), FlexAlignContent.Start, metadataCreator: () => new FrameworkPropertyMetadata { AffectsMeasure = true, AffectsArrange = true });
        public FlexAlignContent JustifyContent
        {
            get => (FlexAlignContent)GetValue(JustifyContentProperty);
            set => SetValue(JustifyContentProperty, value);
        }

        public static readonly DependencyProperty JustifyItemsProperty = DP.Register<FlexPanel, FlexAlign>(nameof(JustifyItems), FlexAlign.Center, metadataCreator: () => new FrameworkPropertyMetadata { AffectsArrange = true});
        public FlexAlign JustifyItems
        {
            get => (FlexAlign)GetValue(JustifyItemsProperty);
            set => SetValue(JustifyItemsProperty, value);
        }

        public static readonly DependencyProperty JustifySelfProperty = DP.RegisterAttached<FlexPanel, FlexAlign>("JustifySelf", FlexAlign.Auto, metadataCreator: () => new FrameworkPropertyMetadata { AffectsParentArrange = true });
        public static FlexAlign GetJustifySelf(UIElement element)
        {
            return (FlexAlign)element.GetValue(JustifySelfProperty);
        }
        public static void SetJustifySelf(UIElement element, FlexAlign value)
        {
            element.SetValue(JustifySelfProperty, value);
        }

        public static readonly DependencyProperty AlignContentProperty = DP.RegisterAttached<FlexPanel, FlexAlignContent>("AlignContent", FlexAlignContent.Start, metadataCreator: () => new FrameworkPropertyMetadata { AffectsArrange = true });
        public FlexAlignContent AlignContent
        {
            get => (FlexAlignContent)GetValue(AlignContentProperty);
            set => SetValue(AlignContentProperty, value);
        }

        public static readonly DependencyProperty AlignItemsProperty = DP.Register<FlexPanel, FlexAlign>(nameof(FlexAlign), FlexAlign.Stretch, metadataCreator: () => new FrameworkPropertyMetadata { AffectsArrange = true });
        public FlexAlign AlignItems
        {
            get => (FlexAlign)GetValue(AlignItemsProperty);
            set => SetValue(AlignItemsProperty, value);
        }

        public static readonly DependencyProperty AlignSelfProperty = DP.RegisterAttached<FlexPanel, FlexAlign>("AlignSelf", FlexAlign.Auto, metadataCreator: () => new FrameworkPropertyMetadata { AffectsArrange = true });
        public FlexAlign GetAlignSelf(UIElement element)
        {
            return (FlexAlign)element.GetValue(AlignSelfProperty);
        }
        public void SetAlignSelf(UIElement element, FlexAlign value)
        {
            element.SetValue(AlignSelfProperty, value);
        }

        public static readonly DependencyProperty FlexBasisProperty = DP.RegisterAttached<FlexPanel, FlexBasis>("FlexBasis", FlexBasis.Auto, metadataCreator: () => new FrameworkPropertyMetadata { AffectsParentMeasure = true, AffectsParentArrange = true });
        public static FlexBasis GetFlexBasis(UIElement element)
        {
            return (FlexBasis)element.GetValue(FlexBasisProperty);
        }
        public static void SetFlexBasis(UIElement element, FlexBasis value)
        {
            element.SetValue(FlexBasisProperty, value);
        }


        public static readonly DependencyProperty FlexGrowProperty = DP.RegisterAttached<FlexPanel, double>("FlexGrow", 0, metadataCreator: () => new FrameworkPropertyMetadata { AffectsParentArrange = true });
        public static double GetFlexGrow(UIElement element)
        {
            return (double)element.GetValue(FlexGrowProperty);
        }
        public static void SetFlexGrow(UIElement element, double value)
        {
            element.SetValue(FlexGrowProperty, value);
        }

        public static readonly DependencyProperty FlexShrinkProperty = DP.RegisterAttached<FlexPanel, double>("FlexShrink", 1, metadataCreator: () => new FrameworkPropertyMetadata { AffectsParentArrange = true });
        public static double GetFlexShrink(UIElement element)
        {
            return (double)element.GetValue(FlexShrinkProperty);
        }
        public static void SetFlexShrink(UIElement element, double value)
        {
            element.SetValue(FlexShrinkProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var panelSize = new UVSize(FlexDirection);
            var constraintSize = new UVSize(FlexDirection, availableSize.Width, availableSize.Height);
            var curLineSize = new UVSize(FlexDirection);
            var shouldWrap = FlexWrap != FlexWrap.NoWrap;

            var children = InternalChildren;
            if (children.Count > 0)
            {
                _lineCount = 1;
            }
            else
            {
                _lineCount = 0;
            }
            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child == null) continue;
                child.Measure(availableSize);

                var sz = GetItemBasisUVSize(child, constraintSize.U);

                if ((curLineSize.U + sz.U) > constraintSize.U)
                {
                    if (shouldWrap)
                    {
                        panelSize.U = Math.Max(panelSize.U, curLineSize.U);
                        panelSize.V += curLineSize.V;
                        curLineSize = sz;
                        _lineCount++;
                    }
                    else
                    {
                        curLineSize.U += sz.U;
                        curLineSize.V = Math.Max(curLineSize.V, sz.V);
                    }
                }
                else
                {
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                    curLineSize.U += sz.U;
                }
            }

            panelSize.U = Math.Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;
            _needTotalV = panelSize.V;

            return new Size(panelSize.Width, panelSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var uvFinal = new UVSize(FlexDirection, finalSize.Width, finalSize.Height);
            var curLineSize = new UVSize(FlexDirection);
            var shouldWrap = FlexWrap != FlexWrap.NoWrap;
            var wrapReverse = FlexWrap == FlexWrap.WrapReverse;
            var curLineStartIndex = 0;
            var totalGrow = 0d;
            var totalShrink = 0d;
            var alignContent = AlignContent;
            var v = 0d;
            var appendV = 0d;
            var remainV = uvFinal.V - _needTotalV;
            switch (alignContent)
            {
                case FlexAlignContent.End:
                    v = remainV;
                    break;
                case FlexAlignContent.Center:
                    v = remainV / 2;
                    break;
                case FlexAlignContent.SpaceAround:
                    v = remainV / (_lineCount * 2);
                    break;
                case FlexAlignContent.SpaceEvenly:
                    v = remainV / (_lineCount + 1);
                    break;
                case FlexAlignContent.Stretch:
                    appendV = remainV / _lineCount;
                    break;
            }

            var children = InternalChildren;
            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child == null) continue;

                var grow = GetFlexGrow(child);
                var shrink = GetFlexShrink(child);

                var sz = GetItemBasisUVSize(child, uvFinal.U);
                if(alignContent == FlexAlignContent.Stretch)
                {
                    sz.V += appendV;
                }

                if ((curLineSize.U + sz.U) > uvFinal.U)
                {
                    if (shouldWrap)
                    {
                        if (wrapReverse)
                        {
                            ArrangeLine(curLineStartIndex, i, curLineSize, uvFinal, uvFinal.V - v - curLineSize.V, totalGrow, totalShrink);

                        }
                        else
                        {
                            ArrangeLine(curLineStartIndex, i, curLineSize, uvFinal, v, totalGrow, totalShrink);
                        }
                        curLineStartIndex = i;

                        switch (alignContent)
                        {
                            case FlexAlignContent.SpaceBetween:
                                if (_lineCount > 1)
                                {
                                    v += curLineSize.V + remainV / (_lineCount - 1);
                                }
                                break;
                            case FlexAlignContent.SpaceAround:
                                v += curLineSize.V + remainV / (_lineCount * 2) * 2;
                                break;
                            case FlexAlignContent.SpaceEvenly:
                                v += curLineSize.V + remainV / (_lineCount + 1);
                                break;
                            default:
                                v += curLineSize.V;
                                break;
                        }

                        curLineSize = sz;
                        totalGrow = totalShrink = 0;
                    }
                    else
                    {
                        curLineSize.U += sz.U;
                        curLineSize.V = Math.Max(curLineSize.V, sz.V);
                    }
                }
                else
                {
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                    curLineSize.U += sz.U;
                }

                totalGrow += grow >= 0 ? grow : 0;
                totalShrink += shrink >= 0 ? shrink : 1;
            }

            if (curLineStartIndex < children.Count)
            {
                if (wrapReverse)
                {
                    ArrangeLine(curLineStartIndex, children.Count, curLineSize, uvFinal, uvFinal.V - v - curLineSize.V, totalGrow, totalShrink);
                }
                else
                {
                    ArrangeLine(curLineStartIndex, children.Count, curLineSize, uvFinal, v, totalGrow, totalShrink);
                }
            }

            return finalSize;
        }

       

        private void ArrangeLine(int startIndex, int endIndex, UVSize lineSize, UVSize totalSize, double v, double totalGrow, double totalShrink)
        {
            var children = InternalChildren;
            var alignItems = AlignItems;
            var justifyItems = JustifyItems;
            var isHorizontal = _isHorizontal;
            var isReverse = _isReverse;
            var justifyContent = JustifyContent;
            var u = 0d;
            var accumulateU = 0d;
            var count = endIndex - startIndex;
            var remainU = totalSize.U - lineSize.U;
            var appenedU = 0d;
            if (justifyContent == FlexAlignContent.Stretch)
            {
                appenedU = remainU / count;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                var child = children[i];
                if (child == null) continue;

                var sz = GetFinalItemUVSize(child, remainU, totalSize.U, totalGrow, totalShrink);
                if (justifyContent == FlexAlignContent.Stretch)
                {
                    sz.U += appenedU;
                }

                var finalRemainU = remainU;
                if (totalGrow > 0 || justifyContent == FlexAlignContent.Stretch)
                {
                    finalRemainU = 0;
                }

                var align = GetAlignSelf(child);
                if (align == FlexAlign.Auto)
                {
                    align = alignItems;
                }

                var justifyItem = GetJustifySelf(child);
                if (justifyItem == FlexAlign.Auto)
                {
                    justifyItem = justifyItems;
                }

                SetMainAxisItemAlign(child as FrameworkElement, justifyItem, isHorizontal);
                SetCrossAxisItemAlign(child as FrameworkElement, align, isHorizontal);

                switch (justifyContent)
                {
                    case FlexAlignContent.End:
                        u = finalRemainU + accumulateU;
                        break;
                    case FlexAlignContent.Center:
                        u = finalRemainU / 2 + accumulateU;
                        break;
                    case FlexAlignContent.SpaceAround:
                        u = finalRemainU / (count * 2) + accumulateU;
                        break;
                    case FlexAlignContent.SpaceEvenly:
                        u = finalRemainU / (count + 1) + accumulateU;
                        break;
                    default:
                        u = accumulateU;
                        break;
                }

                if (isReverse)
                    u = totalSize.U - sz.U - u;

                Arrange(child, u, v, sz, lineSize, isHorizontal);

                switch (justifyContent)
                {
                    case FlexAlignContent.SpaceBetween:
                        if (count > 1)
                        {
                            accumulateU += sz.U + finalRemainU / (count - 1);
                        }
                        break;
                    case FlexAlignContent.SpaceAround:
                        accumulateU += sz.U + finalRemainU / (count * 2) * 2;
                        break;
                    case FlexAlignContent.SpaceEvenly:
                        accumulateU += sz.U + finalRemainU / (count + 1);
                        break;
                    default:
                        accumulateU += sz.U;
                        break;
                }
            }
        }

        private void Arrange(UIElement element, double u, double v, UVSize size, UVSize lineSize, bool isHorizontal)
        {        
            element.Arrange(new Rect(isHorizontal ? u : v, isHorizontal ? v : u, isHorizontal ? size.U : lineSize.V, isHorizontal ? lineSize.V : size.U));
        }

        private UVSize GetItemBasisUVSize(UIElement element, double totalU)
        {
            var basis = GetFlexBasis(element) ?? FlexBasis.Auto;
            var sz = new UVSize(FlexDirection, element.DesiredSize.Width, element.DesiredSize.Height);
            if (basis.IsNumber)
            {
                sz.U = basis.GetDouble();
            }
            else if (basis.IsPercent)
            {
                sz.U = basis.GetDouble() * totalU;
            }

            return sz;
        }

        private UVSize GetFinalItemUVSize(UIElement element, double remainU, double totalU, double totalGrow, double totalShrink)
        {
            var sz = GetItemBasisUVSize(element, totalU);

            if (remainU > 0 && totalGrow > 0)
            {
                var grow = GetFlexGrow(element);
                sz.U += grow / totalGrow * remainU;
            }
            else if (remainU < 0 && totalShrink > 0)
            {
                var shrink = GetFlexShrink(element);
                sz.U = Math.Max(sz.U, sz.U - shrink / totalShrink * remainU);
            }
            return sz;
        }

        private void SetCrossAxisItemAlign(FrameworkElement element, FlexAlign align, bool isHorizontal)
        {
            if (element == null) return;
            var alignH = HorizontalAlignment.Stretch;
            var alignV = VerticalAlignment.Stretch;
            switch (align)
            {
                case FlexAlign.Center:
                    if (isHorizontal)
                        alignV = VerticalAlignment.Center;
                    else
                        alignH = HorizontalAlignment.Center;
                    break;
                case FlexAlign.Start:
                    if (isHorizontal)
                        alignV = VerticalAlignment.Top;
                    else
                        alignH = HorizontalAlignment.Left;
                    break;
                case FlexAlign.End:
                    if (isHorizontal)
                        alignV = VerticalAlignment.Bottom;
                    else
                        alignH = HorizontalAlignment.Right;
                    break;
            }

            if (isHorizontal)
                element.VerticalAlignment = alignV;
            else
                element.HorizontalAlignment = alignH;
        }


        private void SetMainAxisItemAlign(FrameworkElement element, FlexAlign justify, bool isHorizontal)
        {
            if (element == null) return;
            switch (justify)
            {
                case FlexAlign.Start:
                    if (isHorizontal)
                        element.HorizontalAlignment = HorizontalAlignment.Left;
                    else
                        element.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case FlexAlign.End:
                    if (isHorizontal)
                        element.HorizontalAlignment = HorizontalAlignment.Right;
                    else
                        element.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case FlexAlign.Center:
                    if (isHorizontal)
                        element.HorizontalAlignment = HorizontalAlignment.Center;
                    else
                        element.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case FlexAlign.Stretch:
                    if (isHorizontal)
                        element.HorizontalAlignment = HorizontalAlignment.Stretch;
                    else
                        element.VerticalAlignment = VerticalAlignment.Stretch;
                    break;
                default:
                    break;
            }
        }

        private struct UVSize
        {
            private bool _isHorizontal;
            public UVSize(FlexDirection direction) : this(direction, 0, 0)
            {

            }
            public UVSize(FlexDirection direction, double width, double height)
            {
                _isHorizontal = direction == FlexDirection.Row || direction == FlexDirection.RowReverse;
                U = V = 0d;
                Width = width;
                Height = height;
            }

            public double U;
            public double V;

            public double Width
            {
                get => _isHorizontal ? U : V;
                set { if (_isHorizontal) U = value; else V = value; }
            }

            public double Height
            {
                get => _isHorizontal ? V:U;
                set { if (_isHorizontal) V = value; else U = value; }
            }
        }
    }
}
