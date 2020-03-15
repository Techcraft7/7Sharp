using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using Techcraft7_DLL_Pack.Text;
using _7Sharp.Intrerpreter;
using _7SShell = _7Sharp.Shell.Shell;

namespace _7Sharp
{
	using static ColorConsoleMethods;
	using static Console;
	using static ConsoleColor;
	internal class Program
	{
		internal static _7SShell shell = new _7SShell();

		[STAThread]
		private static void Main(string[] args)
		{
			if (args.Length > 1)
			{
				WriteLineColor("You may only pass 1 script file to read!", Red);
				Read();
				return;
			}
			//title
			Title = "7Sharp";
			if (args.Length == 1)
			{
				try
				{
					using (StreamReader sr = new StreamReader(args[0]))
					{
						shell.SetCode(sr.ReadToEnd());
						shell.Execute("run");
					}
				}
				catch (Exception e)
				{
					WriteLineColor($"An error occured while reading the file you passed to 7Sharp... does it exist?\nError Type: {e.GetType()}", Red);
					Read();
					return;
				}
			}
			shell.Run();
			WriteLineColor("Press any key to exit 7Sharp", Yellow);
			_ = ReadKey(true);
		}
	}
}