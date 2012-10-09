using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent {
    public static class Movement {
        public static Range Word(PadEditor editor) {
            int words = editor.Count ?? 1;
            Range range = new Range(editor.Pad.Cursor);

            range.EndColumn = range.StartColumn;

            for(; words > 0; words -= 1) {
                // move to the first non-space
                while(range.EndColumn < editor.Pad.CurrentLine.Text.Length
                        && Char.IsWhiteSpace(editor.Pad.CurrentLine.Text[range.EndColumn]))
                    ++range.EndColumn;

                while(range.EndColumn < editor.Pad.CurrentLine.Text.Length
                        && !Char.IsWhiteSpace(
                            editor.Pad.CurrentLine.Text[range.EndColumn]))
                    ++range.EndColumn;
            }

            return range;
        }

        public static Range WordEnd(PadEditor editor) {
            int words = editor.Count ?? 1;
            Range range = new Range(editor.Pad.Cursor);

            range.EndColumn = range.StartColumn;

            for(; words > 0; words -= 1) {
                range.EndColumn += 1;

                // move to the first non-space
                while(range.EndColumn < editor.Pad.CurrentLine.Text.Length
                        && Char.IsWhiteSpace(editor.Pad.CurrentLine.Text[range.EndColumn]))
                    ++range.EndColumn;

                while(range.EndColumn < editor.Pad.CurrentLine.Text.Length
                        && !Char.IsWhiteSpace(
                            editor.Pad.CurrentLine.Text[range.EndColumn + 1]))
                    ++range.EndColumn;
            }

            return range;
        }

        public static Range WordBeginning(PadEditor editor) {
            int words = editor.Count ?? 1;
            Range range = new Range(editor.Pad.Cursor) {
                EndColumn = editor.Pad.Column
            };

            for( ; words > 0; words -= 1) {
                range.EndColumn -= 1;

                // move to the first non-space
                while(range.EndColumn > 0
                        && Char.IsWhiteSpace(editor.Pad.CurrentLine.Text[range.EndColumn]))
                    --range.EndColumn;

                while(range.EndColumn > 0
                        && !Char.IsWhiteSpace(
                            editor.Pad.CurrentLine.Text[range.EndColumn - 1]))
                    --range.EndColumn;
            }

            return range;
        }

        public static Range Left(PadEditor editor) {
            int chars = editor.Count ?? 1;

            return new Range(editor.Pad.Cursor) {
                StartColumn = editor.Pad.Column,
                EndColumn = editor.Pad.Column - chars
            };
        }

        public static Range Right(PadEditor editor) {
            int chars = editor.Count ?? 1;

            return new Range(editor.Pad.Cursor) {
                EndColumn = editor.Pad.Column + chars
            };
        }

        public static Range Down(PadEditor editor) {
            int lines = editor.Count ?? 1;
            int endRow = editor.Pad.Row + lines;

            if(endRow >= editor.Pad.Lines.Count)
                endRow = editor.Pad.Lines.Count - 1;

            return new Range(editor.Pad.Cursor) {
                EndRow = endRow,
                EndColumn = editor.Pad.Column
            };
        }

        public static Range Up(PadEditor editor) {
            int lines = editor.Count ?? 1;
            int endRow = editor.Pad.Row - lines;

            if(endRow < 0)
                endRow = 0;

            return new Range(editor.Pad.Cursor) {
                EndRow = endRow,
                EndColumn = editor.Pad.Column
            };
        }

        public static Range Top(PadEditor editor) {
            return new Range(editor.Pad.Cursor) {
                EndRow = editor.TopLine,
                EndColumn = editor.Pad.Cursor.Column
            };
        }

        public static Range Bottom(PadEditor editor) {
            return new Range(editor.Pad.Cursor) {
                EndRow = editor.BottomLine,
                EndColumn = editor.Pad.Cursor.Column
            };
        }

        public static Range Middle(PadEditor editor) {
            return new Range(editor.Pad.Cursor) {
                EndRow = editor.TopLine + (editor.BottomLine - editor.TopLine)/2 + 1,
                EndColumn = 0
            };
        }

        public static Range LineEnd(PadEditor editor) {
            return new Range(editor.Pad.Cursor) {
                EndColumn = editor.Pad.CurrentLine.Text.Length + 1
            };
        }

        public static Range LineHome(PadEditor editor) {
            return new Range(editor.Pad.Cursor) {
                EndColumn = 0
            };
        }

        public static Range Goto(PadEditor editor, int? defaultLine = null) {
            return new Range(editor.Pad.Cursor) {
                EndRow = editor.Count ?? (defaultLine ?? editor.Pad.Lines.Count - 1),
                EndColumn = editor.Pad.Cursor.Column
            };
        }
    }
}
