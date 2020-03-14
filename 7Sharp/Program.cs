using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using Techcraft7_DLL_Pack.Text;
using _7Sharp;
using _7Sharp.Intrerpreter;

namespace _7Sharp
{
	using static ColorConsoleMethods;
	using static Console;
	using static ConsoleColor;
	internal class Program
	{
		internal static Interpreter interpreter = new Interpreter();
		private static void Main(string[] args)
		{
			if (args.Length > 1)
			{
				WriteLineColor("You may only pass 1 script file to read!", Red);
				Read();
				return;
			}
			//init
			SystemFunctions.Init();
			//title
			Title = "7Sharp";
			//string code = "foo = 1;\nloop (5) {\n\twrite(\"Hey\"); //this is a comment\n\tif (foo == 5) {\n\t\twrite(\"i can math\");\n\t}\n\twrite(\"Ok im done\");\n\tfoo++;\n}";
			//string code = "aaa = 0;//comment\nwrite(\"aaa is \" + aaa);\nwhile(aaa < 10) {\n\twrite(aaa);\n\taaa = aaa + 1;\n}\nwrite(\"some semicolons: ;;;;;\");";
			//string code = "x=0;\nwhile(x<10){\nwrite(x);\nx = x + 1;\n}";
			string code =
@"foo = 10;
if (foo	< 5) {
	write(""foo is small!"");
} else if (foo == 5) {
	write(""foo is 5!"");
} else if (foo == 10) {
	write(""foo is VERY big!"");
} else {
	write(""foo is big"");
}
loop (foo) {
	write(""Writing this number {foo} times!"");
}";
			WriteLine(code);
			if (args.Length == 1)
			{
				try
				{
					using (StreamReader sr = new StreamReader(args[0]))
					{
						code = sr.ReadToEnd();
					}
				}
				catch (Exception e)
				{
					WriteLineColor($"An error occured while reading the file you passed to 7Sharp... does it exist?\nError Type: {e.GetType()}", Red);
					Read();
					return;
				}
			}
			interpreter.Run(code);
			ReadLine();
		}
	}
}