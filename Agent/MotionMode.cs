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
                { new EditGesture(Key.W), editor => {
                        Range range = new Range(editor.Pad.Cursor);

                        range.EndRow = range.StartRow;
                        range.EndColumn = range.StartColumn;

                        for(int words = editor.Count == null ? 1 : (int)editor.Count;
                                words > 0; words -= 1){
                            // move to the first non-space
                            while(range.EndColumn < editor.Pad.CurrentLine.Text.Length
                                    && Char.IsWhiteSpace(editor.Pad.CurrentLine.Text[range.EndColumn]))
                                ++range.EndColumn;

                            while (range.EndColumn < editor.Pad.CurrentLine.Text.Length
                                    && !Char.IsWhiteSpace(
                                        editor.Pad.CurrentLine.Text[range.EndColumn]))
                                ++range.EndColumn;
                        }

                        return range;
                    }
                },
                { new EditGesture(Key.E), editor => {
                        Range range = new Range(editor.Pad.Cursor);

                        range.EndRow = range.StartRow;
                        range.EndColumn = range.StartColumn;

                        for(int words = editor.Count == null ? 1 : (int)editor.Count;
                                words > 0; words -= 1){
                            // move to the first non-space
                            while(range.EndColumn < editor.Pad.CurrentLine.Text.Length
                                    && Char.IsWhiteSpace(editor.Pad.CurrentLine.Text[range.EndColumn]))
                                ++range.EndColumn;

                            while (range.EndColumn < editor.Pad.CurrentLine.Text.Length
                                    && !Char.IsWhiteSpace(
                                        editor.Pad.CurrentLine.Text[range.EndColumn + 1]))
                                ++range.EndColumn;
                        }

                        return range;
                    }
                },
                { new EditGesture(Key.B), editor => {
                        Range range = new Range(editor.Pad.Cursor) {
                            EndColumn = editor.Pad.Cursor.Column
                        };
                        
                        for(int words = editor.Count == null ? 1 : (int)editor.Count;
                                words > 0; words -= 1){
                            // move to the first non-space
                            while(range.StartColumn > 0
                                    && Char.IsWhiteSpace(editor.Pad.CurrentLine.Text[range.StartColumn]))
                                --range.StartColumn;

                            while (range.StartColumn > 0
                                    && !Char.IsWhiteSpace(
                                        editor.Pad.CurrentLine.Text[range.StartColumn - 1]))
                                --range.StartColumn;
                        }

                        return range;
                    }
                },
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
                { new EditGesture(Key.J), editor => {
                        int endRow = editor.Pad.Cursor.Row + 
                            (editor.Count != null? (int)editor.Count : 1);

                        if(endRow >= editor.Pad.Lines.Count)
                            endRow = editor.Pad.Lines.Count - 1;

                        return new Range(editor.Pad.Cursor.Row) {
                            EndRow = endRow,
                            EndColumn = editor.Pad.Lines[endRow].Text.Length
                        };
                    }
                },
                { new EditGesture(Key.K), editor => {
                        int endRow = editor.Pad.Cursor.Row - 
                            (editor.Count != null? (int)editor.Count : 1);

                        if(endRow < 0)
                            endRow = 0;

                        return new Range(editor.Pad.Cursor.Row) {
                            StartColumn = editor.Pad.CurrentLine.Text.Length + 1,
                            EndRow = endRow,
                            EndColumn = 0
                        };
                    }
                },
            }
        };
    }
}
