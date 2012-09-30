using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;

namespace Agent {
    public class Line : Freezable {
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Line));
		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(Inline), typeof(Line));
		public static readonly DependencyProperty CursorProperty = DependencyProperty.Register("Cursor", typeof(Cursor), typeof(Line), new FrameworkPropertyMetadata(OnCursorChanged));

        public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public Inline Display
		{
			get { return (Inline)GetValue(DisplayProperty); }
			set { SetValue(DisplayProperty, value); }
		}
		public Cursor Cursor
		{
			get { return (Cursor)GetValue(CursorProperty); }
			set { SetValue(CursorProperty, value); }
		}

        private static void OnCursorChanged(object sender, DependencyPropertyChangedEventArgs args) {
        }

        protected override Freezable CreateInstanceCore() {
            return new Line();
        }
    }
}
