using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows;

namespace Agent {

    public class AgentCommands {
        private Dictionary<string, AgentCommand> commands;
        private Memories memories;
        private Reminders reminders;
        private Definitions definitions;

        public AgentCommands() {
            memories = new Memories();
            reminders = new Reminders();
            definitions = new Definitions();
            commands = new Dictionary<string, AgentCommand>{
                { "remember", new AgentCommand{ Exec = Remember,
                    Params = new List<string> { "this", "when" } } },
                { "recall", new AgentCommand{ Exec = Recall } },
                { "remind", new AgentCommand{ Exec = Remind,
                    Params = new List<string> { "on", "message", "repeat" } } },
                { "define", new AgentCommand{ Exec = Define } },
                { "hilight", new AgentCommand{ Exec = Hilight,
                    Params = new List<string> { "bg", "fg", "st", "wt" } } },
                { "map", new AgentCommand{ Exec = Map,
                    Params = new List<string> { "mode", "macro" } } },
                { "source", new AgentCommand{ Exec = Source } },

                { "help", new AgentCommand{ Exec = Help } },
            };
        }

        public bool IsCommand(string command) {
            if(command.Contains(':'))
                command = command.Split(':')[0].Trim();
            
            return commands.ContainsKey(command.ToLower());
        }

        public IEnumerable<string> GetParamaters(string command) {
            return commands[command.ToLower()].Params;
        }

        public string Invoke(string command, Dictionary<string, string> args) {
            return commands[command.ToLower()].Exec(args);
        }

        public void ParseAndRun(IList<string> lines, int start, out Range size, out string result) {
            string command = null;
            Dictionary<string, string> commandArgs = new Dictionary<string, string>();
            int end = start;

            while(end+1 < lines.Count
                    && lines[end + 1].Contains(":")
                    && lines[end + 1].StartsWith(" "))
                end += 1;

            foreach(var line in lines.Skip(start).Take(1 + end - start)) {
                var arg = line.Split(new char[] { ':' }, 2);

                if(command == null)
                    command = arg[0].Trim();

                commandArgs[arg[0].Trim()] = arg[1].Trim();
            }

            result = Invoke(command, commandArgs);
            if(result != null && !result.EndsWith("\n"))
                result = result + '\n';

            size = new Range(start) {
                EndRow = end,
                EndColumn = lines[end].Length
            };
        }

        private string Remember(Dictionary<string, string> args) {
            string replacement = null;

            try {
                if (args.ContainsKey("when"))
                    memories.Add(args["remember"], args["this"], args["when"]);
                else
                    memories.Add(args["remember"], args["this"]);
                replacement = args["this"];
            }
            catch {
            }

            return replacement;
        }

        private string Recall(Dictionary<string, string> args) {
            if(args["recall"] == "today")
                return String.Join("\n", memories.Get(DateTime.Today));
            else if(args["recall"] == "tomorrow")
                return String.Join("\n", memories.Get(DateTime.Today.AddDays(1)));
            else if(String.IsNullOrWhiteSpace(args["recall"]))
                return String.Join("\n", memories.GetTop(10).Select(m => "recall:"+m));
            else
                return String.Join("\n", memories.Get(args["recall"]));
        }

        private string Remind(Dictionary<string, string> args) {
            var message = "";
            try {
                string name = args["remind"];
                string text = null;
                DateTime time;

                if(args.ContainsKey("on") && args["on"] != "")
                    time = DateTime.Parse(args["on"]);
                else
                    throw new Exception("The argument 'on' is required.");

                if(args.ContainsKey("message") && args["message"] != "")
                    text = args["message"];

                //frequency

                reminders.Add(name, text, time, Frequency.Once);
            }
            catch(Exception ex) {
                message = ex.Message;
            }

            return message;
        }

        private string Define(Dictionary<string, string> args) {
            return definitions.Define(args["define"]);
        }

        private string Hilight(Dictionary<string, string> args) {
            Regex regex = new Regex(args["hilight"]);
            Highlight hilight = new Highlight();

            if(args.ContainsKey("bg"))
                hilight.Background = Highlight.ParseColor(args["bg"]);
            if(args.ContainsKey("fg"))
                hilight.Foreground = Highlight.ParseColor(args["fg"]);
            if(args.ContainsKey("st"))
                hilight.Style = Highlight.ParseStyle(args["st"]);
            if(args.ContainsKey("wt"))
                hilight.Weight = Highlight.ParseWeight(args["wt"]);

            Highlighter.Add(regex, hilight);

            return "";
        }

        private string Map(Dictionary<string, string> args) {
            string key = args["map"];
            string mode = args["mode"];
            string macro = args["macro"];

            MapResolver.Map(mode, key, macro);

            return "";
        }

        private string Source(Dictionary<string, string> args) {
            string file = Regex.Replace(
                args["source"],
                @"~",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            Script script = new Script(this, file);
            string result = "";

            try {
                script.Run();
            }
            catch(Exception ex) {
                result = ex.Message;
            }

            return result;
        }

        private string Help(Dictionary<string, string> args) {
            StringBuilder help = new StringBuilder();

            foreach(var ncp in commands)
                help.AppendFormat("help:{0}\n", ncp.Key);

            return help.ToString();
        }
    }
}
