using Microsoft.Extensions.DependencyModel;
using PolarShadows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PolarShadow.Controls
{
    class FlexPanel : Panel, IScrollInfo
    {
        private const double Epsilon = 0.00000153;
        private const double MouseWheelOffset = 15d;
        private bool _isHorizontal => FlexDirection == FlexDirection.Row || FlexDirection == FlexDirection.RowReverse;
        private bool _isReverse => FlexDirection == FlexDirection.RowReverse || FlexDirection == FlexDirection.ColumnReverse;

        private ScrollData _scrollData;
        private LayoutData _layoutData;

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

        public static readonly DependencyProperty JustifyItemsProperty = DP.Register<FlexPanel, FlexAlign>(nameof(JustifyItems), FlexAlign.Center, metadataCreator: () => new FrameworkPropertyMetadata { AffectsArrange = true });
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

        public static readonly DependencyProperty FloatProperty = DP.RegisterAttached<FlexPanel, bool>("Float", false, metadataCreator: ()=> new FrameworkPropertyMetadata { AffectsArrange = true});
        public static bool GetFloat(UIElement element)
        {
            return (bool)element.GetValue(FloatProperty);
        }
        public static void SetFloat(UIElement element, bool value)
        {
            element.SetValue(FloatProperty, value);
        }


        public ScrollViewer ScrollOwner
        {
            get
            {
                EnsureScrollData();
                return _scrollData._scrollOwner;
            }
            set
            {
                EnsureScrollData();
                if (value != _scrollData._scrollOwner)
                {
                    _scrollData._scrollOwner = value;
                }
            }
        }

        public bool CanHorizontallyScroll
        {
            get
            {
                if (_scrollData == null) return false;
                return _scrollData._canHorizontal;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._canHorizontal != value)
                {
                    _scrollData._canHorizontal = value;
                    InvalidateMeasure();
                }
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                if (_scrollData == null) return false;
                return _scrollData._canVertical;
            }
            set
            {
                EnsureScrollData();
                if (_scrollData._canVertical != value)
                {
                    _scrollData._canVertical = value;
                    InvalidateMeasure();
                }
            }
        }

        public double ExtentHeight => _layoutData == null ? 0d : _layoutData._extent.Height;

        public double ExtentWidth => _layoutData == null ? 0d : _layoutData._extent.Width;

        public double ViewportHeight => _layoutData == null ? 0d : _layoutData._viewPort.Height;

        public double ViewportWidth => _layoutData == null ? 0d : _layoutData._viewPort.Width;

        public double HorizontalOffset => _scrollData == null ? 0d : _scrollData._offset.X;

        public double VerticalOffset => _scrollData == null ? 0d : _scrollData._offset.Y;

        public void LineUp()
        {
            SetVerticalOffset(_scrollData._offset.Y - _layoutData.GetLineUpOffset());
        }

        public void LineDown()
        {
            SetVerticalOffset(_scrollData._offset.Y + _layoutData.GetLineDownOffset());
        }

        public void LineLeft()
        {
            SetHorizontalOffset(_scrollData._offset.X - _layoutData.GetLineLeftOffset());
        }

        public void LineRight()
        {
            SetHorizontalOffset(_scrollData._offset.X + _layoutData.GetLineRightOffset());
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(_scrollData._offset.Y - MouseWheelOffset);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(_scrollData._offset.Y + MouseWheelOffset);
        }
        public void MouseWheelLeft()
        {
            SetHorizontalOffset(_scrollData._offset.X - MouseWheelOffset);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(_scrollData._offset.X + MouseWheelOffset);
        }

        public void PageUp()
        {
            SetVerticalOffset(_scrollData._offset.Y - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(_scrollData._offset.Y + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(_scrollData._offset.X - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(_scrollData._offset.X + ViewportWidth);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            var childTransform = visual.TransformToAncestor(this);
            rectangle = childTransform.TransformBounds(rectangle);

            if (!IsScrolling())
            {
                return rectangle;
            }

            EnsureLayoutData();

            var newOffset = new Vector();
            if (_scrollData._canHorizontal)
            {
                if (rectangle.X < 0)
                {
                    newOffset.X = _scrollData._offset.X + rectangle.X;
                }
                else if (rectangle.X > _layoutData._viewPort.Width)
                {
                    newOffset.X = _scrollData._offset.X + rectangle.Right;
                }
            }

            if (_scrollData._canVertical)
            {
                if (rectangle.Y < 0)
                {
                    newOffset.Y = _scrollData._offset.Y + rectangle.Y;
                }
                else if (rectangle.Y > _layoutData._viewPort.Height)
                {
                    newOffset.Y = _scrollData._offset.Y + rectangle.Bottom;
                }
            }

            if (!IsClose(_scrollData._offset, newOffset))
            {
                _scrollData._offset = newOffset;
                InvalidateMeasure();
                OnScrollChange();
            }

            return rectangle;
        }

        public void SetHorizontalOffset(double offset)
        {
            EnsureScrollData();
            if (offset < 0 || ViewportWidth >= ExtentWidth)
            {
                offset = 0;
            }
            else
            {
                var delta = ExtentWidth - ViewportWidth;
                if (offset > delta)
                {
                    offset = delta;
                }
            }

            
            if (!IsClose(offset, _scrollData._offset.X))
            {
                _scrollData._offset.X = offset;
                InvalidateMeasure();
                OnScrollChange();
            }
        }

        public void SetVerticalOffset(double offset)
        {
            EnsureScrollData();
            if (offset < 0 || ViewportHeight >= ExtentHeight)
            {
                offset = 0;
            }
            else
            {
                var delta = ExtentHeight - ViewportHeight;
                if (offset > delta)
                {
                    offset = delta;
                }
            }
            
            if (!IsClose(offset, _scrollData._offset.Y))
            {
                _scrollData._offset.Y = offset;
                InvalidateMeasure();
                OnScrollChange();
            }
        }

        private void EnsureScrollData()
        {
            if (_scrollData == null)
            {
                _scrollData = new ScrollData();
                EnsureLayoutData();
            }
        }

        private void ResetScrolling()
        {
            InvalidateMeasure();
        }

        private void OnScrollChange()
        {
            if (ScrollOwner != null) 
            { 
                ScrollOwner.InvalidateScrollInfo(); 
            }
        }

        private static bool IsClose(double value1, double value2)
        {
            if (value1 == value2) return true;

            var delta = value1 - value2;
            return (delta < Epsilon) && (delta > -Epsilon);
        }

        private static bool IsClose(Vector value1, Vector value2)
        {
            if (value1 == value2) return true;
            var delta = value1 - value2;
            return (delta.X > -Epsilon) && (delta.X < Epsilon)
                && (delta.Y > -Epsilon) && (delta.Y > Epsilon);
        }

        private static bool IsClose(UVSize value1, UVSize value2)
        {
            if (value1 == value2) return true;
            var deltaU = Math.Abs(value1.U - value2.U);
            var deltaV= Math.Abs(value1.V - value2.V);
            return deltaU < Epsilon
                && deltaV < Epsilon;
        }

        private void EnsureLayoutData()
        {
            if (_layoutData == null)
            {
                _layoutData = new LayoutData();
            }
            _layoutData.SetScrollData(_scrollData);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var panelSize = new UVSize(FlexDirection);
            var constraintSize = new UVSize(FlexDirection, availableSize.Width, availableSize.Height);
            var curLineSize = new UVSize(FlexDirection);
            var shouldWrap = FlexWrap != FlexWrap.NoWrap;
            var fValid = true;

            var children = InternalChildren;

            EnsureLayoutData();
            _layoutData.Reset();

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
                        _layoutData.AddLine(panelSize.U, panelSize.V, curLineSize, _isHorizontal);
                        panelSize.U = Math.Max(panelSize.U, curLineSize.U);
                        panelSize.V += curLineSize.V;
                        curLineSize = sz;
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

            _layoutData.AddLine(panelSize.U, panelSize.V, curLineSize, _isHorizontal);
            panelSize.U = Math.Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;

            fValid = IsClose(_layoutData._viewPort, constraintSize);
            fValid &= IsClose(_layoutData._extent, panelSize);

            if (!fValid)
            {
                _layoutData._viewPort = constraintSize;
                _layoutData._extent = panelSize;
                OnScrollChange();
            }

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
            var remainV = uvFinal.V - _layoutData._extent.V;
            var lineCount = _layoutData.LineCount;

            if (remainV > 0)
            {
                switch (alignContent)
                {
                    case FlexAlignContent.End:
                        v = remainV;
                        break;
                    case FlexAlignContent.Center:
                        v = remainV / 2;
                        break;
                    case FlexAlignContent.SpaceAround:
                        v = remainV / (lineCount * 2);
                        break;
                    case FlexAlignContent.SpaceEvenly:
                        v = remainV / (lineCount + 1);
                        break;
                    case FlexAlignContent.Stretch:
                        appendV = remainV / lineCount;
                        break;
                }
            }

            var children = InternalChildren;
            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child == null) continue;

                var grow = GetFlexGrow(child);
                var shrink = GetFlexShrink(child);

                var sz = GetItemBasisUVSize(child, uvFinal.U);
                if (alignContent == FlexAlignContent.Stretch)
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
                                if (lineCount > 1)
                                {
                                    v += curLineSize.V + remainV / (lineCount - 1);
                                }
                                break;
                            case FlexAlignContent.SpaceAround:
                                v += curLineSize.V + remainV / (lineCount * 2) * 2;
                                break;
                            case FlexAlignContent.SpaceEvenly:
                                v += curLineSize.V + remainV / (lineCount + 1);
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
            var x = isHorizontal ? u : v;
            var y = isHorizontal ? v : u;
            var isFloat = GetFloat(element);
            if (_scrollData != null && !isFloat)
            {
                if (_scrollData._canHorizontal)
                {
                    x -= _scrollData._offset.X;
                }
                if (_scrollData._canVertical)
                {
                    y -= _scrollData._offset.Y;
                }
            }
            element.Arrange(new Rect(x, y, isHorizontal ? size.U : lineSize.V, isHorizontal ? lineSize.V : size.U));
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

        private bool IsScrolling()
        {
            return _scrollData != null && _scrollData._scrollOwner != null;
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
                get => _isHorizontal ? V : U;
                set { if (_isHorizontal) V = value; else U = value; }
            }

            public static bool operator ==(UVSize value1, UVSize value2) => value1._isHorizontal == value2._isHorizontal
                    && value1.U == value2.U
                    && value1.V == value2.V;

            public static bool operator !=(UVSize value1, UVSize value2) => value1._isHorizontal != value2._isHorizontal
                    || value1.U != value2.U
                    || value1.V != value2.V;
        }

        private class ScrollData
        {
            internal ScrollViewer _scrollOwner;
            internal Vector _offset;
            internal bool _canVertical;
            internal bool _canHorizontal;
        }

        private class LayoutData
        {
            private ScrollData _scrollData;
            private List<Rect> _lines = new List<Rect>();
            internal UVSize _viewPort;
            internal UVSize _extent;

            public int LineCount => _lines.Count;

            public Rect this[int index]
            {
                get => _lines[index];
            }

            public void SetScrollData(ScrollData scrollData)
            {
                _scrollData = scrollData;
            }

            public void AddLine(double u, double v, UVSize size, bool isHorizontal)
            {
                var x = isHorizontal ? u : v;
                var y = isHorizontal ? v : u;
                if (_scrollData != null)
                {
                    if (_scrollData._canVertical)
                    {
                        y -= _scrollData._offset.Y;
                    }
                    if (_scrollData._canHorizontal)
                    {
                        x -= _scrollData._offset.X;
                    }
                }
                _lines.Add(new Rect(x, y, size.Width, size.Height));
            }

            public int LineOf(Rect rect)
            {
                var center = new Point((rect.Right - rect.Left) / 2 + rect.X, (rect.Bottom - rect.Top) / 2 + rect.Y);
                for (int i = 0; i < _lines.Count; i++)
                {
                    if (_lines[i].Contains(center))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public Rect GetLine(Rect rect)
            {
                var index = LineOf(rect);
                if (index < 0)
                {
                    return Rect.Empty;
                }
                return _lines[index];
            }

            public void Reset()
            {
                _lines.Clear();
            }

            public double GetLineUpOffset()
            {
                if (_scrollData == null) return 0;
                for (int i = 0; i < _lines.Count; i++)
                {
                    var line = _lines[i];

                    if (line.Height - Math.Abs(line.Y) >= 0)
                    {
                        return Math.Abs(line.Y);
                    }
                }

                return 0;
            }

            public double GetLineDownOffset()
            {
                var sumH = 0d;
                for (int i = 0; i < _lines.Count; i++)
                {
                    var line = _lines[i];
                    var lineHeight = line.Height;
                    if (line.Y < 0)
                    {
                        var y = Math.Abs(line.Y);
                        if (y < lineHeight)
                        {
                            sumH += lineHeight - y;
                        }
                    }
                    else
                    {
                        sumH += lineHeight;
                    }

                    if (_viewPort.Height - sumH <= 0)
                    {
                        if(sumH - _viewPort.Height > Epsilon)
                        {
                            return sumH - _viewPort.Height;
                        }
                    }
                }

                return 0;
            }

            public double GetLineLeftOffset()
            {
                if (_scrollData == null) return 0;
                for (int i = 0; i < _lines.Count; i++)
                {
                    var line = _lines[i];
                    if (line.Width - Math.Abs(line.X) >= 0)
                    {
                        return Math.Abs(line.X);
                    }
                }

                return 0;
            }

            public double GetLineRightOffset()
            {
                var sumW = 0d;
                for (int i = 0; i < _lines.Count; i++)
                {
                    var line = _lines[i];
                    var colWidth = line.Width;
                    if (line.X < 0)
                    {
                        var x = Math.Abs(line.X);
                        if (x < line.Width)
                        {
                            sumW += line.Width - x;
                        }
                    }
                    else
                    {
                        sumW += line.Width;
                    }

                    if (_viewPort.Width - sumW <= 0)
                    {
                        return sumW - _viewPort.Width;
                    }
                }

                return 0;
            }
        }
    }
}
