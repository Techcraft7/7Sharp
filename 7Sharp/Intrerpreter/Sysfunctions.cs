using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionDictionary = System.Collections.Generic.Dictionary<string, _7Sharp.Intrerpreter._7sFunction>;

namespace _7Sharp.Intrerpreter
{
	using static Console;
	internal static class SystemFunctions
	{
		internal static Interpreter Interpreter = null;
		internal static Random rng = new Random();

		internal static FunctionDictionary random = new FunctionDictionary()
		{
			{ "nextInt", new _7sFunction("nextInt", 2, new Func<int, int, int>(NextInt)) },
			{ "nextDouble", new _7sFunction("nextDouble", 2, new Func<double, double, double>(NextDouble)) },
			{ "seed", new _7sFunction("seed", 1, new Action<int>(Seed)) }
		};
		internal static FunctionDictionary io = new FunctionDictionary()
		{
			{ "getContents", new _7sFunction("getContents", 1, new Func<string, string>(GetContents)) },
			{ "writeFile", new _7sFunction("writeFile", 2, new Action<string, string>(WriteFile)) }
		};

		internal static void Init(ref Interpreter interpreter)
		{
			Interpreter = interpreter;
			interpreter.functions.Clear();
			Interpreter.functions.Add("write", new _7sFunction("write", 1, new Action<dynamic>(Write)));
			Interpreter.functions.Add("read", new _7sFunction("read", 0, new Func<string>(Read)));
			Interpreter.functions.Add("getLoopIndex", new _7sFunction("getLoopIndex", 0, new Func<int>(GetLoopIndex)));
			Interpreter.functions.Add("fgColor", new _7sFunction("fgColor", 1, new Action<dynamic>(FgColor)));
			Interpreter.functions.Add("bgColor", new _7sFunction("bgColor", 1, new Action<dynamic>(BgColor)));
			Interpreter.functions.Add("clear", new _7sFunction("clear", 0, new Action(Clear)));
			Interpreter.functions.Add("wait", new _7sFunction("wait", 1, new Action<int>(Wait)));
			Interpreter.functions.Add("len", new _7sFunction("len", 1, new Func<object, int>(Len)));
			Interpreter.functions.Add("chars", new _7sFunction("chars", 1, new Func<string, char[]>(Chars)));
		}
		#region main
		static char[] Chars(string s) => s.ToCharArray();

		static int Len(object v) => (v == null || (!v.GetType().IsSubclassOf(typeof(Array)) && v.GetType() != typeof(string))) ? -1 : (v.GetType().IsSubclassOf(typeof(Array)) ? ((Array)v).Length : ((string)v).Length);

		static void Wait(int ms) => System.Threading.Thread.Sleep(ms);

		static void ClearConsole() => Clear();

		static string Read() => ReadLine();

		static void BgColor(dynamic c) => BackgroundColor = (ConsoleColor)c;

		static void FgColor(dynamic c) => ForegroundColor = (ConsoleColor)c;

		static void Write(dynamic s) => WriteLine(s);

		static int GetLoopIndex() => Interpreter.loopIndexes.Count != 0 ? Interpreter.loopIndexes.Peek() : -1;
		#endregion

		#region random.sys
		static int NextInt(int min, int max) => rng.Next(min, max + 1);

		static double NextDouble(double min, double max) => (rng.NextDouble() * (max - min)) + min;

		static void Seed(int seed) => rng = new Random(seed);
		#endregion
		#region io.sys
		static string GetContents(string path)
		{
			string s;
			using (StreamReader sr = new StreamReader(path))
			{
				s = sr.ReadToEnd();
			}
			return s;
		}

		static void WriteFile(string path, string contents)
		{
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.Write(contents);
			}
		}
		#endregion
	}
}
