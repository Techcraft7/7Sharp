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
			Interpreter.functions.Add("getLoopIndex", new _7sFunction("getLoopIndex", 0, new Func<int>(GetLoopIndex)));
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
