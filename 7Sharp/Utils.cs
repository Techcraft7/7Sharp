using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;

namespace _7Sharp
{
	internal class Utils
	{
		internal static void PrintError(Exception e)
		{
			ColorConsoleMethods.WriteLineColor($"{e.GetType()}: {e.Message}\n{e.StackTrace}", ConsoleColor.Red);
		}
	}
}
