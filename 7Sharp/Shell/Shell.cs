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
using System.Reflection;
using _7Sharp.Plugins;

namespace _7Sharp.Shell
{
	using static Console;
	using static ConsoleColor;
	using static ColorConsoleMethods;
	internal sealed partial class Shell
	{
		internal _7SEditor editor = new _7SEditor();
		internal Interpreter interpreter = new Interpreter();
		internal CommandList commands;
		bool run = true;
		private readonly string PLUGINS_DIRECTORY = "plugins" + Path.DirectorySeparatorChar;

		public Shell()
		{
			if (!Directory.Exists(PLUGINS_DIRECTORY))
			{
				Directory.CreateDirectory(PLUGINS_DIRECTORY);
			}
			PLUGINS_DIRECTORY = Path.GetFullPath(PLUGINS_DIRECTORY);
			commands = new CommandList()
			{
				{ new SysCommandInfo("edit", "Open the editor"), Edit },
				{ new SysCommandInfo("run", "Run the code in the editor"), RunCode },
				{ new SysCommandInfo("help", "Display this message"), Help },
				{ new SysCommandInfo("load", "Load a file into the editor"), Load },
				{ new SysCommandInfo("save", "Save the code in the editor to a file"), Save },
				{ new SysCommandInfo("clear", "Clear the screen"), new Action<string[]>((args) => { Clear(); }) },
				{ new SysCommandInfo("exit", "Close 7Sharp"), new Action<string[]>((args) => { run = false; }) },
				{ new SysCommandInfo("export", "Export the code into "), new Action<string[]>(Export) }
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
			LoadPlugins(this);
			while (run)
			{
				try
				{
					ForegroundColor = Gray;
					BackgroundColor = Black;
					WriteMultiColor(new string[] { "7", "Sharp", "> " }, new ConsoleColor[] { Yellow, Green, Cyan });
					Execute(ReadLine());
				}
				catch (Exception e)
				{
					Utils.PrintError(e);
				}
			}
		}

		internal static void LoadPlugins(Shell s)
		{
			try
			{
				foreach (string path in Directory.EnumerateFiles(s.PLUGINS_DIRECTORY, "*.dll", SearchOption.AllDirectories))
				{
					Assembly dll = Assembly.LoadFile(path);
					foreach (Type t in dll.GetTypes())
					{
						if (t.IsSubclassOf(typeof(ShellPlugin)))
						{
							ShellPlugin plugin = (ShellPlugin)Activator.CreateInstance(t);
							plugin.Load(ref s);
						}
					}
				}
			}
			catch (Exception e)
			{
				WriteLineColor("An error occured while loading plugins...", Red);
				Utils.PrintError(e);
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
