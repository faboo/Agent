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
            // dates
            {
                new Regex(@"\d\d?/\d\d?/(\d\d)?\d\d"),
                new Highlight {
                    Foreground = Colors.Blue
                }
            },
            // dollars
            {
                new Regex(@"\$\d+(\.\d+)?"),
                new Highlight {
                    Foreground = Colors.DarkGreen
                }
            },
            // commands
            {
                new Regex(@"^[^:\s]+:"),
                new Highlight {
                    Weight = FontWeights.Bold
                }
            },
            // command arguments
            {
                new Regex(@"^\s+[^:\s]+:"),
                new Highlight {
                    Weight = FontWeights.Bold,
                    Foreground = Colors.DarkSlateGray
                }
            },
        };

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
