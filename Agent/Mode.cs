using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Agent {
    public class Mode: IMode {
        public bool DefaultInsert { get; set; }
        public CursorType Cursor { get; set; }
        public Dictionary<InputGesture, Action<PadEditor>> Gestures { get; set; }
        public MotionMode Motions { get; set; }
    }

    public static class DefaultModes {
        public static Mode Command = new Mode{
            DefaultInsert = false,
            Cursor = CursorType.Box,
            Motions = DefaultMotions.Command,
            Gestures = new Dictionary<InputGesture,Action<PadEditor>> {
                //
                // Movement
                //
                { new EditGesture(Key.H), editor => EditingCommands.MoveLeft.Execute(editor.Count, editor) },
                { new EditGesture(Key.L), editor => EditingCommands.MoveRight.Execute(editor.Count, editor) },
                { new EditGesture(Key.K), editor => EditingCommands.MoveUp.Execute(editor.Count, editor) },
                { new EditGesture(Key.J), editor => EditingCommands.MoveDown.Execute(editor.Count, editor) },
                { new EditGesture(Key.D4, ModifierKeys.Shift), editor => EditingCommands.MoveEnd.Execute(null, editor) },
                { new EditGesture(Key.D6, ModifierKeys.Shift), editor => EditingCommands.MoveHome.Execute(null, editor) },

                //
                // Adding text
                //
                { new EditGesture(Key.O, ModifierKeys.Shift), editor => {
                        EditingCommands.PrependNewline.Execute(null, editor);
                        EditingCommands.SetMode.Execute(DefaultModes.Insert, editor);
                    }
                },
                { new EditGesture(Key.O), editor => {
                        EditingCommands.AppendNewline.Execute(null, editor);
                        EditingCommands.SetMode.Execute(DefaultModes.Insert, editor);
                    }
                },
                { new EditGesture(Key.P, ModifierKeys.Shift), editor => {
                        if (Clipboard.ContainsText(TextDataFormat.Text))
                            for(int times = editor.Count == null ? 1 : (int)editor.Count;
                                    times > 0; times -= 1)
                                EditingCommands.PrependText.Execute(Clipboard.GetText(TextDataFormat.Text), editor);
                    }
                },
                { new EditGesture(Key.P), editor => {
                        if (Clipboard.ContainsText(TextDataFormat.Text))
                            for(int times = editor.Count == null ? 1 : (int)editor.Count;
                                    times > 0; times -= 1)
                                EditingCommands.AppendText.Execute(Clipboard.GetText(TextDataFormat.Text), editor);
                    }
                },

                //
                // Changing modes
                //
                { new EditGesture(Key.I), editor => EditingCommands.SetMode.Execute(Insert, editor) },
                { new EditGesture(Key.A), editor => {
                        EditingCommands.MoveRight.Execute(null, editor);
                        EditingCommands.SetMode.Execute(Insert, editor);
                    }
                },

                //
                // Deleting text
                //
                { new EditGesture(Key.D, ModifierKeys.Shift), editor => EditingCommands.DeleteLine.Execute(null, editor) },
                { new EditGesture(Key.D), editor => 
                    editor.WithMotion(range =>
                        EditingCommands.Delete.Execute(range, editor)) },

                //
                // Yanking text
                //
                { new EditGesture(Key.Y, ModifierKeys.Shift), editor => EditingCommands.YankLine.Execute(null, editor) },
                { new EditGesture(Key.Y), editor => 
                    editor.WithMotion(range =>
                        EditingCommands.Yank.Execute(range, editor)) },

                { new EditGesture(Key.Enter), editor => EditingCommands.RunCommand.Execute(null, editor) },
            }
        };
        public static Mode Insert = new Mode {
            DefaultInsert = true,
            Cursor = CursorType.Bar,
            Gestures = new Dictionary<InputGesture, Action<PadEditor>> {
                { new EditGesture(Key.Escape), editor => EditingCommands.SetMode.Execute(Command, editor) },
                { new EditGesture(Key.Enter), editor => EditingCommands.InsertNewline.Execute(null, editor) },
                { new EditGesture(Key.Back), editor => EditingCommands.DeleteBack.Execute(1, editor) },
            }
        };
    }
}
