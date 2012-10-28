using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows;

namespace Agent {
    public static class Highlighter {
        private static Dictionary<Regex, Highlight> Highlights = new Dictionary<Regex, Highlight> {
        };

        public static void Add(Regex regex, Highlight hilight) {
            Highlights[regex] = hilight;
        }

        public static void Highlight(Line line) {
            line.Highlights.Clear();

            foreach(var hl in Highlights){
                var matches = hl.Key.Matches(line.Text);

                foreach(Match match in matches)
                    line.Highlights.Add(new Highlight(hl.Value) {
                        Start = match.Index,
                        Length = match.Length
                    });
            }
        }
    }
}
