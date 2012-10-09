using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agent {
    public class Cursor : Freezable {
        public static readonly DependencyProperty RowProperty = DependencyProperty.Register("Row", typeof(int), typeof(Cursor), new FrameworkPropertyMetadata { AffectsRender = true });
        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register("Column", typeof(int), typeof(Cursor), new FrameworkPropertyMetadata { AffectsRender = true });
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(CursorType), typeof(Cursor), new FrameworkPropertyMetadata { AffectsRender = true });

        public Cursor() {
        }

        public Cursor(Range range) {
            Row = range.EndRow;
            Column = range.EndColumn;
        }

		public int Row
		{
			get { return (int)GetValue(RowProperty); }
			set { SetValue(RowProperty, value); }
		}
		public int Column
		{
			get { return (int)GetValue(ColumnProperty); }
			set { SetValue(ColumnProperty, value); }
		}
		public CursorType Type
		{
			get { return (CursorType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args) {
            try {
                base.OnPropertyChanged(args);
            }
            catch(ArgumentOutOfRangeException) {
                // Changing the cursor throws this on occasion. It shouldn't.
            }
        }

        protected override Freezable CreateInstanceCore() {
            return new Cursor();
        }
    }
}
