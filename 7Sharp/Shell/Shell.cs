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
	internal class Shell
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
				{ new CommandInfo("clear", "Clear the screen"), new Action<string[]>((args) => { Clear(); }) },
				{ new CommandInfo("exit", "Close 7Sharp"), new Action<string[]>((args) => { run = false; }) }
			};
		}

		public void Run()
		{
			run = true;
			while (run)
			{
				WriteMultiColor(new string[] { "7", "Sharp", "> " }, new ConsoleColor[] { Yellow, Green, Cyan });
				string input = ReadLine();
				string command = input.Split(' ').First().ToLower();
				string[] args = input.Split(' ').Skip(1).ToArray();
				int cmdIndex = commands.Keys.ToList().FindIndex(x => x.Name == command);
				if (cmdIndex < 0)
				{
					WriteLineColor("Invalid command! Type \"help\" for commands!", Red);
					continue;
				}
				else
				{
					commands.ToList()[cmdIndex].Value.Invoke(args);
				}
				
			}
		}

		public void Load(string[] args)
		{
			string path = string.Join(" ", args);
			if (File.Exists(path))
			{
				try
				{
					using (StreamReader sr = new StreamReader(path))
					{
						SetCode(sr.ReadToEnd());
					}
				}
				catch
				{
					WriteLineColor("An error occured loading the file into the editor!", Red);
				}
			}
			else
			{
				WriteLineColor("Could not find the file or the path was invalid!", Red);
			}
			WriteLineColor("File loaded into the editor!", Green);
		}

		public void Help(string[] args) => commands.ToList().ForEach(c => { WriteLineMultiColor(new string[] { c.Key.Name + ": ", c.Key.Help }, new ConsoleColor[] { Yellow, Cyan }); });

		void RunCode(string[] args) => interpreter.Run(GetCode());

		void Edit(string[] args) => editor.ShowDialog();

		public void SetCode(string code) => editor.Code = code;

		public string GetCode() => editor.Code;
	}
}
