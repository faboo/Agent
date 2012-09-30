using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agent {
    public static class KeyExtension {
        public static bool IsVisible(this Key key) {
            bool visible = true;

            visible &= !(key <= Key.Help && key != Key.Space);
            visible &= !(key >= Key.LWin && key <= Key.Sleep);
            visible &= !(key >= Key.NumLock && key <= Key.LaunchApplication2);
            visible &= !(key >= Key.ImeProcessed);

            return visible;
        }
    }
}
