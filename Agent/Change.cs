using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent {
    public interface IChange {
        Cursor Do(Pad pad);
    }

    public class Insert: IChange {
        private string text;
        private Cursor position;

        public Insert(Cursor position, string text) {
            this.position = position.Clone() as Cursor;
            this.text = text;
        }

        public Cursor Do(Pad pad) {
            Cursor position = (Cursor)this.position.Clone();

            pad.InsertText(position, text);

            return position;
        }
    }

    public class Remove : IChange {
        private Range range;
        public Remove(Range range) {
            this.range = range;
        }

        public Cursor Do(Pad pad) {
            pad.GetText(range, true);

            return new Cursor {
                Row = range.StartRow,
                Column = range.StartColumn
            };
        }
    }
}
