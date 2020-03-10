using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter
{
	using static _7Sharp.Program;
	using static Console;
	internal static class SystemFunctions
	{
		internal static void Init()
		{
			interpreter.functions.Add("write", new _7sFunction("write", 1, new Action<dynamic>(Write)));
			interpreter.functions.Add("getLoopIndex", new _7sFunction("getLoopIndex", 0, new Func<int>(GetLoopIndex)));
		}

		static void Write(dynamic s)
		{
			WriteLine(s);
		}

		static int GetLoopIndex()
		{
			return interpreter.loopIndexes.Peek();
		}
	}
}
