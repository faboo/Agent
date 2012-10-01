using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Agent {
    public class Mode {
        public bool DefaultInsert { get; set; }
        public CursorType Cursor { get; set; }
        public IDictionary<InputGesture, Action<PadEditor>> Gestures { get; set; }
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
                { new EditGesture(Key.H), editor => 
                    EditingCommands.Move.Execute(new Cursor(Movement.Left(editor)), editor) },
                { new EditGesture(Key.L), editor => 
                    EditingCommands.Move.Execute(new Cursor(Movement.Right(editor)), editor) },
                { new EditGesture(Key.K), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.Up(editor)), editor) },
                { new EditGesture(Key.J), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.Down(editor)), editor) },
                { new EditGesture(Key.D4, ModifierKeys.Shift), editor => 
                    EditingCommands.Move.Execute(new Cursor(Movement.LineEnd(editor)), editor) },
                { new EditGesture(Key.D6, ModifierKeys.Shift), editor => 
                    EditingCommands.Move.Execute(new Cursor(Movement.LineHome(editor)), editor) },
                { new EditGesture(Key.L, ModifierKeys.Shift), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.Bottom(editor)), editor) },
                { new EditGesture(Key.H, ModifierKeys.Shift), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.Top(editor)), editor) },
                { new EditGesture(Key.M, ModifierKeys.Shift), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.Middle(editor)), editor) },
                { new EditGesture(Key.W), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.Word(editor)), editor) },
                { new EditGesture(Key.E), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.WordEnd(editor)), editor) },
                { new EditGesture(Key.B), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.WordBeginning(editor)), editor) },
                { new EditGesture(Key.G, ModifierKeys.Shift), editor =>
                    EditingCommands.Move.Execute(new Cursor(Movement.Goto(editor)), editor) },

                //
                // Pasting text
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
                        EditingCommands.Move.Execute(new Cursor(Movement.Right(editor)), editor);
                        EditingCommands.SetMode.Execute(Insert, editor);
                    }
                },
                { new EditGesture(Key.G), editor => EditingCommands.SetMode.Execute(Go, editor) },

                //
                // Deleting text
                //
                { new EditGesture(Key.D, ModifierKeys.Shift), editor => 
                    EditingCommands.Delete.Execute(Movement.LineEnd(editor), editor) },
                { new EditGesture(Key.D), editor => 
                    editor.WithMotion(range =>
                        EditingCommands.Delete.Execute(range, editor)) },
                { new EditGesture(Key.X), editor => 
                    EditingCommands.Delete.Execute(Movement.Right(editor), editor) },

                //
                // Yanking text
                //
                { new EditGesture(Key.Y, ModifierKeys.Shift), editor =>                    
                    EditingCommands.Yank.Execute(Movement.LineEnd(editor), editor) },
                { new EditGesture(Key.Y), editor => 
                    editor.WithMotion(range =>
                        EditingCommands.Yank.Execute(range, editor)) },

                { new EditGesture(Key.Enter), editor => EditingCommands.RunCommand.Execute(null, editor) },
            }
        };

        public static Mode Go = new Mode {
            DefaultInsert = false,
            Cursor = CursorType.Box,
            Motions = DefaultMotions.Command,
            Gestures = new Dictionary<InputGesture, Action<PadEditor>> {
                { new EditGesture(Key.Escape), editor => EditingCommands.SetMode.Execute(Command, editor) },

                { new EditGesture(Key.G), editor => {
                        EditingCommands.Move.Execute(new Cursor(Movement.Goto(editor, 0)), editor);
                        EditingCommands.SetMode.Execute(Command, editor);
                    }
                },
            }
        };

        public static Mode Insert = new Mode {
            DefaultInsert = true,
            Cursor = CursorType.Bar,
            Gestures = new Dictionary<InputGesture, Action<PadEditor>> {
                { new EditGesture(Key.Escape), editor => EditingCommands.SetMode.Execute(Command, editor) },
                { new EditGesture(Key.Enter), editor => EditingCommands.InsertNewline.Execute(null, editor) },
                { new EditGesture(Key.Back), editor => EditingCommands.Delete.Execute(Movement.Left(editor), editor) },
                { new EditGesture(Key.Delete), editor => EditingCommands.Delete.Execute(Movement.Right(editor), editor) },
            }
        };
    }
}
