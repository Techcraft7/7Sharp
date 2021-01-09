using _7Sharp.Manual;
using System;
using System.Linq;
using System.Threading;

namespace _7Sharp.Intrerpreter
{
	internal class SysFunctions
	{
		[ManualDocs("double", "{\"title\":\"double(o)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"double(<object>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Convert \"},{\"text\":\"o\",\"color\":\"Green\"},{\"text\":\" to a double (64-bit floating-point number)\"}]}]}")]
		public static double Double(object value)
		{
			try
			{
				return Convert.ToDouble(value);
			}
			catch (OverflowException)
			{
				throw new InterpreterException("double: Number is too big!");
			}
			catch (FormatException)
			{
				throw new InterpreterException("double: Invalid number!");
			}
			catch (Exception e)
			{
				throw new InterpreterException("double: Unkown error", e);
			}
		}

		[ManualDocs("int", "{\"title\":\"int(o)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"int(<object>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Convert \"},{\"text\":\"o\",\"color\":\"Green\"},{\"text\":\" to an int (32-bit integer)\"}]}]}")]
		public static int Int(object value)
		{
			try
			{
				return Convert.ToInt32(value);
			}
			catch (OverflowException)
			{
				throw new InterpreterException("int: Number is too big!");
			}
			catch (FormatException)
			{
				throw new InterpreterException("int: Invalid number!");
			}
			catch (Exception e)
			{
				throw new InterpreterException("int: Unkown error", e);
			}
		}

		[ManualDocs("write", "{\"title\":\"write(value)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"Outputs \"},{\"text\":\"value\",\"color\":\"Green\"},{\"text\":\" to the console, followed by a newline.\"}]}]}")]
		public static void Write(object obj) => Console.WriteLine(obj);

		[ManualDocs("writeraw", "{\"title\":\"writeraw(value)\",\"sections\":[{\"header\":\"Behavior\",\"text\":[{\"text\":\"Like \"},{\"text\":\"write(value)\",\"color\":\"Green\"},{\"text\":\", but it does not start a new line\"}]}]}")]
		public static void WriteRaw(object obj) => Console.Write(obj);

		[ManualDocs("read", "{\"title\":\"read()\",\"sections\":[{\"header\":\"Behavior\",\"text\":[{\"text\":\"Asks the user for input, and returns a string of the text provided.\"}]}]}")]
		public static string Read() => Console.ReadLine();

		#region trig
		[ManualDocs("trig", "{\"title\":\"trig\",\"sections\":[{\"header\":\"Note\",\"text\":[{\"text\":\"ALL TRIG FUNCTIONS ARE IN RADIANS!\",\"color\":\"Red\"}]},{\"header\":\"sin(x)\",\"text\":[{\"text\":\"Returns the sine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"cos(x)\",\"text\":[{\"text\":\"Returns the cosine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"tan(x)\",\"text\":[{\"text\":\"Returns the tangent of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"sin(x)\",\"text\":[{\"text\":\"Returns the sine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"cos(x)\",\"text\":[{\"text\":\"Returns the cosine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"tan(x)\",\"text\":[{\"text\":\"Returns the tangent of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"atan2(x, y)\",\"text\":[{\"text\":\"Returns the angle between (0, 0) and \"},{\"color\":\"Green\",\"text\":\"(x, y)\"},{\"text\":\" in radians.\"}]},{\"header\":\"Degrees and radians conversion\",\"text\":[{\"text\":\"Use \"},{\"text\":\"deg2rad(x)\",\"color\":\"Green\"},{\"text\":\" and \"},{\"text\":\"rad2deg(x)\",\"color\":\"Green\"},{\"text\":\" to convert \"},{\"text\":\"x\",\"color\":\"Green\"},{\"text\":\" from radians to degrees, or vice. versa. \"}]}]}")]
		public static double Sin(double v) => Math.Sin(v);

		public static double Cos(double v) => Math.Cos(v);

		public static double Tan(double v) => Math.Tan(v);

		public static double Asin(double v) => Math.Asin(Utils.CheckRange(v, -1, 1, "asin: Invalid domain!"));

		public static double Acos(double v) => Math.Acos(Utils.CheckRange(v, -1, 1, "acos: Invalid domain!"));

		public static double Atan(double v) => Math.Atan(v);

		public static double Atan2(double x, double y) => x == 0 && y == 0 ? 0 : Math.Atan2(x, y);

		public static double Deg2Rad(double v) => v * Math.PI / 180D;

		public static double Rad2Deg(double v) => v * 180D / Math.PI;
		#endregion

		[ManualDocs("sleep", "{\"title\":\"sleep(ms)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"sleep(<number of miliseconds to wait>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Waits for \"},{\"text\":\"ms\",\"color\":\"Green\"},{\"text\":\" miliseconds.\"}]}]}")]
		public static void Sleep(int ms)
		{
			if (ms < 0)
			{
				throw new InterpreterException("sleep: Cannot sleep for a negative number of miliseconds!");
			}
			Thread.Sleep(ms);
		}

		[ManualDocs("len", "{\"title\":\"len(obj)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"len(<array or string>)\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Returns the length of \"},{\"text\":\"obj\",\"color\":\"Green\"},{\"text\":\", if it is a string or an array. If not it will throw an error.\"}]}]}")]
		public static int Len(object obj)
		{
			if (obj == null)
			{
				throw new InterpreterException("len: cannot get the length of null!");
			}
			if (obj is System.Collections.IEnumerable ie)
			{
				return ie.Cast<object>().Count();
			}
			else if (obj is string s)
			{
				return s.Length;
			}
			throw new InterpreterException("len: object passed was not an array or a string!");
		}

		[ManualDocs("chars", "{\"title\":\"chars(str)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"chars(<string>)\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Returns an array of every character in \"},{\"text\":\"str\",\"color\":\"Green\"}]}]}")]
		public static object[] Chars(string s)
		{
			if (s == null)
			{
				throw new InterpreterException("chars: string passed in was null!");
			}
			return Utils.ToArray(s.ToArray());
		}

		[ManualDocs("toString", "{\"title\":\"toString(obj)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"chars(<string>)\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Converts \"},{\"text\":\"obj\",\"color\":\"Green\"},{\"text\":\" to a string. If \"},{\"text\":\"obj\",\"color\":\"Green\"},{\"text\":\" is an array it will take the following format:\n\"},{\"text\":\"Array [stuff, goes, here, etc]\",\"color\":\"Cyan\"}]}]}")]
		public static string _7sToString(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			if (obj is string s)
			{
				return s;
			}
			if (obj is System.Collections.IEnumerable ie)
			{
				return $"[{string.Join(", ", Utils.ToArray(ie))}]";
			}
			return obj.ToString();
		}

		[ManualDocs("arrayAdd", "{\"title\":\"arrayAdd(arr, value)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"arrayAdd(<array>, <value>)\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Returns \"},{\"text\":\"array\",\"color\":\"Green\"},{\"text\":\" with \"},{\"text\":\"value\",\"color\":\"Green\"},{\"text\":\" added to the end.\"}]}]}")]
		public static object[] ArrayAdd(object[] arr, object value)
		{
			if (arr == null)
			{
				throw new InterpreterException("arrayAdd: Cannot add to an array that is null!");
			}
			Array.Resize(ref arr, arr.Length + 1);
			arr[arr.Length - 1] = value;
			return arr;
		}

		[ManualDocs("arrayRemove", "{\"title\":\"arrayRemove(arr, index)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"arrayAdd(<array>, <index>)\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Returns \"},{\"text\":\"array\",\"color\":\"Green\"},{\"text\":\", but the element at index \"},{\"text\":\"index\",\"color\":\"Green\"},{\"text\":\" is removed.\"}]}]}")]
		public static object[] ArrayRemove(object[] arr, int index)
		{
			if (arr == null)
			{
				throw new InterpreterException("arrayRemove: Cannot remove from an array that is null!");
			}
			if (index < 0 || index >= arr.Length)
			{
				throw new InterpreterException("Attempted to remove an element that is not in bounds of the array");
			}
			return arr.Take(index).Concat(arr.Skip(index + 1)).ToArray();
		}

		[ManualDocs("sqrt", "{\"title\":\"sqrt(x)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"sqrt(<number>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Takes the square root of\"},{\"text\":\"x\",\"color\":\"Green\"},{\"text\":\".\"}]}]}")]
		public static double Sqrt(double x)
		{
			if (x < 0)
			{
				throw new InterpreterException("sqrt: Complex numbers are not supported! (Attempted to take the square root of a negative number)");
			}
			return Math.Sqrt(x);
		}

		[ManualDocs("pow", "{\"title\":\"pow(x, n)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"pow(<number>, <number>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Raise \"},{\"text\":\"x\",\"color\":\"Green\"},{\"text\":\" to the power of \"},{\"text\":\"n\",\"color\":\"Green\"}]}]}")]
		public static double Pow(double x, double y)
		{
			if (x == 0 && y == 0)
			{
				throw new InterpreterException("pow: Cannot raise 0 to the 0th power!");
			}
			return Math.Pow(x, y);
		}

		[ManualDocs("fgColor", "{\"title\":\"fgColor(color)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"fgColor(<color number or name>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Set the text color to \"},{\"text\":\"color\",\"color\":\"Green\"}]},{\"header\":\"Valid colors\",\"text\":[{\"text\":\"BLACK BLUE CYAN DARK_BLUE DARK_CYAN DARK_GRAY DARK_GREEN DARK_MAGENTA DARK_RED DARK_YELLOW GRAY GREEN MAGENTA RED WHITE YELLOW\",\"color\":\"Cyan\"}]}]}")]
		public static void FgColor(int color)
		{
			if (color < 0 || color > 15)
			{
				throw new InterpreterException("fgColor: color was invalid! Must be from 0 to 15!");
			}
			Console.ForegroundColor = (ConsoleColor)color;
		}

		[ManualDocs("bgColor", "{\"title\":\"bgColor(color)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"bgColor(<color number or name>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Set the background color to \"},{\"text\":\"color\",\"color\":\"Green\"},{\"text\":\". call \"},{\"text\":\"clear()\",\"color\":\"Yellow\"},{\"text\":\"to if you want to set the background color of the whole screen, because \"},{\"text\":\"bgColor(color)\",\"color\":\"Yellow\"},{\"text\":\" only affects the background color of new printed text.\"}]},{\"header\":\"Valid colors\",\"text\":[{\"text\":\"BLACK BLUE CYAN DARK_BLUE DARK_CYAN DARK_GRAY DARK_GREEN DARK_MAGENTA DARK_RED DARK_YELLOW GRAY GREEN MAGENTA RED WHITE YELLOW\",\"color\":\"Cyan\"}]}]}")]
		public static void BgColor(int color)
		{
			if (color < 0 || color > 15)
			{
				throw new InterpreterException("bgColor: color was invalid! Must be from 0 to 15!");
			}
			Console.BackgroundColor = (ConsoleColor)color;
		}

		[ManualDocs("resetColor", "{\"title\":\"resetColor()\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"resetColor();\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Reset the console colors back to their defaults.\"}]}]}")]
		public static void ResetColor() => Console.ResetColor();
	}
}