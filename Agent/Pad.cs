using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data.SqlServerCe;

namespace Agent {
    public class Pad : Freezable {
		public static readonly DependencyProperty LinesProperty = DependencyProperty.Register("Lines", typeof(ObservableCollection<Line>), typeof(Pad));
        public static readonly DependencyProperty CursorProperty = DependencyProperty.Register("Cursor", typeof(Cursor), typeof(Pad), new FrameworkPropertyMetadata(OnCursorChanged));

        private int currentRow = -1;

        public Pad() {
            Lines = new ObservableCollection<Line>{ new Line { Text = "" } };
            Cursor = new Cursor { Column = 0, Row = 0, Type = CursorType.Box };

            if (!DataStore.TableExists("pad"))
                MakeTable();
            Load();

            Lines[0].Cursor = Cursor;
        }

        public ObservableCollection<Line> Lines
		{
			get { return (ObservableCollection<Line>)GetValue(LinesProperty); }
			set { SetValue(LinesProperty, value); }
		}
		public Cursor Cursor
		{
			get { return (Cursor)GetValue(CursorProperty); }
			set { SetValue(CursorProperty, value); }
		}
        public int Column {
            get { return Cursor.Column; }
            set {
                if(value < 0)
                    value = 0;
                else if(value > CurrentLine.Text.Length)
                    value = CurrentLine.Text.Length;
                Cursor.Column = value;
            }
        }
        public int Row {
            get { return Cursor.Row; }
            set {
                if(value < 0)
                    value = 0;
                else if(value >= Lines.Count)
                    value = Lines.Count - 1;
                Cursor.Row = value;
            }
        }
        public Line CurrentLine {
            get { return Lines[Cursor.Row]; }
        }

        private void MakeTable() {
            var command = DataStore.Connection.CreateCommand();

            command.CommandText = "CREATE TABLE [pad] "
                + "([id] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY, [text] NVARCHAR(256) NOT NULL)";

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Load() {
            var command = DataStore.Connection.CreateCommand();
            SqlCeDataReader reader = null;

            command.CommandText = "SELECT [text] FROM [pad] ORDER BY [id]";

            Lines.Clear();

            reader = command.ExecuteReader();

            while(reader.Read())
                Lines.Add(new Line { Text = reader.GetString(0) });

            if(Lines.Count == 0 || Lines.Last().Text != "")
                Lines.Add(new Line { Text = "" });

            reader.Dispose();
            command.Dispose();
        }

        public void Save() {
            using (var transaction = DataStore.Connection.BeginTransaction()) {
                var command = DataStore.Connection.CreateCommand();

                command.CommandText = "DELETE FROM [pad]";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO [pad] ([text]) VALUES (?)";

                foreach (var line in Lines) {
                    command.Parameters.Clear();
                    command.Parameters.Add(new SqlCeParameter("text", line.Text));
                    command.ExecuteNonQuery();
                }

                command.Dispose();
                transaction.Commit();
            }
        }

        public void InsertLine(int at, string text) {
            if (at < Lines.Count && Cursor.Row < Lines.Count)
                Lines[Cursor.Row].Cursor = null;
            Lines.Insert(at, new Line { Text = text });

            if(Cursor.Row >= Lines.Count)
                Cursor.Row = Lines.Count - 1;
            else
                Lines[Cursor.Row].Cursor = Cursor;
        }

        public void RemoveLine(int at) {
            if (Cursor.Row == at)
                CurrentLine.Cursor = null;
            Lines.RemoveAt(at);
            if (Lines.Count == 0)
                Lines.Add(new Line { Text = "" });
            if (!(Cursor.Row < Lines.Count))
                Cursor.Row = Lines.Count - 1;
            if (Cursor.Row == at)
                CurrentLine.Cursor = Cursor;
        }

        public void InsertText(Cursor cursor, string text) {
            if(text.Contains('\n')) {
                var lines = text.Split('\n');
                foreach(string line in lines.Take(lines.Count() - 1)) {
                    InsertLine(cursor.Row, line);
                    cursor.Row += 1;
                }
            }
            else {
                Lines[cursor.Row].Text = Lines[cursor.Row].Text.Insert(
                        cursor.Column,
                        text);
                cursor.Column += text.Length;
            }
        }

        public string GetText(Range range, bool delete) {
            string text = "";

            range.Normalize();

            // if we're doing a linewise (-ish) get
            if(range.EndRow != range.StartRow || range.EndColumn == Lines[range.EndRow].Text.Length) {

                for(int row = range.StartRow; row <= range.EndRow && row < Lines.Count; /* see the end */) {
                    Line line = Lines[row];
                    int startColumn = row == range.StartRow ? range.StartColumn : 0;
                    int endColumn = row == range.EndRow ? range.EndColumn : line.Text.Length;
                    int length = endColumn - startColumn + 1;

                    if(length + startColumn > line.Text.Length)
                        length = line.Text.Length - startColumn;

                    text += line.Text.Substring(startColumn, length) + "\n";

                    if(startColumn == 0 && endColumn == line.Text.Length) {
                        if(delete)
                            Lines.RemoveAt(row);
                        range.EndRow -= 1;
                    }
                    else {
                        if(delete)
                            line.Text = line.Text.Remove(startColumn, length);
                        row += 1;
                    }
                }
            }
            // if we're doing a characterwise get
            else {
                Line line = Lines[range.StartRow];
                int length = range.EndColumn - range.StartColumn;

                if(length + range.StartColumn > line.Text.Length)
                    length = line.Text.Length - range.StartColumn;

                if(length > 0) {
                    text += line.Text.Substring(range.StartColumn, length);
                    if(delete)
                        line.Text = line.Text.Remove(range.StartColumn, length);
                }
            }

            OnCursorModified(null, new EventArgs());

            return text;
        }

        private void OnCursorChanged() {
            if (Cursor != null) {
                Cursor.Changed += OnCursorModified;
                OnCursorModified(null, new EventArgs());
            }
        }

        private void OnCursorModified(object sender, EventArgs args) {
            if(currentRow >= 0 && currentRow < Lines.Count)
                Lines[currentRow].Cursor = null;
            Lines[Cursor.Row].Cursor = Cursor;
            currentRow = Cursor.Row;
            if (Cursor.Column > Lines[Cursor.Row].Text.Length)
                Cursor.Column = Lines[Cursor.Row].Text.Length;
        }

        private static void OnCursorChanged(object sender, DependencyPropertyChangedEventArgs args) {
            (sender as Pad).OnCursorChanged();
        }

        protected override Freezable CreateInstanceCore() {
            return new Pad();
        }
    }
}
