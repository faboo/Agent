using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agent {
    public delegate string ExecCommand(Dictionary<string, string> args);

    public class AgentCommand {
        public ExecCommand Exec { get; set; }
        public IEnumerable<string> Params { get; set; }

        public AgentCommand() {
            Params = new List<string>();
        }
    }
}
