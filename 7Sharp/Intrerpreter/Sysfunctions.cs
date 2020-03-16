using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter
{
	using static Console;
	internal static class SystemFunctions
	{
		internal static Interpreter Interpreter = null;

		internal static void Init(ref Interpreter interpreter)
		{
			Interpreter = interpreter;
			Interpreter.functions.Add("write", new _7sFunction("write", 1, new Action<dynamic>(Write)));
			Interpreter.functions.Add("read", new _7sFunction("read", 0, new Func<string>(Read)));
			Interpreter.functions.Add("getLoopIndex", new _7sFunction("getLoopIndex", 0, new Func<int>(GetLoopIndex)));
			Interpreter.functions.Add("fgColor", new _7sFunction("fgColor", 1, new Action<dynamic>(FgColor)));
			Interpreter.functions.Add("bgColor", new _7sFunction("bgColor", 1, new Action<dynamic>(BgColor)));
			Interpreter.functions.Add("clear", new _7sFunction("clear", 0, new Action(Clear)));
		}

		static void _7sClear()
		{
			Clear();
		}

		static string Read()
		{
			return ReadLine();
		}

		static void BgColor(dynamic c)
		{
			BackgroundColor = (ConsoleColor)c;
		}

		static void FgColor(dynamic c)
		{
			ForegroundColor = (ConsoleColor)c;
		}

		static void Write(dynamic s)
		{
			WriteLine(s);
		}

		static int GetLoopIndex()
		{
			return Interpreter.loopIndexes.Count != 0 ? Interpreter.loopIndexes.Peek() : -1;
		}
	}
}
