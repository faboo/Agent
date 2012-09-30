using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agent {
    public class EditGesture : InputGesture {
        public Key Key { get; set; }
        public ModifierKeys Mods { get; set; }

        public EditGesture(Key key) {
            Key = key;
            Mods = ModifierKeys.None;
        }

        public EditGesture(Key key, ModifierKeys mods) {
            Key = key;
            Mods = mods;
        }

        private bool IsModDown(KeyboardDevice keyboard) {
            bool down = true;

            down &= (Mods & ModifierKeys.Alt) == ModifierKeys.Alt
                == (keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.RightAlt));

            down &= (Mods & ModifierKeys.Control) == ModifierKeys.Control
                == (keyboard.IsKeyDown(Key.LeftCtrl) || keyboard.IsKeyDown(Key.RightCtrl));

            down &= (Mods & ModifierKeys.Shift) == ModifierKeys.Shift
                == (keyboard.IsKeyDown(Key.LeftShift) || keyboard.IsKeyDown(Key.RightShift));

            down &= (Mods & ModifierKeys.Windows) == ModifierKeys.Windows
                == (keyboard.IsKeyDown(Key.LWin) || keyboard.IsKeyDown(Key.RWin));

            return down;
        }

        public bool IsKey(KeyEventArgs args) {
            bool match = args.IsDown && args.Key == Key;

            match = match && IsModDown(args.KeyboardDevice);

            return match;
        }

        public override bool Matches(object targetElement, InputEventArgs args) {
            return args is KeyEventArgs && IsKey(args as KeyEventArgs);
        }
    }
}
