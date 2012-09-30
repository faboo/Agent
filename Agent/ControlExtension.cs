using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Globalization;

namespace Agent {
    public static class ControlExtension {
        public static FormattedText GetFont(this Control control) {
            return new FormattedText(
                "x",
                CultureInfo.CurrentUICulture,
                control.FlowDirection,
                new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch),
                control.FontSize,
                control.Foreground);
        }
    }
}
