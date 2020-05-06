﻿using System;
using System.Linq;
using System.IO;
using Techcraft7_DLL_Pack.Text;
using _7Sharp._7sLib;

namespace _7Sharp.Shell
{
	using static ConsoleColor;
	using static ColorConsoleMethods;
	internal sealed partial class Shell
	{
		private void Export(string[] args)
		{
			_7sLibManager.Save(string.Join(" ", args));
		}

		void Load(string[] args)
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
					WriteLineColor("File loaded into the editor!", Green);
				}
				catch (Exception e)
				{
					WriteLineColor($"An error occured loading the file into the editor! {e.GetType()}: {e.Message}", Red);
				}
			}
			else
			{
				WriteLineColor("Could not find the file or the path was invalid!", Red);
			}
		}

		void Save(string[] args)
		{
			bool replace = false;
			if (args[0] == "-o")
			{
				replace = true;
				args = args.Skip(1).ToArray();
			}
			string path = string.Join(" ", args);
			if (File.Exists(path) && !replace)
			{
				WriteLineColor("The file already exists! Do save -o <path> to overrite files!", Red);
			}
			else
			{
				try
				{
					using (StreamWriter sw = new StreamWriter(path))
					{
						sw.Write(GetCode());
					}
					WriteLineColor("Code saved!", Green);
				}
				catch
				{
					WriteLineColor("An error occured saving the file! Is the path valid?", Red);
				}
			}
		}

		void Help(string[] args)
		{
			for (int i = 0; i < commands.Keys.Count; i++)
			{
				var key = commands.Keys.ToArray()[i];
				if (key == null)
				{
					key = new CommandInfo(" NULL COMMAND ", " The command help could not be loaded");
				}
				if (commands[key] == null)
				{
					commands[key] = new Action<string[]>(_ => { WriteLineColor("The command code could not be loaded", Red); });
				}
			}
			commands.ToList().ForEach(c => { WriteLineMultiColor(new string[] { c.Key.Name + ": ", c.Key.Help }, new ConsoleColor[] { Yellow, Cyan }); });
		}

		void RunCode(string[] args)
		{
			Intrerpreter.SystemFunctions.Init(ref interpreter);
			interpreter.Run(GetCode());
		}

		void Edit(string[] args) => _ = editor.ShowDialog();
	}
}
