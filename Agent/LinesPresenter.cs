using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.Specialized;

namespace Agent {
    public class LinesPresenter: ItemsControl, IScrollInfo {
        public static readonly DependencyPropertyKey ExtentHeightPropertyKey = DependencyProperty.RegisterReadOnly("ExtentHeight", typeof(double), typeof(LinesPresenter), new FrameworkPropertyMetadata(0.0, OnScrollChanged));
        public static readonly DependencyProperty ExtentHeightProperty = ExtentHeightPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey ExtentWidthPropertyKey = DependencyProperty.RegisterReadOnly("ExtentWidth", typeof(double), typeof(LinesPresenter), new FrameworkPropertyMetadata(0.0, OnScrollChanged));
        public static readonly DependencyProperty ExtentWidthProperty = ExtentWidthPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey HorizontalOffsetPropertyKey = DependencyProperty.RegisterReadOnly("HorizontalOffset", typeof(double), typeof(LinesPresenter), new FrameworkPropertyMetadata(0.0, OnScrollChanged));
        public static readonly DependencyProperty HorizontalOffsetProperty = HorizontalOffsetPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey VerticalOffsetPropertyKey = DependencyProperty.RegisterReadOnly("VerticalOffset", typeof(double), typeof(LinesPresenter), new FrameworkPropertyMetadata(0.0, OnScrollChanged));
        public static readonly DependencyProperty VerticalOffsetProperty = VerticalOffsetPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey ViewportHeightPropertyKey = DependencyProperty.RegisterReadOnly("ViewportHeight", typeof(double), typeof(LinesPresenter), new FrameworkPropertyMetadata(0.0, OnScrollChanged));
        public static readonly DependencyProperty ViewportHeightProperty = ViewportHeightPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey ViewportWidthPropertyKey = DependencyProperty.RegisterReadOnly("ViewportWidth", typeof(double), typeof(LinesPresenter), new FrameworkPropertyMetadata(0.0, OnScrollChanged));
        public static readonly DependencyProperty ViewportWidthProperty = ViewportWidthPropertyKey.DependencyProperty;

        
        public ScrollViewer ScrollOwner {
            get;
            set;
        }
        public bool CanHorizontallyScroll {
            get { return false; }
            set { }
        }
        public bool CanVerticallyScroll {
            get { return true; }
            set { }
        }
        public double ExtentHeight {
            get { return (double)GetValue(ExtentHeightProperty); }
            private set { SetValue(ExtentHeightPropertyKey, value); }
        }

        public double ExtentWidth {
            get { return (double)GetValue(ExtentWidthProperty); }
            private set { SetValue(ExtentWidthPropertyKey, value); }
        }

        public double HorizontalOffset {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            private set { SetValue(HorizontalOffsetPropertyKey, value); }
        }

        public double VerticalOffset {
            get { return (double)GetValue(VerticalOffsetProperty); }
            private set { SetValue(VerticalOffsetPropertyKey, value); }
        }

        public double ViewportHeight {
            get { return (double)GetValue(ViewportHeightProperty); }
            private set { SetValue(ViewportHeightPropertyKey, value); }
        }

        public double ViewportWidth {
            get { return (double)GetValue(ViewportWidthProperty); }
            private set { SetValue(ViewportWidthPropertyKey, value); }
        }

        public int LineOffset {
            get {
                double lineHeight = this.GetFont().Height;

                return (int)(VerticalOffset / lineHeight);
            }
        }

        public int VisibleLines {
            get {
                double lineHeight = this.GetFont().Height;

                return (int)(ViewportHeight / lineHeight);
            }
        }

        public event EventHandler ScrollChanged;

        public void LineUp() {
            SetVerticalOffset(VerticalOffset - this.GetFont().Height);
        }

        public void LineDown() {
            SetVerticalOffset(VerticalOffset + this.GetFont().Height);
        }

        public void LineLeft() {
        }

        public void LineRight() {
        }

        public void MouseWheelLeft() {
        }

        public void MouseWheelRight() {
        }

        public void MouseWheelUp() {
            SetVerticalOffset(VerticalOffset - 3 * this.GetFont().Height);
        }

        public void MouseWheelDown() {
            SetVerticalOffset(VerticalOffset + 3 * this.GetFont().Height);
        }

