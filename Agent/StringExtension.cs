using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent {
    public static class StringExtension {
        public static string Repeat(this char c, int repeat) {
            return new String(c, repeat);
        }
    }
}
