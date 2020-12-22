using _7Sharp.Manual;
using System;
using System.Linq;
using System.Threading;

namespace _7Sharp.Intrerpreter
{
	internal class SysFunctions
	{
		[ManualDocs("write", "{\"title\":\"write(value)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"Outputs \"},{\"text\":\"value\",\"color\":\"Green\"},{\"text\":\" to the console, followed by a newline.\"}]}]}")]
		public static void Write(object obj) => Console.WriteLine(obj);
		
		[ManualDocs("writeraw", "{\"title\":\"writeraw(value)\",\"sections\":[{\"header\":\"Behavior\",\"text\":[{\"text\":\"Like \"},{\"text\":\"write(value)\",\"color\":\"Green\"},{\"text\":\", but it does not start a new line\"}]}]}")]
		public static void WriteRaw(object obj) => Console.Write(obj);

		[ManualDocs("read", "{\"title\":\"read()\",\"sections\":[{\"header\":\"Behavior\",\"text\":[{\"text\":\"Asks the user for input, and returns a string of the text provided.\"}]}]}")]
		public static string Read() => Console.ReadLine();

		[ManualDocs("trig", "{\"title\":\"trig\",\"sections\":[{\"header\":\"Note\",\"text\":[{\"text\":\"ALL TRIG FUNCTIONS ARE IN RADIANS!\",\"color\":\"Red\"}]},{\"header\":\"sin(x)\",\"text\":[{\"text\":\"Returns the sine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"cos(x)\",\"text\":[{\"text\":\"Returns the cosine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"tan(x)\",\"text\":[{\"text\":\"Returns the tangent of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"sin(x)\",\"text\":[{\"text\":\"Returns the sine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"cos(x)\",\"text\":[{\"text\":\"Returns the cosine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"tan(x)\",\"text\":[{\"text\":\"Returns the tangent of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"atan2(x, y)\",\"text\":[{\"text\":\"Returns the angle between (0, 0) and \"},{\"color\":\"Green\",\"text\":\"(x, y)\"},{\"text\":\" in radians.\"}]},{\"header\":\"Degrees and radians conversion\",\"text\":[{\"text\":\"Use \"},{\"text\":\"deg2rad(x)\",\"color\":\"Green\"},{\"text\":\" and \"},{\"text\":\"rad2deg(x)\",\"color\":\"Green\"},{\"text\":\" to convert \"},{\"text\":\"x\",\"color\":\"Green\"},{\"text\":\" from radians to degrees, or vice. versa. \"}]}]}")]
		public static double Sin(double v) => Math.Sin(v);
		
		public static double Cos(double v) => Math.Cos(v);
		
		public static double Tan(double v) => Math.Tan(v);

		public static double Asin(double v) => Math.Asin(v);
		
		public static double Acos(double v) => Math.Acos(v);
		
		public static double Atan(double v) => Math.Atan(v);
		
		public static double Atan2(double x, double y) => Math.Atan2(x, y);

		public static double Deg2Rad(double v) => v * Math.PI / 180D;
		
		public static double Rad2Deg(double v) => v * 180D / Math.PI;

		[ManualDocs("sleep", "{\"title\":\"sleep(ms)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"sleep(<number of miliseconds to wait>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Waits for \"},{\"text\":\"ms\",\"color\":\"Green\"},{\"text\":\" miliseconds.\"}]}]}")]
		public static void Sleep(int ms) => Thread.Sleep(ms);

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
	}
}