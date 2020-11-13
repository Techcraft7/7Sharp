using _7Sharp.Manual;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			Interpreter.functions.Add("writeraw", new _7sFunction("writeraw", 1, new Action<dynamic>(WriteRaw)));
			Interpreter.functions.Add("read", new _7sFunction("read", 0, new Func<string>(Read)));
			Interpreter.functions.Add("getLoopIndex", new _7sFunction("getLoopIndex", 0, new Func<int>(GetLoopIndex)));
			Interpreter.functions.Add("fgColor", new _7sFunction("fgColor", 1, new Action<dynamic>(FgColor)));
			Interpreter.functions.Add("bgColor", new _7sFunction("bgColor", 1, new Action<dynamic>(BgColor)));
			Interpreter.functions.Add("clear", new _7sFunction("clear", 0, new Action(Clear)));
			Interpreter.functions.Add("wait", new _7sFunction("wait", 1, new Action<int>(Wait)));
			Interpreter.functions.Add("len", new _7sFunction("len", 1, new Func<object, int>(Len)));
			Interpreter.functions.Add("chars", new _7sFunction("chars", 1, new Func<string, char[]>(Chars)));
			Interpreter.functions.Add("matrix", new _7sFunction("matrix", 2, new Func<int, int[], dynamic>(Matrix)));
		}
		#region main
		[ManualDocs("{\"title\":\"matrix()\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"matrix(int nDims, int[] dimSizes)\"}]},{\"header\":\"Description\",\"text\":[{\"text\":\"Creates a matrix of with \"},{\"text\":\"nDims\",\"color\":\"Green\"},{\"text\":\" dimensions, with the sizes of each dimension stored in \"},{\"text\":\"dimSizes\",\"color\":\"Green\"},{\"text\":\". Will crash if \"},{\"text\":\"nDims\",\"color\":\"Green\"},{\"text\":\" < 1 or \"},{\"text\":\"dimSizes\",\"color\":\"Green\"},{\"text\":\" is null OR its length is less than \"},{\"text\":\"nDims\",\"color\":\"Green\"}]}]}")]
		private static dynamic Matrix(int numberOfDimensions, int[] dimensionSizes)
		{
			if (numberOfDimensions < 1)
			{
				throw new ArgumentException("You cannot have less than a 1 dimensional array!");
			}
			if (dimensionSizes == null)
			{
				throw new ArgumentNullException("Dimension size array was null!");
			}
			if (dimensionSizes.Length < numberOfDimensions)
			{
				throw new ArgumentException("You did not supply enough dimension sizes!");
			}
			dynamic output = Array.CreateInstance(typeof(object), dimensionSizes, new int[numberOfDimensions]);
			return Utils.MultiDimToJagged(output, numberOfDimensions);
		}

		private static char[] Chars(string str) => str.ToCharArray();

		private static int Len(object array) => (array == null || (!array.GetType().IsSubclassOf(typeof(Array)) && array.GetType() != typeof(string))) ? -1 : (array.GetType().IsSubclassOf(typeof(Array)) ? ((Array)array).Length : ((string)array).Length);

		private static void Wait(int mils) => System.Threading.Thread.Sleep(mils);

		private static void ClearConsole() => Clear();

		private static string Read() => ReadLine();

		private static void BgColor(dynamic color) => BackgroundColor = (ConsoleColor)color;

		private static void FgColor(dynamic color) => ForegroundColor = (ConsoleColor)color;

		private static void Write(dynamic value) => WriteLine(value);

		private static void WriteRaw(dynamic value) => Out.Write(value);

		private static int GetLoopIndex() => Interpreter.loopIndexes.Count != 0 ? Interpreter.loopIndexes.Peek() : -1;
		#endregion

		#region random.sys
		private static int NextInt(int min, int max) => rng.Next(min, max + 1);

		private static double NextDouble(double min, double max) => (rng.NextDouble() * (max - min)) + min;

		private static void Seed(int seed) => rng = new Random(seed);
		#endregion
		#region io.sys
		private static string GetContents(string path)
		{
			string s;
			using (StreamReader sr = new StreamReader(path))
			{
				s = sr.ReadToEnd();
			}
			return s;
		}

		private static void WriteFile(string path, string contents)
		{
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.Write(contents);
			}
		}
		#endregion
	}
}
