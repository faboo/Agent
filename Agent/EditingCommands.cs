using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agent {
    public static class EditingCommands {
        public static RoutedCommand HandleKey = new RoutedCommand("HandleKey", typeof(EditingCommands));
        public static RoutedCommand SetMode = new RoutedCommand("SetMode", typeof(EditingCommands));

        public static RoutedCommand Move = new RoutedCommand("Move", typeof(EditingCommands));

        public static RoutedCommand Yank = new RoutedCommand("Yank", typeof(EditingCommands));
        public static RoutedCommand Delete = new RoutedCommand("Delete", typeof(EditingCommands));

        public static RoutedCommand Undo = new RoutedCommand("Undo", typeof(EditingCommands));

        // TODO: we should really only have one InsertText function
        public static RoutedCommand InsertNewline = new RoutedCommand("InsertNewline", typeof(EditingCommands));
        public static RoutedCommand AppendNewline = new RoutedCommand("AppendNewline", typeof(EditingCommands));
        public static RoutedCommand PrependNewline = new RoutedCommand("PrependNewline", typeof(EditingCommands));
        public static RoutedCommand AppendText = new RoutedCommand("AppendText", typeof(EditingCommands));
        public static RoutedCommand InsertText = new RoutedCommand("InsertText", typeof(EditingCommands));

        public static RoutedCommand OpenFile = new RoutedCommand("OpenFile", typeof(EditingCommands));

        public static RoutedCommand RunCommand = new RoutedCommand("RunCommand", typeof(EditingCommands));
    }
}
