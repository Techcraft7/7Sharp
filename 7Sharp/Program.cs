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
	internal class Program
	{
        internal static Interpreter interpreter = new Interpreter();
		private static void Main(string[] args)
		{
            //init
            SystemFunctions.Init();
            //title
            Title = "7Sharp";
            interpreter.Run("loop (5) {\n\twrite(\"Hey\") //this is a comment\n\tif (@(foo) == 69) {\n\t\twrite(\"i can math\")\n\t}\n\twrite(\"Ok im done\")\n}");
            ReadLine();
        }
    }
}