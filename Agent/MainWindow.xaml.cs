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
using System.Collections.ObjectModel;

namespace Agent {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
		public static readonly DependencyProperty PadProperty = DependencyProperty.Register("Pad", typeof(Pad), typeof(MainWindow));

		public Pad Pad
		{
			get { return (Pad)GetValue(PadProperty); }
			set { SetValue(PadProperty, value); }
		}

        public MainWindow() {

            InitializeComponent();
            Pad = new Pad();

            editor.Focus();

            Left = System.Windows.SystemParameters.WorkArea.Width - (Width + 12);
            Top = System.Windows.SystemParameters.WorkArea.Height - (Height + 12);
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            Focus();
        }

        protected override void OnGotFocus(RoutedEventArgs e) {
            base.OnGotFocus(e);
            editor.Focus();
        }

        protected override void OnDeactivated(EventArgs args) {
            Visibility = System.Windows.Visibility.Hidden;
            Pad.Save();
        }
    }
}
