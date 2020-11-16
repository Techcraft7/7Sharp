using _7Sharp.Manual;
using System;

namespace _7Sharp.Intrerpreter
{
	internal class SysFunctions
	{
		[ManualDocs("write", "{\"title\":\"write(value)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"Outputs \"},{\"text\":\"value\",\"color\":\"Green\"},{\"text\":\" to the console, followed by a newline.\"}]}]}")]
		public static void Write(object obj) => Console.WriteLine(obj);
		
		[ManualDocs("writeraw", "{\"title\":\"writeraw(value)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"Like \"},{\"text\":\"write(value)\",\"color\":\"Green\"},{\"text\":\", but it does not start a new line\"}]}]}")]
		public static void WriteRaw(object obj) => Console.Write(obj);
		
		[ManualDocs("trig", "{\"title\":\"trig\",\"sections\":[{\"header\":\"Note\",\"text\":[{\"text\":\"ALL TRIG FUNCTIONS ARE IN RADIANS!\",\"color\":\"Red\"}]},{\"header\":\"sin(x)\",\"text\":[{\"text\":\"Returns the sine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"cos(x)\",\"text\":[{\"text\":\"Returns the cosine of \"},{\"text\":\"x\",\"color\":\"Green\"}]},{\"header\":\"tan(x)\",\"text\":[{\"text\":\"Returns the tangent of \"},{\"text\":\"x\",\"color\":\"Green\"}]}]}")]
		public static double Sin(double v) => Math.Sin(v);
		
		public static double Cos(double v) => Math.Cos(v);
		
		public static double Tan(double v) => Math.Tan(v);

		public static double Deg2Rad(double v) => v * Math.PI / 180D;
		
		public static double Rad2Deg(double v) => v * 180D / Math.PI;
	}
}