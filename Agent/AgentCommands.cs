using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent {

    public class AgentCommands {
        private Dictionary<string, AgentCommand> commands;
        private Memories memories;
        private Definitions definitions;

        public AgentCommands() {
            memories = new Memories();
            definitions = new Definitions();
            commands = new Dictionary<string, AgentCommand>{
                { "remember", new AgentCommand{ Exec = Remember, Params = new List<string> { "this" } } },
                { "recall", new AgentCommand{ Exec = Recall } },
                { "define", new AgentCommand{ Exec = Define } },

                { "help", new AgentCommand{ Exec = Help } },
            };
        }

        public bool IsCommand(string command) {
            return commands.ContainsKey(command.ToLower());
        }

        public IEnumerable<string> GetParamaters(string command) {
            return commands[command.ToLower()].Params;
        }

        public string Invoke(string command, Dictionary<string, string> args) {
            return commands[command.ToLower()].Exec(args);
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
            else if (args["recall"] == "tomorrow")
                return String.Join("\n", memories.Get(DateTime.Today.AddDays(1)));
            else
                return String.Join("\n", memories.Get(args["recall"]));
        }

        private string Define(Dictionary<string, string> args) {
            return definitions.Define(args["define"]);
        }

        private string Help(Dictionary<string, string> args) {
            StringBuilder help = new StringBuilder();

            foreach(var ncp in commands)
                help.AppendFormat("help:{0}\n", ncp.Key);

            return help.ToString();
        }
    }
}
