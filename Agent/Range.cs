using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent {
    public class Range {
        public Range(Cursor start) {
            StartRow = start.Row;
            StartColumn = start.Column;
            EndRow = start.Row;
            EndColumn = 0;
        }

        public Range(int startRow) {
            StartRow = startRow;
            StartColumn = 0;
            EndRow = startRow;
            EndColumn = 0;
        }

        public int StartRow { get; set; }
        public int StartColumn { get; set; }
        public int EndRow { get; set; }
        public int EndColumn { get; set; }

        public void Normalize(){
            if(EndRow < StartRow) {
                int swap = StartRow;

                StartRow = EndRow;
                EndRow = swap;

                swap = StartColumn;
                StartColumn = EndColumn;
                EndColumn = swap;
            }
        }
    }
}
