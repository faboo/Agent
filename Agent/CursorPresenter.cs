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
    public class CursorPresenter : Control {
		public new static readonly DependencyProperty CursorProperty = DependencyProperty.Register("Cursor", typeof(Cursor), typeof(CursorPresenter), new FrameworkPropertyMetadata(OnCursorChanged));

        static CursorPresenter() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CursorPresenter), new FrameworkPropertyMetadata(typeof(CursorPresenter)));
        }

		public new Cursor Cursor
		{
			get { return (Cursor)GetValue(CursorProperty); }
			set { SetValue(CursorProperty, value); }
		}

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args) {
            base.OnPropertyChanged(args);

            if(args.Property == DataContextProperty) {
            }
        }

        private void OnCursorChanged(Cursor oldCursor) {
            if (oldCursor != null)
                oldCursor.Changed -= OnCursorMoved;
            if (Cursor != null)
                Cursor.Changed += OnCursorMoved;
            InvalidateVisual();
        }

        private void OnCursorMoved(object sender, EventArgs args) {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            if (Cursor != null) {
                var font = this.GetFont();
                int maxWidth = (int)(ActualWidth / font.Width);
                int row = Cursor.Column / maxWidth;
                int column = Cursor.Column % maxWidth;

                if (Cursor.Type == CursorType.Bar)
                    DrawBar(drawingContext, row, column, font);
                else if (Cursor.Type == CursorType.Block)
                    DrawBlock(drawingContext, row, column, font);
                else if (Cursor.Type == CursorType.Box)
                    DrawBox(drawingContext, row, column, font);
                else if (Cursor.Type == CursorType.Line)
                    DrawLine(drawingContext, row, column, font);
            }
        }

        private void DrawBar(DrawingContext drawingContext, int row, int column, FormattedText font) {
            drawingContext.DrawLine(
                new Pen(Foreground, 2),
                new Point(column * font.Width, row * font.Height),
                new Point(column * font.Width, (row + 1) * font.Height));
        }

        private void DrawBlock(DrawingContext drawingContext, int row, int column, FormattedText font) {
            drawingContext.DrawRectangle(
                Foreground,
                new Pen(Foreground, 2),
                new Rect {
                    Height = font.Height,
                    Width = font.Width,
                    X = column * font.Width,
                    Y = row * font.Height
                });
        }

        private void DrawBox(DrawingContext drawingContext, int row, int column, FormattedText font) {
            drawingContext.DrawRectangle(
                null,
                new Pen(Foreground, 1),
                new Rect {
                    Height = font.Height,
                    Width = font.Width,
                    X = column * font.Width,
                    Y = row * font.Height
                });
        }

        private void DrawLine(DrawingContext drawingContext, int row, int column, FormattedText font) {
            drawingContext.DrawLine(
                new Pen(Foreground, 2),
                new Point(column * font.Width, (row + 1) * font.Height - 1),
                new Point((column + 1) * font.Width, (row + 1) * font.Height - 1));
        }

        private static void OnCursorChanged(object sender, DependencyPropertyChangedEventArgs args) {
            (sender as CursorPresenter).OnCursorChanged(args.OldValue as Cursor);
        }
    }
}
