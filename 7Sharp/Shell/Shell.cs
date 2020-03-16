using _7Sharp.Intrerpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Techcraft7_DLL_Pack.Text;
using _7SEditor = _7Sharp.Editor.Editor;
using CommandList = System.Collections.Generic.Dictionary<_7Sharp.Shell.CommandInfo, System.Action<string[]>>;

namespace _7Sharp.Shell
{
	using static Console;
	using static ConsoleColor;
	using static ColorConsoleMethods;
	internal partial class Shell
	{
		internal _7SEditor editor = new _7SEditor();
		internal Interpreter interpreter = new Interpreter();
		internal CommandList commands;
		bool run = true;

		public Shell()
		{
			commands = new CommandList()
			{
				{ new CommandInfo("edit", "Open the editor"), Edit },
				{ new CommandInfo("run", "Run the code in the editor"), RunCode },
				{ new CommandInfo("help", "Display this message"), Help },
				{ new CommandInfo("load", "Load a file into the editor"), Load },
				{ new CommandInfo("save", "Save the code in the editor to a file"), Save },
				{ new CommandInfo("clear", "Clear the screen"), new Action<string[]>((args) => { Clear(); }) },
				{ new CommandInfo("exit", "Close 7Sharp"), new Action<string[]>((args) => { run = false; }) },
				{ new CommandInfo("export", "Export the code into "), new Action<string[]>(Export) }
			};
		}

		public void Run(params string[] onStart)
		{
			if (onStart != null && onStart.Length > 0)
			{
				WriteLineColor("Running shell commands!", Magenta);
				foreach (string cmd in onStart)
				{
					Execute(cmd);
				}
			}
			run = true;
			while (run)
			{
				ForegroundColor = Gray;
				BackgroundColor = Black;
				WriteMultiColor(new string[] { "7", "Sharp", "> " }, new ConsoleColor[] { Yellow, Green, Cyan });
				Execute(ReadLine());
			}
		}

		public void Execute(string input)
		{
			string command = input.Split(' ').First().ToLower();
			string[] args = input.Split(' ').Skip(1).ToArray();
			int cmdIndex = commands.Keys.ToList().FindIndex(x => x.Name == command);
			if (string.IsNullOrEmpty(input))
			{
				return;
			}
			if (cmdIndex < 0)
			{
				WriteLineColor("Invalid command! Type \"help\" for commands!", Red);
			}
			else
			{
				commands.ToList()[cmdIndex].Value.Invoke(args);
			}
		}

		public void SetCode(string code) => editor.Code = code;

		public string GetCode() => editor.Code;
	}
}