        public void PageUp() {
            double lineHeight = this.GetFont().Height;
            int lines = (int)(ViewportHeight / lineHeight);

            SetVerticalOffset(VerticalOffset - lineHeight * lines);
        }

        public void PageDown() {
            double lineHeight = this.GetFont().Height;
            int lines = (int)(ViewportHeight / lineHeight);

            SetVerticalOffset(VerticalOffset + lineHeight * lines);
        }

        public void PageLeft() {
        }

        public void PageRight() {
        }

        public void ScrollIntoView(Line line) {
            LinePresenter container = (LinePresenter)ItemContainerGenerator.ContainerFromItem(line);

            MakeVisible(container, new Rect(0, 0, container.ActualWidth, container.ActualHeight));
        }

        public Rect MakeVisible(Visual visual, Rect rectangle) {
            if(rectangle.IsEmpty || visual == null || visual == this || !base.IsAncestorOf(visual))
                return Rect.Empty;

            double lineHeight = this.GetFont().Height;
            Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);

            rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);

            if(!viewRect.Contains(rectangle)) {
                if(rectangle.Top < viewRect.Top) {
                    double offset = (Math.Floor((viewRect.Top + rectangle.Top) / lineHeight) - 2) * lineHeight;

                    SetVerticalOffset(offset);
                }
                else if(rectangle.Bottom > viewRect.Bottom) {
                    double offset = (Math.Floor((rectangle.Bottom - viewRect.Bottom) / lineHeight) + 2) * lineHeight;

                    SetVerticalOffset(offset);
                }
            }

            return rectangle;
        }

        public void SetHorizontalOffset(double horOffset) {
            // we can't scroll horizontally at all.
        }

        public void SetVerticalOffset(double vertOffset) {
            double lineHeight = this.GetFont().Height;

            if(vertOffset < 0)
                vertOffset = 0;
            if(vertOffset > ExtentHeight - ViewportHeight)
                vertOffset = ExtentHeight - ViewportHeight;

            VerticalOffset = Math.Floor(vertOffset / lineHeight) * lineHeight;

            if(vertOffset > VerticalOffset) {
                if(VerticalOffset < vertOffset)
                    VerticalOffset += lineHeight;
            }
            InvalidateArrange();
            InvalidateVisual();
        }

        protected override DependencyObject GetContainerForItemOverride() {
            LinePresenter container = new LinePresenter();

            container.SetBinding(LinePresenter.LineProperty, new Binding());

            return container;
        }

        protected override Size MeasureOverride(Size available) {
            if(this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) {
                Size constraint = new Size { Height = Double.PositiveInfinity, Width = available.Width };
                Size actualSize = new Size { Height = 0, Width = available.Width };
                // we should only have one child: the panel.
                UIElement panel = (UIElement)VisualTreeHelper.GetChild(this, 0);

                panel.Measure(constraint);
                actualSize.Height = panel.DesiredSize.Height;

                ViewportHeight = available.Height;
                ViewportWidth = available.Width;
                ExtentHeight = panel.DesiredSize.Height;
                ExtentWidth = available.Width;

                if(ScrollOwner != null)
                    ScrollOwner.InvalidateScrollInfo();
            }
            else {
                ItemContainerGenerator.StatusChanged += (s, a) => {
                        if(ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                            InvalidateMeasure();
                    };
            }

            return available;
        }

        protected override Size ArrangeOverride(Size arrangeBounds) {
            // we should only have one child: the panel.
            UIElement panel = (UIElement)VisualTreeHelper.GetChild(this, 0);

            panel.Arrange(new Rect(
                -HorizontalOffset,
                -VerticalOffset,
                arrangeBounds.Width,
                panel.DesiredSize.Height));

            return arrangeBounds;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs args) {
            base.OnItemsChanged(args);
            if(ScrollOwner != null)
                ScrollOwner.InvalidateScrollInfo();
        }

        private static void OnScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            LinesPresenter presenter = (sender as LinesPresenter);

            if(presenter.ScrollOwner != null)
                presenter.ScrollOwner.InvalidateScrollInfo();
            if(presenter.ScrollChanged != null)
                presenter.ScrollChanged(presenter, new EventArgs());
        }
    }
}
