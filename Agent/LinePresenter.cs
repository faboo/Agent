using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

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

        private FormattedText formattedText = null;

        private Typeface CreateTypeface() {
            return new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        }

        private Typeface CreateTypeface(Highlight highlight) {
            return new Typeface(FontFamily, highlight.Style, highlight.Weight, FontStretch);
        }

        private FormattedText MeasureText(double width) {
            if (formattedText == null) {
                formattedText = new FormattedText(
                    String.IsNullOrEmpty(Line.Text)? " " : Line.Text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection,
                    CreateTypeface(),
                    FontSize,
                    Foreground);
                formattedText.MaxTextWidth = width;
            }

            return formattedText;
        }

        private FormattedText RenderHighlight(Highlight highlight) {
            FormattedText text = new FormattedText(
                Line.Text.Substring(highlight.Start, highlight.Length),
                CultureInfo.CurrentUICulture,
                FlowDirection,
                CreateTypeface(highlight),
                FontSize,
                new SolidColorBrush(highlight.Foreground));
            text.MaxTextWidth = formattedText.MaxTextWidth;

            return text;
        }

        private double GetHighlightOffset(Highlight highlight) {
            return this.GetFont().Width * highlight.Start;
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
            formattedText = null;
            InvalidateMeasure();
            InvalidateVisual();
        }

        protected override Size MeasureOverride(Size constraint) {
            if(Line == null)
                return new Size();

            FormattedText text = MeasureText(constraint.Width);

            return new Size(constraint.Width, text.Height + text.LineHeight);
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if(Line != null) {
                drawingContext.DrawText(formattedText, new Point(0, 0));

                foreach(Highlight hl in Line.Highlights) {
                    drawingContext.DrawText(
                        RenderHighlight(hl),
                        new Point(GetHighlightOffset(hl), 0));
                }
            }
        }

        private static void OnLineChanged(object sender, DependencyPropertyChangedEventArgs args) {
            (sender as LinePresenter).OnLineChanged(args.OldValue as Line);
        }
    }
}
