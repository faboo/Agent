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
                // Searching
                //
                { new EditGesture(Key.F), editor =>
                    editor.WithNextCharacter(c => {
                        string rest = editor.Pad.CurrentLine.Text.Substring(editor.Pad.Column);
                        int location = rest.IndexOf(c);
                        if(location != -1)
                            EditingCommands.Move.Execute(new Cursor {
                                    Row = editor.Pad.Row,
                                    Column = editor.Pad.Column += location
                                },
                                editor);
                    })
                },
                { new EditGesture(Key.F, ModifierKeys.Shift), editor =>
                    editor.WithNextCharacter(c => {
                        string rest = editor.Pad.CurrentLine.Text.Substring(0, editor.Pad.Column);
                        int location = rest.LastIndexOf(c);
                        if(location != -1)
                            EditingCommands.Move.Execute(new Cursor {
                                    Row = editor.Pad.Row,
                                    Column = location
                                },
                                editor);
                    })
                },
                { new EditGesture(Key.T), editor =>
                    editor.WithNextCharacter(c => {
                        if(editor.Pad.Column + 1 < editor.Pad.CurrentLine.Text.Length){
                            string rest = editor.Pad.CurrentLine.Text.Substring(editor.Pad.Column + 1);
                            int location = rest.IndexOf(c);
                            if(location != -1)
                                EditingCommands.Move.Execute(new Cursor {
                                        Row = editor.Pad.Row,
                                        Column = editor.Pad.Column += location
                                    },
                                    editor);
                        }
                    })
                },
                { new EditGesture(Key.T, ModifierKeys.Shift), editor =>
                    editor.WithNextCharacter(c => {
                        if(editor.Pad.Column > 0){
                            string rest = editor.Pad.CurrentLine.Text.Substring(0, editor.Pad.Column - 1);
                            int location = rest.LastIndexOf(c);
                            if(location != -1)
                                EditingCommands.Move.Execute(new Cursor {
                                        Row = editor.Pad.Row,
                                        Column = location + 1
                                    },
                                    editor);
                        }
                    })
                },


                //
                // Pasting text
                //
                { new EditGesture(Key.O, ModifierKeys.Shift), editor => {
                        EditingCommands.InsertText.Execute("\n", editor);
                        EditingCommands.Move.Execute(new Cursor(Movement.Up(editor)), editor);
                        EditingCommands.SetMode.Execute(DefaultModes.Insert, editor);
                    }
                },
                { new EditGesture(Key.O), editor => {
                        EditingCommands.AppendText.Execute("\n", editor);
                        EditingCommands.SetMode.Execute(DefaultModes.Insert, editor);
                    }
                },
                { new EditGesture(Key.P, ModifierKeys.Shift), editor => {
                        if (Clipboard.ContainsText(TextDataFormat.Text))
                            for(int times = editor.Count == null ? 1 : (int)editor.Count;
                                    times > 0; times -= 1)
                                EditingCommands.InsertText.Execute(Clipboard.GetText(TextDataFormat.Text), editor);
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
                { new EditGesture(Key.I, ModifierKeys.Shift), editor => {
                        EditingCommands.Move.Execute(new Cursor(Movement.LineHome(editor)), editor);
                        EditingCommands.SetMode.Execute(Insert, editor);
                    }
                },
                { new EditGesture(Key.A), editor => {
                        EditingCommands.Move.Execute(new Cursor(Movement.Right(editor)), editor);
                        EditingCommands.SetMode.Execute(Insert, editor);
                    }
                },
                { new EditGesture(Key.A, ModifierKeys.Shift), editor => {
                        EditingCommands.Move.Execute(new Cursor(Movement.LineEnd(editor)), editor);
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

                //
                // Undo
                //
                { new EditGesture(Key.U), editor =>
                    EditingCommands.Undo.Execute(null, editor) },

                //
                // Replacing
                //
                { new EditGesture(Key.R), editor => {
                        var range = Movement.Right(editor);
                        editor.WithNextCharacter(character => {
                                EditingCommands.Delete.Execute(range, editor);
                                EditingCommands.InsertText.Execute(
                                    character.Repeat(range.EndColumn - range.StartColumn),
                                    editor);
                            });
                    }
                },
                { new EditGesture(Key.C), editor =>
                    editor.WithMotion(range => {
                        EditingCommands.Delete.Execute(range, editor);
                        EditingCommands.SetMode.Execute(DefaultModes.Insert, editor);
                    })
                },
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
                { new EditGesture(Key.F), editor => EditingCommands.OpenFile.Execute(null, editor) },
            }
        };

        public static Mode Insert = new Mode {
            DefaultInsert = true,
            Cursor = CursorType.Bar,
            Gestures = new Dictionary<InputGesture, Action<PadEditor>> {
                { new EditGesture(Key.Escape), editor => EditingCommands.SetMode.Execute(Command, editor) },
                { new EditGesture(Key.OemOpenBrackets, ModifierKeys.Control), editor => EditingCommands.SetMode.Execute(Command, editor) },
                { new EditGesture(Key.Enter), editor => EditingCommands.InsertNewline.Execute(null, editor) },
                { new EditGesture(Key.Back), editor => EditingCommands.Delete.Execute(Movement.Left(editor), editor) },
                { new EditGesture(Key.Delete), editor => EditingCommands.Delete.Execute(Movement.Right(editor), editor) },
            }
        };
    }
}
