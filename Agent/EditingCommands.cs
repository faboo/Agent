using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agent {
    public static class EditingCommands {
        public static RoutedCommand HandleKey = new RoutedCommand("HandleKey", typeof(EditingCommands));
        public static RoutedCommand SetMode = new RoutedCommand("SetMode", typeof(EditingCommands));

        public static RoutedCommand Motion = new RoutedCommand("Motion", typeof(EditingCommands));

        public static RoutedCommand MoveLeft = new RoutedCommand("MoveLeft", typeof(EditingCommands));
        public static RoutedCommand MoveRight = new RoutedCommand("MoveRight", typeof(EditingCommands));
        public static RoutedCommand MoveUp = new RoutedCommand("MoveUp", typeof(EditingCommands));
        public static RoutedCommand MoveDown = new RoutedCommand("MoveDown", typeof(EditingCommands));
        public static RoutedCommand MoveEnd = new RoutedCommand("MoveEnd", typeof(EditingCommands));
        public static RoutedCommand MoveHome = new RoutedCommand("MoveHome", typeof(EditingCommands));
        public static RoutedCommand MoveHome = new RoutedCommand("MoveLine", typeof(EditingCommands));

        public static RoutedCommand DeleteBack = new RoutedCommand("DeleteBack", typeof(EditingCommands));
        public static RoutedCommand Delete = new RoutedCommand("Delete", typeof(EditingCommands));
        public static RoutedCommand DeleteLine = new RoutedCommand("DeleteLine", typeof(EditingCommands));

        public static RoutedCommand InsertNewline = new RoutedCommand("InsertNewline", typeof(EditingCommands));
        public static RoutedCommand AppendNewline = new RoutedCommand("AppendNewline", typeof(EditingCommands));
        public static RoutedCommand PrependNewline = new RoutedCommand("PrependNewline", typeof(EditingCommands));
        public static RoutedCommand PrependText = new RoutedCommand("PrependText", typeof(EditingCommands));
        public static RoutedCommand AppendText = new RoutedCommand("AppendText", typeof(EditingCommands));
        public static RoutedCommand Yank = new RoutedCommand("Yank", typeof(EditingCommands));
        public static RoutedCommand YankLine = new RoutedCommand("YankLine", typeof(EditingCommands));

        public static RoutedCommand RunCommand = new RoutedCommand("RunCommand", typeof(EditingCommands));
    }
}
