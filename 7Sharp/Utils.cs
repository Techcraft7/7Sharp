using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;

namespace _7Sharp
{
	internal class Utils
	{
		//Converts "SomePascalCaseString" -> "Some Pascal Case String"
		internal static string FormatPascalString(string v)
		{
			string output = string.Empty;
			for (int i = 0; i < v.Length; i++)
			{
				//if character is uppercase and not the first character
				if (i > 0 && v[i].ToString() == v[i].ToString().ToUpper())
				{
					output += " ";
				}
				output += v[i];
			}
			return output;
		}

		internal static void PrintError(Exception e)
		{
			ColorConsoleMethods.WriteLineColor($"{e.GetType()}: {e.Message}\n{e.StackTrace}", ConsoleColor.Red);
		}

		internal static dynamic MultiDimToJagged(dynamic input, int nDims)
		{
			try
			{
				MethodInfo m = typeof(Enumerable).GetMethod("Cast").GetGenericMethodDefinition().MakeGenericMethod(Type.GetType($"System.Object{new string('X', input.Rank).Replace("X", "[]")}"));
				IEnumerator ie = ((IEnumerable)m.Invoke(input, new object[] { input })).GetEnumerator();
				ie.MoveNext();
				return ie.Current;
			}
			catch (Exception e)
			{
				PrintError(e);
				return null;
			}
		}
	}
}
