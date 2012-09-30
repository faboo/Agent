using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Agent {
    public class Reminder : Freezable {
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public Frequency Frequency { get; set; }

        protected override Freezable CreateInstanceCore() {
            return new Reminder();
        }
    }
}
