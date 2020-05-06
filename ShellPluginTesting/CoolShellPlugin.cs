using _7Sharp.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellPluginTesting
{
    public class CoolShellPlugin : ShellPlugin
    {
        protected override List<ShellCommand> GetCommandsInternal()
        {
            List<ShellCommand> cmds = new List<ShellCommand>
            {
                new CoolCommand()
            };
            return cmds;
        }
    }
    public class CoolCommand : ShellCommand
    {
        public CoolCommand() : base("cool_command", "do something cool!") => _ = new object();

        public override void Run(string[] args)
        {
            Console.WriteLine("Something cool!");
        }
    }
}
