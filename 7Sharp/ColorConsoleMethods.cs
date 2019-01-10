using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.ColorConsoleMethods
{
	class ColorConsoleMethods
	{
		public static void WriteColor(string text, ConsoleColor color)
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(text);
			Console.ForegroundColor = foregroundColor;
		}

		public static void WriteLineColor(string text, ConsoleColor color)
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(text);
			Console.ForegroundColor = foregroundColor;
		}

		public static void WriteMultiColor(string[] strings, ConsoleColor[] colors)
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				if (strings.Length >= colors.Length)
				{
					foreach (string str in strings)
					{
						Console.ForegroundColor = colors[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.Write(str);
					}
					Console.ForegroundColor = foregroundColor;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("An error has occured in ColorConsoleMethods.dll of type of:\nTechcraft7_DLL_Pack.ColorConsoleMethods.InvalidLength\nPlease make sure that the length of the input string is equal to the length of the color input array.");
					Console.WriteLine(string.Format("The length of the input string is {0}.\nThe length of the color input is {1}.", (object)strings.Length, (object)colors.Length));
					Console.Read();
				}
				Console.ForegroundColor = foregroundColor;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(ex.Message);
				Console.Read();
			}
		}

		public static void WriteLineMultiColor(string[] strings, ConsoleColor[] colors)
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				if (strings.Length >= colors.Length)
				{
					foreach (string str in strings)
					{
						Console.ForegroundColor = colors[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.Write(str);
					}
					Console.ForegroundColor = foregroundColor;
					Console.WriteLine(string.Empty);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("An error has occured in ColorConsoleMethods.dll of type of:\nTechcraft7_DLL_Pack.ColorConsoleMethods.InvalidLength\nPlease make sure that the length of the input string is equal to the length of the color input array.");
					Console.WriteLine(string.Format("The length of the input string is {0}.\nThe length of the color input is {1}.", (object)strings.Length, (object)colors.Length));
					Console.Read();
				}
				Console.ForegroundColor = foregroundColor;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(ex.Message);
				Console.Read();
			}
		}

		public static void WriteBGColor(string text, ConsoleColor color)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			Console.BackgroundColor = color;
			Console.Write(text);
			Console.BackgroundColor = backgroundColor;
		}

		public static void WriteLineBGColor(string text, ConsoleColor color)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			Console.BackgroundColor = color;
			Console.WriteLine(text);
			Console.BackgroundColor = backgroundColor;
		}

		public static void WriteMultiBGColor(string[] strings, ConsoleColor[] colors)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			try
			{
				if (strings.Length >= colors.Length)
				{
					foreach (string str in strings)
					{
						Console.BackgroundColor = colors[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.Write(str);
					}
					Console.BackgroundColor = backgroundColor;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine("An error has occured in ColorConsoleMethods.dll of type of:\nTechcraft7_DLL_Pack.ColorConsoleMethods.InvalidLength\nPlease make sure that the length of the input string is equal to the length of the color input array.");
					Console.WriteLine(string.Format("The length of the input string is {0}.\nThe length of the color input is {1}.", (object)strings.Length, (object)colors.Length));
					Console.Read();
				}
				Console.BackgroundColor = backgroundColor;
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.White;
				Console.WriteLine(ex.Message);
				Console.Read();
			}
		}

		public static void WriteLineMultiBGColor(string[] strings, ConsoleColor[] colors)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			try
			{
				if (strings.Length >= colors.Length)
				{
					foreach (string str in strings)
					{
						Console.BackgroundColor = colors[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.Write(str);
					}
					Console.BackgroundColor = backgroundColor;
					Console.WriteLine(string.Empty);
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine("An error has occured in ColorConsoleMethods.dll of type of:\nTechcraft7_DLL_Pack.ColorConsoleMethods.InvalidLength\nPlease make sure that the length of the input string is equal to the length of the color input array.");
					Console.WriteLine(string.Format("The length of the input string is {0}.\nThe length of the color input is {1}.", (object)strings.Length, (object)colors.Length));
					Console.Read();
				}
				Console.BackgroundColor = backgroundColor;
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.White;
				Console.WriteLine(ex.Message);
				Console.Read();
			}
		}

		public static void WriteBGandFGcolor(string text, ConsoleColor bg, ConsoleColor fg)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.Write(text);
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}

		public static void WriteLineBGandFGcolor(string text, ConsoleColor bg, ConsoleColor fg)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.WriteLine(text);
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}

		public static void WriteMultiBGandFGColor(string[] strings, ConsoleColor[] fgs, ConsoleColor[] bgs)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				if (strings.Length >= fgs.Length && strings.Length >= bgs.Length)
				{
					foreach (string str in strings)
					{
						Console.ForegroundColor = fgs[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.BackgroundColor = bgs[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.Write(str);
					}
					Console.BackgroundColor = backgroundColor;
					Console.ForegroundColor = foregroundColor;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.White;
					Console.WriteLine("An error has occured in ColorConsoleMethods.dll of type of:\nTechcraft7_DLL_Pack.ColorConsoleMethods.InvalidLength\nPlease make sure that the length of the input string is equal to the length of the color input array.");
					Console.WriteLine(string.Format("The length of the input string is {0}.\nThe length of the bg input is {1}.\nThe length of the fg input is {2}.", (object)strings.Length, (object)bgs.Length, (object)fgs.Length));
					Console.Read();
				}
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.White;
				Console.WriteLine(ex.Message);
				Console.Read();
			}
			finally
			{
				Console.BackgroundColor = backgroundColor;
				Console.ForegroundColor = foregroundColor;
			}
		}

		public static void WriteLineMultiBGandFGColor(string[] strings, ConsoleColor[] fgs, ConsoleColor[] bgs)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				if (strings.Length >= fgs.Length && strings.Length >= bgs.Length)
				{
					foreach (string str in strings)
					{
						Console.ForegroundColor = fgs[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.BackgroundColor = bgs[Array.IndexOf<string>(((IEnumerable<string>)strings).ToArray<string>(), str)];
						Console.Write(str);
					}
					Console.BackgroundColor = backgroundColor;
					Console.ForegroundColor = foregroundColor;
					Console.WriteLine(string.Empty);
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("An error has occured in ColorConsoleMethods.dll of type of:\nTechcraft7_DLL_Pack.ColorConsoleMethods.InvalidLength\nPlease make sure that the length of the input string is equal to the length of the color input array.");
					Console.WriteLine(string.Format("The length of the input string is {0}.\nThe length of the bg input is {1}.\nThe length of the fg input is {2}.", (object)strings.Length, (object)bgs.Length, (object)fgs.Length));
					Console.Read();
				}
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.WriteLine(ex.Message);
				Console.Read();
			}
			finally
			{
				Console.BackgroundColor = backgroundColor;
				Console.ForegroundColor = foregroundColor;
			}
		}

		public static int ReadColor(ConsoleColor color)
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			int num = Console.Read();
			Console.ForegroundColor = foregroundColor;
			return num;
		}

		public static string ReadLineColor(ConsoleColor color)
		{
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			string str = Console.ReadLine();
			Console.ForegroundColor = foregroundColor;
			return str;
		}

		public static int ReadBGColor(ConsoleColor color)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			Console.BackgroundColor = color;
			int num = Console.Read();
			Console.BackgroundColor = backgroundColor;
			return num;
		}

		public static string ReadLineBGColor(ConsoleColor color)
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			Console.BackgroundColor = color;
			string str = Console.ReadLine();
			Console.BackgroundColor = backgroundColor;
			return str;
		}
	}
}
