using _7Sharp.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _7Sharp.Plugins
{
	public abstract class ShellCommand
	{
		public readonly string Name;
		private readonly string Help;

		public ShellCommand(string Name, string Help)
		{
			this.Name = Name ?? throw new ArgumentNullException(nameof(Name));
			this.Help = Help ?? throw new ArgumentNullException(nameof(Help));
			if (Name.Contains(" "))
			{
				throw new ArgumentException("Commands cannot have a space in the name!", nameof(Name));
			}
		}

		public abstract void Run(string[] args);

		internal KeyValuePair<CommandInfo, Action<string[]>> GetCommandKV()
		{
			return new KeyValuePair<CommandInfo, Action<string[]>>(new CommandInfo(Name, Help), new Action<string[]>(Run));
		}
	}
}
