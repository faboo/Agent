using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Agent {
    public class LinePresenter : Control {
        static LinePresenter() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinePresenter), new FrameworkPropertyMetadata(typeof(LinePresenter)));
        }

		public static readonly DependencyProperty LineProperty = DependencyProperty.Register("Line", typeof(Line), typeof(LinePresenter), new FrameworkPropertyMetadata(OnLineChanged));

		public Line Line
		{
			get { return (Line)GetValue(LineProperty); }
			set { SetValue(LineProperty, value); }
		}

        // TransformToAncestor (apparently) gives answers that are slightly off. This is a workaround.
        public Rect VisualRect {
            get {
                return new Rect(this.VisualOffset.X, this.VisualOffset.Y, this.ActualWidth, this.ActualHeight);
            }
        }

        public int GetVisualLineCount() {
            //TODO: This is an appoximation
            return Math.Max((int)Math.Ceiling((this.GetFont().Width * Line.Text.Length) / ActualWidth), 1);
        }

        private Typeface CreateTypeface() {
            return new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        }

        private Typeface CreateTypeface(Highlight highlight) {
            return new Typeface(FontFamily, highlight.Style, highlight.Weight, FontStretch);
        }

        private double GetTextHeight(double width) {
            var font = this.GetFont();
            int lineWidth = (int)(width / font.Width);
            int lines = 1 + Line.Text.Length / lineWidth;

            return lines * font.Height;
        }

        private FormattedText MeasureText(string text) {
            return new FormattedText(
                    String.IsNullOrEmpty(text) ? " " : text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection,
                    CreateTypeface(),
                    FontSize,
                    Foreground) {
                        MaxTextWidth = ActualWidth
                    };
        }

        private void RenderHighlight(DrawingContext drawingContext, Highlight highlight) {
			RenderText(drawingContext, highlight.Foreground, highlight.Background, highlight.Start, highlight.Length);
		}

        private FormattedText RenderHighlight(DrawingContext drawingContext, int line, Highlight highlight) {
            var font = this.GetFont();
			int lineWidth = (int)(this.ActualWidth / font.Width);
            return new FormattedText(
                Line.Text.Substring(highlight.Start, highlight.Length),
                CultureInfo.CurrentUICulture,
                FlowDirection,
                CreateTypeface(highlight),
                FontSize,
                new SolidColorBrush(highlight.Foreground)) {
                    MaxTextWidth = ActualWidth
                };
        }

		private void RenderText(DrawingContext context, Brush foreground, Brush background, int textOffset, int textWidth){
			var font = this.GetFont();
			int lineWidth = (int)(this.ActualWidth / font.Width);

			while(textWidth > 0){
				int width = Math.Min(lineWidth - (textOffset%lineWidth), textWidth);
				int line = textOffset/lineWidth;
				double y = line * font.Height;
                double x = textOffset % lineWidth * font.Width;
				var format = new FormattedText(
						Line.Text.Substring(textOffset, width),
						CultureInfo.CurrentUICulture,
						FlowDirection,
						CreateTypeface(),
						FontSize,
						foreground);

                if(background != null)
                    context.DrawRectangle(
                        background,
                        new Pen(background, 1),
                        new Rect(x, y, width * font.Width, font.Height));
                context.DrawText(format, new Point(x, y));
				textOffset += width;
                textWidth -= width;
			}
		}

		private void RenderText(DrawingContext context, Color foreground, Color background, int textOffset, int textWidth){
			RenderText(context, new SolidColorBrush(foreground), new SolidColorBrush(background), textOffset, textWidth);
		}

        private Point GetHighlightOffset(int lineWidth, Highlight highlight) {
            return new Point(
                this.GetFont().Width * (highlight.Start % lineWidth),
                this.GetFont().Height * (highlight.Start / lineWidth));
        }

        private void OnLineChanged(Line oldValue) {
            if (oldValue != null)
                oldValue.Changed -= OnLineModified;
            if (Line != null)
                Line.Changed += OnLineModified;

            InvalidateMeasure();
            InvalidateVisual();
        }

        private void OnLineModified(object sender, EventArgs args) {
            InvalidateMeasure();
            InvalidateVisual();
        }

        protected override Size MeasureOverride(Size constraint) {
            if(Line == null)
                return new Size();

            return new Size(constraint.Width, GetTextHeight(constraint.Width));
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if(Line != null) {
                RenderText(drawingContext, Foreground, Background, 0, Line.Text.Length);

                foreach(Highlight hl in Line.Highlights)
					RenderHighlight(drawingContext, hl);
            }
        }

        private static void OnLineChanged(object sender, DependencyPropertyChangedEventArgs args) {
            (sender as LinePresenter).OnLineChanged(args.OldValue as Line);
        }
    }
}
