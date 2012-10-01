using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Agent {
    public class Highlight {
        public Highlight() {
            Style = FontStyles.Normal;
            Weight = FontWeights.Normal;
            Foreground = Colors.Black;
            Background = Colors.Transparent;
        }

        public Highlight(Highlight copy) {
            Start = copy.Start;
            Length = copy.Length;
            Style = copy.Style;
            Weight = copy.Weight;
            Foreground = copy.Foreground;
            Background = copy.Background;
        }

        public int Start { get; set; }
        public int Length { get; set; }
        public FontStyle Style { get; set; }
        public FontWeight Weight { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }
    }
}
