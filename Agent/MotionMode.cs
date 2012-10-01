using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agent {
    public class MotionMode {
        public Dictionary<InputGesture, Func<PadEditor, Range>> Gestures { get; set; }
    }

    // TODO: How can we generalize these motions so we can use them alone?
    public static class DefaultMotions {
        public static MotionMode Command = new MotionMode {
            Gestures = new Dictionary<InputGesture, Func<PadEditor, Range>> {
                { new EditGesture(Key.Escape), editor => null },
                { new EditGesture(Key.W), editor => Movement.Word(editor) },
                { new EditGesture(Key.E), editor => Movement.WordEnd(editor) },
                { new EditGesture(Key.B), editor => Movement.WordBeginning(editor) },
                // Y and D are both shortcuts for the current line
                { new EditGesture(Key.D), editor =>
                        new Range(editor.Pad.Cursor.Row) {
                            EndColumn = editor.Pad.Lines[editor.Pad.Cursor.Row].Text.Length
                        }
                },
                { new EditGesture(Key.Y), editor =>
                        new Range(editor.Pad.Cursor.Row) {
                            EndColumn = editor.Pad.Lines[editor.Pad.Cursor.Row].Text.Length
                        }
                },
                { new EditGesture(Key.H), editor => Movement.Left(editor) },
                { new EditGesture(Key.L), editor => Movement.Right(editor) },
                { new EditGesture(Key.J), editor => {
                        var range = Movement.Down(editor);

                        range.StartColumn = 0;
                        range.EndColumn = editor.Pad.Lines[range.EndRow].Text.Length + 1;

                        return range;
                    }
                },
                { new EditGesture(Key.K), editor => {
                        var range = Movement.Up(editor);

                        range.StartColumn = editor.Pad.Lines[range.StartRow].Text.Length + 1;
                        range.EndColumn = 0;

                        return range;
                    }
                },
                { new EditGesture(Key.D4, ModifierKeys.Shift), editor =>
                    Movement.LineEnd(editor) },
                { new EditGesture(Key.D6, ModifierKeys.Shift), editor => 
                    Movement.LineHome(editor) },
            }
        };
    }
}
