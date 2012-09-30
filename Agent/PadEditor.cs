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

namespace Agent {
    public class PadEditor: Control {
        static PadEditor() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PadEditor), new FrameworkPropertyMetadata(typeof(PadEditor)));
        }

		public static readonly DependencyProperty PadProperty = DependencyProperty.Register("Pad", typeof(Pad), typeof(PadEditor), new FrameworkPropertyMetadata(OnPadChanged));

        private Mode currentMode = null;
        private bool defaultInsert = false;
        private Action<Range> withMotion = null;
        private string count = null;
        private LinesPresenter linesPresenter = null;

        public PadEditor() {
            IsTabStop = false;

            Commands = new AgentCommands();

            Pad = new Pad();

            CommandBindings.Add(new CommandBinding(EditingCommands.HandleKey, ExecuteHandleKey));

            CommandBindings.Add(new CommandBinding(EditingCommands.SetMode, ExecuteSetMode));
            CommandBindings.Add(new CommandBinding(EditingCommands.RunCommand, ExecuteRunCommand, CanRunCommand));

            CommandBindings.Add(new CommandBinding(EditingCommands.InsertNewline, ExecuteInsertNewline));
            CommandBindings.Add(new CommandBinding(EditingCommands.AppendNewline, ExecuteAppendNewline));
            CommandBindings.Add(new CommandBinding(EditingCommands.PrependNewline, ExecutePrependNewline));
            CommandBindings.Add(new CommandBinding(EditingCommands.PrependText, ExecutePrependText));
            CommandBindings.Add(new CommandBinding(EditingCommands.AppendText, ExecuteAppendText));

            CommandBindings.Add(new CommandBinding(EditingCommands.YankLine, ExecuteYankLine));
            CommandBindings.Add(new CommandBinding(EditingCommands.Yank, ExecuteYank));

            CommandBindings.Add(new CommandBinding(EditingCommands.MoveLeft, ExecuteMoveLeft, CanMoveLeft));
            CommandBindings.Add(new CommandBinding(EditingCommands.MoveRight, ExecuteMoveRight, CanMoveRight));
            CommandBindings.Add(new CommandBinding(EditingCommands.MoveUp, ExecuteMoveUp, CanMoveUp));
            CommandBindings.Add(new CommandBinding(EditingCommands.MoveDown, ExecuteMoveDown, CanMoveDown));
            CommandBindings.Add(new CommandBinding(EditingCommands.MoveEnd, ExecuteMoveEnd, CanMoveEnd));
            CommandBindings.Add(new CommandBinding(EditingCommands.MoveHome, ExecuteMoveHome, CanMoveHome));

            CommandBindings.Add(new CommandBinding(EditingCommands.DeleteBack, ExecuteDeleteBack, CanDeleteBack));
            CommandBindings.Add(new CommandBinding(EditingCommands.Delete, ExecuteDelete));
            CommandBindings.Add(new CommandBinding(EditingCommands.DeleteLine, ExecuteDeleteLine));

            SetMode(DefaultModes.Command);
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

                if(count != null) {
                    value = Int32.Parse(count);
                }

                return value;
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

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            linesPresenter = (LinesPresenter)Template.FindName("PART_Presenter", this);
            linesPresenter.ScrollChanged += OnScrolled;
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
        }

        private char FixCase(KeyboardDevice keyboard, char ch) {
            if (!(keyboard.IsKeyDown(Key.LeftShift) ||
                    keyboard.IsKeyDown(Key.RightShift)))
                ch = Char.ToLower(ch, CultureInfo.CurrentUICulture);

            return ch;
        }

        protected override void OnTextInput(TextCompositionEventArgs args) {
            // do auto-insert for insert modes
            if (defaultInsert) {
                Pad.Lines[Pad.Cursor.Row].Text = Pad.Lines[Pad.Cursor.Row].Text.Insert(
                    Pad.Cursor.Column,
                    args.Text);
                Pad.Cursor.Column += args.Text.Length;
                count = null;
            }
            else if(args.Text.All(c => Char.IsDigit(c))) {
                if(count == null)
                    count = args.Text;
                else
                    count += args.Text;
            }
            // if we're getting a motion and they type a non-motion key, abandon the motion
            else if (withMotion != null) {
                RevertMode();
                withMotion = null;
                count = null;
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

                if (Commands.IsCommand(maybeCommand)) {
                    var parameters = Commands.GetParamaters(maybeCommand);

                    foreach (var param in parameters)
                        Pad.InsertLine(row++, "  " + param + ":");
                }

                nextLine = "  " + nextLine;
                Pad.InsertLine(row, nextLine);
            }
            else {
                Pad.InsertLine(Pad.Cursor.Row + 1, nextLine);
            }
            Pad.Cursor.Row += 1;
            Pad.Cursor.Column = Pad.CurrentLine.Text.Length;
        }

        private void ExecuteAppendNewline(object sender, ExecutedRoutedEventArgs args) {
            Pad.InsertLine(Pad.Cursor.Row + 1, "");
            Pad.Cursor.Row += 1;
            Pad.Cursor.Column = 0;
        }

        private void ExecutePrependNewline(object sender, ExecutedRoutedEventArgs args) {
            Pad.InsertLine(Pad.Cursor.Row, "");
            Pad.Cursor.Column = 0;
        }

        private void ExecutePrependText(object sender, ExecutedRoutedEventArgs args) {
            string text = args.Parameter as string;

            if (text.Contains('\n')) {
                var lines = text.Split('\n');
                foreach(string line in lines.Take(lines.Count() - 1)) {
                    Pad.InsertLine(Pad.Cursor.Row, line);
                    Pad.Cursor.Row += 1;
                }
            }
            else {
                Pad.CurrentLine.Text = Pad.CurrentLine.Text.Insert(Pad.Cursor.Column, text);
                Pad.Cursor.Column += text.Length;
            }
        }

        private void ExecuteAppendText(object sender, ExecutedRoutedEventArgs args) {
            string text = args.Parameter as string;

            if (text.Contains('\n')) {
                var lines = text.Split('\n');
                foreach(string line in lines.Take(lines.Count() - 1)) {
                    Pad.InsertLine(Pad.Cursor.Row + 1, line);
                    Pad.Cursor.Row += 1;
                }
            }
            else {
                Pad.CurrentLine.Text = Pad.CurrentLine.Text.Insert(
                    Pad.Cursor.Column < Pad.CurrentLine.Text.Length?
                        Pad.Cursor.Column+1 :
                        Pad.Cursor.Column,
                    text);
                Pad.Cursor.Column += text.Length;
            }
        }

        private void ExecuteYank(object sender, ExecutedRoutedEventArgs args) {
            Range range = args.Parameter as Range;

            Clipboard.SetText(Pad.GetText(range, false));
            Pad.Cursor.Row = range.StartRow;
            Pad.Cursor.Column = range.StartColumn;
        }

        private void ExecuteYankLine(object sender, ExecutedRoutedEventArgs args) {
            Clipboard.SetText(Pad.CurrentLine.Text);
        }

        private void ExecuteMoveLeft(object sender, ExecutedRoutedEventArgs args) {
            Pad.Column -= args.Parameter == null? 1 : (int)args.Parameter;
        }

        private void CanMoveLeft(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = Pad.Column > 0;
        }

        private void ExecuteMoveRight(object sender, ExecutedRoutedEventArgs args) {
            Pad.Column += args.Parameter == null ? 1 : (int)args.Parameter;
        }

        private void CanMoveRight(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = Pad.Column < Pad.Lines[Pad.Cursor.Row].Text.Length;
        }

        private void ExecuteMoveUp(object sender, ExecutedRoutedEventArgs args) {
            Pad.Row -= args.Parameter == null ? 1 : (int)args.Parameter;
        }

        private void CanMoveUp(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = Pad.Row > 0;
        }

        private void ExecuteMoveDown(object sender, ExecutedRoutedEventArgs args) {
            Pad.Cursor.Row += args.Parameter == null ? 1 : (int)args.Parameter;
        }

        private void CanMoveDown(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = Pad.Cursor.Row+1 < Pad.Lines.Count;
        }

        private void ExecuteMoveEnd(object sender, ExecutedRoutedEventArgs args) {
            Pad.Cursor.Column = Pad.CurrentLine.Text.Length;
        }

        private void CanMoveEnd(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = true;
        }

        private void ExecuteMoveHome(object sender, ExecutedRoutedEventArgs args) {
            Pad.Cursor.Column = 0;
        }

        private void CanMoveHome(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = true;
        }

        private void ExecuteDeleteBack(object sender, ExecutedRoutedEventArgs args) {
            int characters = args.Command == null? 1 : (int)args.Parameter;

            if (characters > Pad.Cursor.Column)
                characters = Pad.Cursor.Column;

            Clipboard.SetText(Pad.CurrentLine.Text.Substring(Pad.Cursor.Column - characters, characters));
            Pad.CurrentLine.Text = Pad.CurrentLine.Text.Remove(Pad.Cursor.Column - characters, characters);
            Pad.Cursor.Column -= characters;
        }

        private void CanDeleteBack(object sender, CanExecuteRoutedEventArgs args) {
            args.CanExecute = Pad.Cursor.Column > 0;
        }

        private void ExecuteDelete(object sender, ExecutedRoutedEventArgs args) {
            Range range = args.Parameter as Range;

            Clipboard.SetText(Pad.GetText(range, true));

            if(Pad.Cursor.Row < Pad.Lines.Count) {
                Pad.Cursor.Row = range.StartRow;
                Pad.Cursor.Column = range.StartColumn;
            }
            else {
                Pad.Cursor.Row = Pad.Lines.Count - 1;
                Pad.Cursor.Column = 0;
            }
        }

        private void ExecuteDeleteLine(object sender, ExecutedRoutedEventArgs args) {
            Clipboard.SetText(Pad.CurrentLine.Text);
            Pad.RemoveLine(Pad.Cursor.Row);
        }

        private void ExecuteRunCommand(object sender, ExecutedRoutedEventArgs args) {
            string command = null;
            Dictionary<string, string> commandArgs = new Dictionary<string,string>();
            int start = Pad.Cursor.Row;
            int end = Pad.Cursor.Row+1;
            string result = null;

            while (start > 0
                    && Pad.Lines[start].Text.Contains(":")
                    && Pad.Lines[start].Text.StartsWith(" "))
                start -= 1;

            while (end < Pad.Lines.Count
                    && Pad.Lines[end].Text.Contains(":")
                    && Pad.Lines[end].Text.StartsWith(" "))
                end += 1;

            foreach (var line in Pad.Lines.Skip(start).Take(end - start)) {
                var arg = line.Text.Split(new char[] { ':' }, 2);

                if (command == null)
                    command = arg[0].Trim();

                commandArgs[arg[0].Trim()] = arg[1].Trim();
            }

            result = Commands.Invoke(command, commandArgs);

            if (result != null) {
                Pad.Lines.RemoveRange(start, end);

                foreach(string line in result.Split('\n'))
                    Pad.InsertLine(
                        start++,
                        line);
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
