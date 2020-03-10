using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter
{
	using static Techcraft7_DLL_Pack.Text.ColorConsoleMethods;
	using static ConsoleColor;
	using static Console;
#pragma warning disable IDE1006 // Naming Styles
	public class _7sFunction
#pragma warning restore IDE1006 // Naming Styles
	{
		public int NumberOfArguments;
		public string Name;
		public dynamic Code;

		public _7sFunction(string name, int args, Delegate action)
		{
			Name = name;
			NumberOfArguments = args;
			Code = action;
		}

		public void Run(out dynamic ReturnValue, params object[] args)
		{
			ReturnValue = null;
			try
			{
				ReturnValue = Code.DynamicInvoke(args);
			}
			catch (Exception e)
			{
				WriteLineColor($"Error in function {Name}: {e.Message}", Red);
			}
		}
	}
}
