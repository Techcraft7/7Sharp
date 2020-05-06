using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;

namespace _7Sharp.Plugins
{
	public abstract class ShellPlugin
	{
		internal bool valid = true;
		internal string InvalidReason = string.Empty;

		internal void Load(ref Shell.Shell s)
		{
			InvalidReason = string.Empty;
			valid = true;
			_ = GetCommands();
			if (!valid)
			{
				throw new InvalidProgramException("Plugin is not valid!");
			}
			foreach (ShellCommand sc in GetCommands())
			{
				var kv = sc.GetCommandKV();
				if (s.commands.ToList().FindIndex(x => x.Key.Name.Equals(kv.Key.Name)) > -1)
				{
					ColorConsoleMethods.WriteLineColor($"Command {kv.Key.Name} already exists!", ConsoleColor.Red);
					continue;
				}
				s.commands.Add(kv.Key, kv.Value);
			}
		}

		public List<ShellCommand> GetCommands()
		{
			foreach (ShellCommand cmd in GetCommandsInternal())
			{
				if (cmd == null)
				{
					valid = false;
				}
			}
			return GetCommandsInternal();
		}

		protected abstract List<ShellCommand> GetCommandsInternal();
	}
}
