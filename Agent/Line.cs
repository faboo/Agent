using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Collections.ObjectModel;

namespace Agent {
    public class Line : Freezable {
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Line), new FrameworkPropertyMetadata(OnTextChanged));
		public static readonly DependencyProperty HighlightsProperty = DependencyProperty.Register("Highlights", typeof(ObservableCollection<Highlight>), typeof(Line));
		public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(Inline), typeof(Line));
		public static readonly DependencyProperty CursorProperty = DependencyProperty.Register("Cursor", typeof(Cursor), typeof(Line), new FrameworkPropertyMetadata(OnCursorChanged));

        public Line() {
            Highlights = new ObservableCollection<Highlight>();
        }

        public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public ObservableCollection<Highlight> Highlights
		{
			get { return (ObservableCollection<Highlight>)GetValue(HighlightsProperty); }
			set { SetValue(HighlightsProperty, value); }
		}
		public Cursor Cursor
		{
			get { return (Cursor)GetValue(CursorProperty); }
			set { SetValue(CursorProperty, value); }
		}

        private static void OnTextChanged(object sender, DependencyPropertyChangedEventArgs args) {
            Highlighter.Highlight(sender as Line);
        }

        private static void OnCursorChanged(object sender, DependencyPropertyChangedEventArgs args) {
        }

        protected override Freezable CreateInstanceCore() {
            return new Line();
        }
    }
}
