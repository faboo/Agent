using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Agent {
    public class Script {
        public static readonly string DefaultScript;

        static Script() {
            DefaultScript = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".agentrc");
        }

        private AgentCommands commands = null;
        private string scriptfile = null;

        public Script(AgentCommands commands, string scriptfile) {
            this.commands = commands;
            this.scriptfile = scriptfile;
        }

        public void Run() {
            string[] lines = File.ReadAllLines(scriptfile);
            int commandStart = 0;
            Range size;
            string result;

            while(commandStart < lines.Length) {
                if(commands.IsCommand(lines[commandStart])){
                    commands.ParseAndRun(lines, commandStart, out size, out result);

                    commandStart = size.EndRow;
                }

                commandStart += 1;
            }
        }
    }
}
