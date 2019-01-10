using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using _7Sharp;
using System.Reflection;

namespace _7Sharp
{
	class Program
	{
		public static bool _inputs = false;
		public static string[] input_split = { };
		public static bool exit = false;
		public static bool loop = false;
		public static bool parsed = false;
		public static string input = "";
		public static List<InstalledPackageCmd> custom_cmds = new List<InstalledPackageCmd>();
		public static bool has_args = false;
		public static List<VarInt> ints = new List<VarInt>();
		public static List<VarString> strings = new List<VarString>();
		public static StreamReader srr = null;
		public static int times = 1;
		public static List<Command> commands = new List<Command>();
		public static bool echo = true;
		static void Main(string[] args)
		{
			//add commands
			commands.Add(new Write());
			commands.Add(new Help());
			commands.Add(new Clear());
			commands.Add(new Add());
			commands.Add(new Exit());
			commands.Add(new Loop());
			commands.Add(new Int());
			commands.Add(new StringSet());
			commands.Add(new _7sRandom());
			commands.Add(new VarStringSet());
			commands.Add(new VarIntSet());
			commands.Add(new ReadFile());
			commands.Add(new WriteFile());
			commands.Add(new InstallPkg());
			commands.Add(new Out());
			commands.Add(new Open());
			commands.Add(new Com());
			commands.Add(new WhileInt());
			commands.Add(new If());
			commands.Add(new Out());
			//Do parsing stuff...
			if (args.Length == 0 || args == null)
			{
				has_args = false;
			}
			else
			{
				has_args = true;
			}
			//set titlebar
			Console.Title = "7Sharp";
			//cant read more than one 7s file
			if (has_args == true)
			{
				if (args.Length > 1)
				{
					Console.WriteLine("Cannot read multiple .7s files...  Press enter to exit...");
					Console.Read();
				}
			}
			//start 7Sharp
			while (true)
			{
				if (_inputs == true)
				{
					_inputs = false;
					goto inputs;
				}
				if (has_args == true)
				{
					//display "Parsing..."
					srr = new StreamReader(args[0]);
					input = srr.ReadLine();
					input_split = input.Split(' ', '\n');
					bool x = false;
					if (input != "out off" && x == false)
					{
						Techcraft7_DLL_Pack.ColorConsoleMethods.WriteLineColor("Parsing...", ConsoleColor.Yellow);
						x = true;
					}
				}
				exit = false;
				//get input
				if (!has_args)
				{
					input = Console.ReadLine();
					input_split = input.Split(' ', '\n');
				}
				//Input parsing
				inputs:
				parse:
				//loops
				foreach (Command i in commands)
				{
					input_split = input.Split(' ', '\n');
					i.Parse();
				}
				//custom commands
				foreach (InstalledPackageCmd i in custom_cmds)
				{
					input_split = input.Split(' ', '\n');
					i.Parse();
				}
				if (exit == true)
				{
					if (srr != null)
					{
						srr.Close();
						srr.Dispose();
					}
				}
				if (srr != null && srr.EndOfStream == false)
				{
					has_args = true;
					input = srr.ReadLine();
					goto parse;
				}
				else if (srr != null && srr.EndOfStream == true)
				{
					has_args = false;
					if (parsed == false && echo)
					{
						Techcraft7_DLL_Pack.ColorConsoleMethods.WriteLineColor("Done!", ConsoleColor.Yellow);
					}
					parsed = true;
				}
				//retake input
				_inputs = false;
			}
		}
	}
}