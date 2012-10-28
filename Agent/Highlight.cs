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

        public static Color ParseColor(string colorString) {
            Color color = Colors.Black;

            try {
                color = (Color)ColorConverter.ConvertFromString(colorString);
            }
            catch {
            }

            return color;
        }

        public static FontStyle ParseStyle(string styleString) {
            FontStyle style = FontStyles.Normal;
            FontStyleConverter converter = new FontStyleConverter();

            try {
                style = (FontStyle)converter.ConvertFromString(styleString);
            }
            catch {
            }

            return style;
        }

        public static FontWeight ParseWeight(string weightString) {
            FontWeight weight = FontWeights.Normal;
            FontWeightConverter converter = new FontWeightConverter();

            try {
                weight = (FontWeight)converter.ConvertFromString(weightString);
            }
            catch {
            }

            return weight;
        }
    }
}
