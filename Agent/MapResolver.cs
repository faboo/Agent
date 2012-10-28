using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agent {
    public delegate void AddMap(Mode mode, InputGesture gesture, Action<PadEditor> action);

    public static class MapResolver {
        public static Dictionary<Mode, Dictionary<InputGesture, Action<PadEditor>>> maps =
            new Dictionary<Mode, Dictionary<InputGesture, Action<PadEditor>>>();

        public static event AddMap MapAdded;

        public static void Map(string modeName, string keyName, string keys) {
            Mode mode = ResolveMode(modeName);
            InputGesture key = ResolveKeyName(ref keyName);

            if(!maps.ContainsKey(mode))
                maps[mode] = new Dictionary<InputGesture, Action<PadEditor>>();

            maps[mode][key] = (editor) =>
                EditingCommands.Macro.Execute(keys, editor);

            if(MapAdded != null)
                MapAdded(mode, key, maps[mode][key]);
        }

        public static bool HasMacros(Mode mode) {
            return maps.ContainsKey(mode);
        }

        public static IDictionary<InputGesture, Action<PadEditor>> GetMacros(Mode mode) {
            return maps[mode];
        }

        public static EditGesture ResolveKeyName(ref string keys) {
            EditGesture gesture = new EditGesture();
            Key keyValue;
            string key;

            if(keys.StartsWith("^")) {
                gesture.Mods = ModifierKeys.Control;
                keys = keys.Substring(1);
            }

            if(keys.Length < 1)
                throw new Exception("key string is empty");

            if(keys.StartsWith("<")){
                int end = keys.IndexOf('>');

                if(end == -1)
                    throw new Exception("Open < not closed in map");

                key = keys.Substring(1, end - 1);
                keys.Substring(key.Length + 2);
            }
            else{
                key = keys.Substring(0, 1);
                keys = keys.Substring(1);
            }

            if(!Enum.TryParse(key, true, out keyValue))
                throw new Exception("Invalid key name " + key);

            gesture.Key = keyValue;

            return gesture;
        }

        public static Mode ResolveMode(string mode) {
            switch(mode.ToLower()){
                case "command": return DefaultModes.Command;
                case "go": return DefaultModes.Go;
                case "insert": return DefaultModes.Insert;
                default: return null;
            }
        }

        public static IEnumerable<Action<PadEditor>> ResolveMacro(Mode mode, string macro) {
            List<Action<PadEditor>> keys = new List<Action<PadEditor>>();

            macro = macro.Substring(0);

            while(macro.Length > 0) {
                if(Char.IsDigit(macro[0])) {
                    string digits = new String(macro.TakeWhile(c => Char.IsDigit(c)).ToArray());

                    macro = macro.Substring(digits.Length);
                    keys.Add(new Action<PadEditor>(pe =>
                        pe.AddToCount(digits)));
                }
                else {
                    EditGesture gesture = ResolveKeyName(ref macro);

                    if(mode.Gestures.ContainsKey(gesture))
                        keys.Add(mode.Gestures[gesture]);
                    else
                        throw new Exception("Key not found: " + gesture.ToString());
                }
            }

            return keys;
        }
    }
}
