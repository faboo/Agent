using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace Agent {
    public class PadEditor: Control {
        static PadEditor() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PadEditor), new FrameworkPropertyMetadata(typeof(PadEditor)));
        }

		public static readonly DependencyProperty PadProperty = DependencyProperty.Register("Pad", typeof(Pad), typeof(PadEditor), new FrameworkPropertyMetadata(OnPadChanged));

        private Mode currentMode = null;
        private bool defaultInsert = false;
        private Action<Range> withMotion = null;
        private Action<Char> withCharacter = null;
        private string count = null;
        private LinesPresenter linesPresenter = null;
        private int maxUndoLevels = 100;
        private List<IChange> undoLevels = new List<IChange>();
        private int? insertStarted = null;

        public PadEditor() {
            Script rc;

            IsTabStop = false;

            Commands = new AgentCommands();

            Pad = new Pad();

            CommandBindings.Add(new CommandBinding(EditingCommands.HandleKey, ExecuteHandleKey));

            CommandBindings.Add(new CommandBinding(EditingCommands.SetMode, ExecuteSetMode));
            CommandBindings.Add(new CommandBinding(EditingCommands.RunCommand, ExecuteRunCommand, CanRunCommand));
            CommandBindings.Add(new CommandBinding(EditingCommands.OpenFile, ExecuteOpenFile));

            CommandBindings.Add(new CommandBinding(EditingCommands.InsertNewline, ExecuteInsertNewline));
            CommandBindings.Add(new CommandBinding(EditingCommands.InsertText, ExecuteInsertText));
            CommandBindings.Add(new CommandBinding(EditingCommands.AppendText, ExecuteAppendText));

            CommandBindings.Add(new CommandBinding(EditingCommands.Move, ExecuteMove));

            CommandBindings.Add(new CommandBinding(EditingCommands.Yank, ExecuteYank));
            CommandBindings.Add(new CommandBinding(EditingCommands.Delete, ExecuteDelete));
            CommandBindings.Add(new CommandBinding(EditingCommands.Undo, ExecuteUndo, CanUndo));

            CommandBindings.Add(new CommandBinding(EditingCommands.Macro, ExecuteMacro));

            SetMode(DefaultModes.Command);

            MapResolver.MapAdded += OnMappingAdded;

            try {
                rc = new Script(Commands, Script.DefaultScript);
                rc.Run();
            }
            catch {
            }
        }

		public Pad Pad
		{
			get { return (Pad)GetValue(PadProperty); }
			set { SetValue(PadProperty, value); }
		}
        public AgentCommands Commands { get; private set; }
        public int? Count {
            get {
                int? value = null;

                if(!String.IsNullOrEmpty(count)) {
                    value = Int32.Parse(count);
                }

                return value;
            }
        }
        public int TopLine {
            get {
                return linesPresenter.LineOffset;
            }
        }
        public int BottomLine {
            get {
                return linesPresenter.LineOffset + linesPresenter.VisibleLines - 1;
            }
        }

        public void WithMotion(Action<Range> with) {
            InputBindings.Clear();

            foreach (var ghp in currentMode.Motions.Gestures)
                InputBindings.Add(
                    new InputBinding(EditingCommands.HandleKey, ghp.Key) {
                        CommandParameter = ghp.Value
                    });

            withMotion = with;
        }

        public void WithNextCharacter(Action<Char> with) {
            InputBindings.Clear();

            withCharacter = with;
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            linesPresenter = (LinesPresenter)Template.FindName("PART_Presenter", this);
            linesPresenter.ScrollChanged += OnScrolled;
        }

        public void AddToCount(string chars) {
            if(count == null)
                count = chars;
            else
                count += chars;
        }

        private void PushChange(IChange change) {
            undoLevels.Add(change);
            if(undoLevels.Count >= maxUndoLevels)
                undoLevels.RemoveAt(0);
        }

        private void UndoLastChange() {
            IChange change = undoLevels.Last();
            Cursor position = change.Do(this.Pad);

            Pad.Cursor.Row = position.Row;
            Pad.Cursor.Column = position.Column;

            undoLevels.RemoveAt(undoLevels.Count - 1);
        }

        private void OnPadChanged(DependencyPropertyChangedEventArgs args) {
            if(args.OldValue != null)
                (args.OldValue as Pad).Cursor.Changed -= OnCursorChanged;
            if(Pad != null)
                Pad.Cursor.Changed += new EventHandler(OnCursorChanged);
        }

        private void OnCursorChanged(object sender, EventArgs e)
        {
            linesPresenter.ScrollIntoView(Pad.CurrentLine);
        }

        private void OnScrolled(object sender, EventArgs args) {
            if(Pad.Cursor.Row < linesPresenter.LineOffset)
                Pad.Cursor.Row = linesPresenter.LineOffset;
            else if(Pad.Cursor.Row > linesPresenter.LineOffset + linesPresenter.VisibleLines)
                Pad.Cursor.Row = linesPresenter.LineOffset + linesPresenter.VisibleLines;
        }

        private void OnMappingAdded(Mode mode, InputGesture gesture, Action<PadEditor> action) {
            if(mode == currentMode)
                BindMacro(gesture, action);
        }

        private void SetMode(Mode mode) {
            currentMode = mode;
            defaultInsert = mode.DefaultInsert;
            Pad.Cursor.Type = mode.Cursor;

            RevertMode();
        }

        private void RevertMode() {
            InputBindings.Clear();

            foreach (var ghp in currentMode.Gestures)
                InputBindings.Add(
                    new InputBinding(EditingCommands.HandleKey, ghp.Key) {
                        CommandParameter = ghp.Value
                    });
            if(MapResolver.HasMacros(currentMode))
                foreach(var ghp in MapResolver.GetMacros(currentMode))
                    BindMacro(ghp.Key, ghp.Value);
        }

        private void BindMacro(InputGesture gesture, Action<PadEditor> action) {
            var oldBinding = InputBindings.OfType<InputBinding>().FirstOrDefault(i =>
                i.Gesture.Equals(gesture));

            if(oldBinding != null)
                InputBindings.Remove(oldBinding);
            InputBindings.Add(
                new InputBinding(EditingCommands.HandleKey, gesture) {
                    CommandParameter = action
                });
        }

        private string GetFile(int row, int column) {
            bool isFile = false;
            var fileRegex = new Regex(@"^\s*.:\\([^\\]+\\)*[^\\]+");
            var driveRegex = new Regex(@"^\s*.:\\$");
            var match = fileRegex.Match(Pad.Lines[row].Text.Substring(column));
            string file = null;

            if(match.Success) {
                file = match.Value;

                do {
                    isFile = File.Exists(file) || Directory.Exists(file);

                    if(!isFile)
                        file = System.IO.Path.GetDirectoryName(file);
                } while(!isFile && !driveRegex.IsMatch(file));
            }

            return isFile ? file : null;
        }

        protected override void OnTextInput(TextCompositionEventArgs args) {
            // if we're getting a motion and they type a non-motion key, abandon the motion
            if (withMotion != null) {
                RevertMode();
                withMotion = null;
                count = null;
            }
            // get the next character if there's an action to consume it
            else if(withCharacter != null) {
                RevertMode();
                withCharacter(args.Text[0]);
                withCharacter = null;
                count = null;
            }
            // do auto-insert for insert modes
            else if (defaultInsert) {
                if(insertStarted == null)
                    insertStarted = Pad.Column;

                Pad.CurrentLine.Text = Pad.CurrentLine.Text.Insert(
                    Pad.Column,
                    args.Text);
                Pad.Column += args.Text.Length;
                
                count = null;
            }
            else if(args.Text.All(c => Char.IsDigit(c))) {
                AddToCount(args.Text);
            }
            else{
                count = null;
            }
        }

        private void ExecuteSetMode(object sender, ExecutedRoutedEventArgs args) {
            if (args.Parameter is Mode)
                SetMode(args.Parameter as Mode);
        }

        private void ExecuteHandleKey(object sender, ExecutedRoutedEventArgs args) {
            // push any inserted text onto the undo stack
            if(insertStarted != null) {
                PushChange(new Remove(new Range(Pad.Row) {
                        StartColumn = (int)insertStarted,
                        EndColumn = Pad.Column,
                    }));
                insertStarted = null;
            }

            // handle regular commands
            if(args.Parameter is Action<PadEditor>){
                (args.Parameter as Action<PadEditor>)(this);
            }
            // handle motions
            else if(args.Parameter is Func<PadEditor, Range>) {
                if(withMotion != null) {
                    var motion = args.Parameter as Func<PadEditor, Range>;
                    var range = motion(this);

                    if(range != null)
                        withMotion(range);

                    withMotion = null;
                }

                RevertMode();
            }

            // either way, clear the count
            count = null;
        }

        private void ExecuteInsertNewline(object sender, ExecutedRoutedEventArgs args) {
            string nextLine = Pad.CurrentLine.Text.Substring(Pad.Cursor.Column);

            Pad.CurrentLine.Text = Pad.CurrentLine.Text.Substring(0, Pad.Cursor.Column);

            if (Pad.CurrentLine.Text.Contains(":")) {
                string maybeCommand = Pad.CurrentLine.Text.Split(':')[0];
                int row = Pad.Cursor.Row + 1;
                bool addedParameters = false;

                if (Commands.IsCommand(maybeCommand)) {
                    var parameters = Commands.GetParamaters(maybeCommand);

                    foreach(var param in parameters) {
                        Pad.InsertLine(row++, "  " + param + ":");
                        addedParameters = true;
                    }
                }

                if(!addedParameters || !String.IsNullOrWhiteSpace(nextLine)) {
                    nextLine = "  " + nextLine;
                    Pad.InsertLine(row, nextLine);
                }
            }
            else {
                Pad.InsertLine(Pad.Cursor.Row + 1, nextLine);
            }
            Pad.Cursor.Row += 1;
            Pad.Cursor.Column = Pad.CurrentLine.Text.Length;
        }

        private void ExecuteInsertText(object sender, ExecutedRoutedEventArgs args) {
            string text = args.Parameter as string;

            if (text.Contains('\n')) {
                var lines = text.Split('\n');
                int startRow = Pad.Cursor.Row;

                foreach(string line in lines.Take(lines.Count() - 1)) {
                    Pad.InsertLine(Pad.Cursor.Row, line);
                    Pad.Cursor.Row += 1;
                }

                PushChange(new Remove(new Range(startRow) {
                    EndRow = Pad.Cursor.Row - 1,
                    EndColumn = Pad.Lines[Pad.Cursor.Row - 1].Text.Length,
                }));
            }
            else {
                int column = Pad.Cursor.Column;

                Pad.CurrentLine.Text = Pad.CurrentLine.Text.Insert(
                    column,
                    text);
                PushChange(new Remove(new Range(Pad.Cursor.Row) {
                        StartColumn = column,
                        EndColumn = column + text.Length
                    }));
                Pad.Cursor.Column += text.Length;
            }
        }

        private void ExecuteAppendText(object sender, ExecutedRoutedEventArgs args) {
            string text = args.Parameter as string;

            if (text.Contains('\n')) {
                var lines = text.Split('\n');
                int startRow = Pad.Cursor.Row + 1;
                
                foreach(string line in lines.Take(lines.Count() - 1)) {
                    Pad.Cursor.Row += 1;
                    Pad.InsertLine(Pad.Cursor.Row, line);
                }

                PushChange(new Remove(new Range(startRow) {
                    EndRow = Pad.Cursor.Row,
                    EndColumn = Pad.Lines[Pad.Cursor.Row].Text.Length,
                }));
            }
            else {
                int column = Pad.Cursor.Column < Pad.CurrentLine.Text.Length ?
                        Pad.Cursor.Column + 1 :
                        Pad.Cursor.Column;

                Pad.CurrentLine.Text = Pad.CurrentLine.Text.Insert(
                    column,
                    text);
                PushChange(new Remove(new Range(Pad.Cursor.Row) {
                        StartColumn = column,
                        EndColumn = column + text.Length
                    }));
                Pad.Cursor.Column += text.Length;
            }
        }

        private void ExecuteYank(object sender, ExecutedRoutedEventArgs args) {
            Range range = args.Parameter as Range;

            Clipboard.SetText(Pad.GetText(range, false));
            Pad.Cursor.Row = range.StartRow;
            Pad.Cursor.Column = range.StartColumn;
        }

        private void ExecuteMove(object sender, ExecutedRoutedEventArgs args) {
            Cursor cursor = (Cursor)args.Parameter;

            Pad.Row = cursor.Row;
            Pad.Column = cursor.Column;
        }

        private void ExecuteDelete(object sender, ExecutedRoutedEventArgs args) {
            Range range = args.Parameter as Range;
            string text = Pad.GetText(range, true);

            Clipboard.SetText(text);
            PushChange(new Insert(
                new Cursor { Column = range.StartColumn, Row = range.StartRow },
                text));

            if(Pad.Cursor.Row < Pad.Lines.Count) {
                Pad.Cursor.Row = range.StartRow;
                Pad.Cursor.Column = range.StartColumn;
            }
            else {
                Pad.Cursor.Row = Pad.Lines.Count - 1;
                Pad.Cursor.Column = 0;
            }
        }

        private void ExecuteUndo(object sender, ExecutedRoutedEventArgs args) {
            UndoLastChange();
        }

        private void CanUndo(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = undoLevels.Count > 0;
        }

        private void ExecuteMacro(object sender, ExecutedRoutedEventArgs args) {
            string macro = args.Parameter as string;
            int count = Count ?? 1;

            this.count = null;

            for(; count > 0; count -= 1)
                foreach(var action in MapResolver.ResolveMacro(currentMode, macro))
                    action(this);
        }

        private void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs args) {
            int column = Pad.Column;
            string file = null;

            while(column >= 0 && (file = GetFile(Pad.Row, column)) == null)
                column -= 1;

            if(file != null) {
                using(Process launcher = new Process()) {
                    launcher.StartInfo = new ProcessStartInfo {
                        ErrorDialog = true,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(file),
                        FileName = file,
                    };

                    launcher.Start();
                }
            }
        }

        private void ExecuteRunCommand(object sender, ExecutedRoutedEventArgs args) {
            Dictionary<string, string> commandArgs = new Dictionary<string,string>();
            int start = Pad.Cursor.Row;
            Range size = null;
            string result = null;

            while (start > 0
                    && Pad.Lines[start].Text.Contains(":")
                    && Pad.Lines[start].Text.StartsWith(" "))
                start -= 1;

            Commands.ParseAndRun(Pad.Lines.Select(l => l.Text).ToList(), start, out size, out result);

            if (result != null) {
                EditingCommands.Delete.Execute(size, this);
                EditingCommands.InsertText.Execute(result, this);
            }
        }

        private void CanRunCommand(object sender, CanExecuteRoutedEventArgs args) {
            int start = Pad.Cursor.Row;

            while (Pad.CurrentLine.Text.Contains(":")
                    && Pad.Lines[start].Text.StartsWith(" ")
                    && start > 0)
                start -= 1;

            args.CanExecute = Pad.Lines[start].Text.Contains(":")
                && !Pad.Lines[start].Text.StartsWith(" ")
                && Commands.IsCommand(Pad.Lines[start].Text.Split(':')[0].Trim());
        }

        private static void OnPadChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            (sender as PadEditor).OnPadChanged(args);
        }
    }
}
