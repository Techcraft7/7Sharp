﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using Techcraft7_DLL_Pack.Text;
using _7Sharp.Interpreter;
using _7SShell = _7Sharp.Shell.Shell;

namespace _7Sharp
{
	using static ColorConsoleMethods;
	using static Console;
	using static ConsoleColor;
	internal sealed class Program
	{
		internal static bool IsMono { get; } = Type.GetType("Mono.Runtime") != null;
		internal static char DirectorySeperator { get; } = IsMono ? '/' : '\\';
		internal static _7SShell shell = new _7SShell();

		[STAThread]
		private static void Main(string[] args)
		{
			//title
			Title = "7Sharp";
			if (args.Length >= 1)
			{
				try
				{
					using (StreamReader sr = new StreamReader(string.Join(" ", args)))
					{
						shell.SetCode(sr.ReadToEnd());
						shell.Execute("run");
#if DEBUG
#else
						return;
#endif
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
			if (!shell.SilentExit)
			{
				WriteLineColor("Press any key to exit 7Sharp", Yellow);
				_ = ReadKey(true);
			}
		}
	}
}