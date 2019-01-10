///library from codeproject used! Link: https://www.codeproject.com/Articles/9114/math-function-boolean-string-expression-evaluator

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Techcraft7_DLL_Pack;
using ExpressionEvaluation;

namespace _7Sharp
{
	using static Program;
	using static ColorConsoleMethods;
	using static Console;
	class Command
	{
		public override string ToString()
		{
			return string.Format("{0} -  {1}", call, help);
		}
		protected string call { get; set; }
		protected string help { get; set; }
		public virtual void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				//Code goes here
			}
			else
			{
				return;
			}
		}
	}
	class StartWithArgs : Command
	{
		public StartWithArgs()
		{
			call = "startargs";
			help = "starts a command with args: startargs \"<path>\" \"<args>\" (path and args MUST be in quotes)";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				/*
				Syntax:
				startargs "<path>" "<args>"
				*/
				string[] quoted = input.Split('\"');
				try
				{
					if (quoted[0].EndsWith(" ") == true && quoted[2][0] == ' ') //if there is a space after "startargs" and between "<path>" and "<args>", continue, otherwise: throw FormatException
					{
						//quoted[1] = path
						//quoted[3] = args 
						Process.Start(quoted[1], quoted[3]);
					}
					else
					{
						throw new FormatException("Expected \' \' after \'startargs\' and between \"<path>\" and \"<args>\"");
					}
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("Oops! An error of type {1} occured: {0}", e.Message, e.GetType().ToString()), ConsoleColor.Red);
				}
			}
			else
			{
				return;
			}
		}
	}
	class Write : Command
	{
		public Write()
		{
			call = "write";
			help = "prints text: write <text> OR write & <string name>";
		}
		public override void Parse()
		{
			if (input_split.Length >= 2 && input_split[0].ToLower() == call)
			{
				if (input_split.Length >= 2 && input_split[1] != "&" && input_split[1] != "$")
				{
					string output = "";
					bool first = true;
					foreach (string i in input_split)
					{
						List<string> arguments = new List<string>();
						if (first == true)
						{
							first = false;
							continue;
						}
						else
						{
							arguments.Add(i);
						}
						foreach (string a in arguments)
						{
							output += a;
							if (a != input_split[input_split.Length - 1].ToLower())
							{
								output += " ";
							}
						}
					}
					WriteLine(output);
				}
				else if (input_split[1] == "$")
				{
					string output = "";
					foreach (VarInt i in ints)
					{
						if (input_split[2] == i.name)
						{
							output = i.value.ToString();
						}
					}
					WriteLine(output);
				}
				else
				{
					if (input_split.Length == 3)
					{
						foreach (VarString i in strings)
						{
							if (i.name == input_split[2])
							{
								WriteLine(i.value);
							}
						}
					}
				}
			}
			else
			{
				return;
			}
		}
	}
	class StringSet : Command
	{
		public StringSet()
		{
			call = "string";
			help = "sets a string to a variable: string <name> <value>";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				if (input_split.Length >= 3)
				{
					string val = "";
					int x = 1;
					foreach (string i in input_split)
					{
						if (x >= 3)
						{
							val += i + " ";
						}
						x++;
					}
					strings.Add(new VarString(input_split[1], val));
					if (echo)
						WriteLineColor("Stored...", ConsoleColor.Yellow);
				}
				else
				{
					WriteLineColor("An error occured...", ConsoleColor.Red);
				}
			}
		}
	}
	class VarStringSet : Command
	{
		public VarStringSet()
		{
			call = "&";
			help = "sets a string\'s value: & <name> <value>";
		}
		public override void Parse()
		{
			if (input_split.Length >= 4 && input_split[0].ToLower() == call)
			{
				foreach (VarString i in strings)
				{
					if (input.Split(' ', '\n')[2] == i.name)
					{
						if (input_split[1] == "set")
						{
							string val = "";
							int x = 0;
							foreach (string y in input.Split(' ', '\n'))
							{
								if (x >= 3)
								{
									val += y + " ";
								}
								x++;
							}
							i.value = val;
							if (echo)
								WriteLineColor("Set value...", ConsoleColor.Yellow);
						}
						else if (input_split[1] == i.name && input_split[2] == "add")
						{
							i.value += input_split[3];
							if (echo)
								WriteLineColor("Set value...", ConsoleColor.Yellow);
						}
					}
				}
			}
			else
			{
				return;
			}
		}
	}
	class Add : Command
	{
		public Add()
		{
			call = "add";
			help = "adds a value to a number: add <name> <value>";
		}
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				if (input_split.Length == 3 && input_split[0].ToLower() == call)
				{
					foreach (VarInt i in ints)
					{
						if (input_split[1] == i.name)
						{
							try
							{
								int x = Convert.ToInt32(input_split[2]);
								i.value = i.value + x;
								if (echo)
								{
									WriteLineColor("Added value...", ConsoleColor.Yellow);
								}
							}
							catch (Exception e)
							{
								WriteLineColor(string.Format("An error occured of type {1}: {0}", e.Message, e.GetType().ToString()), ConsoleColor.Red);
							}
						}
						return;
					}
				}
				else
				{
					WriteLineColor("Oopsie! you did something wrong!", ConsoleColor.Red);
				}
			}
			return;
		}
	}
	class VarIntSet : Command
	{
		public VarIntSet()
		{
			call = "$";
			help = "sets an ints value, same syntax as &: $ <name> <value>";
		}
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				foreach (VarInt y in ints)
				{
					if (input.Split(' ', '\n')[1] == y.name)
					{
						try
						{
							y.value = Convert.ToInt32(input_split[2]);
							break;
						}
						catch (Exception e)
						{
							WriteLineColor(string.Format("An error occured: {0}", e.Message), ConsoleColor.Red);
						}
					}
				}
				if (echo)
				{
					WriteLineColor("Set value... ", ConsoleColor.Yellow);
				}
			}
			else
			{
				return;
			}
		}
	}
	class WriteFile : Command
	{
		public WriteFile()
		{
			call = "writefile";
			help = "writes a string to a file: writelfile <path> <text> (\\n and \\t allowed)";
		}
		public override void Parse()
		{
			if (input_split.Length == 2 && input_split[0].ToLower() == call)
			{
				try
				{
					StreamWriter sw = new StreamWriter(input_split[1], true);
					if (echo)
					{
						WriteLineColor("Write text (escape squences allowed (\\n \\t)):", ConsoleColor.Yellow);
					}
					string foo = has_args == true ? srr.ReadLine() : ReadLine();
					foo.Replace("\\n", "\n");
					foo.Replace("\\t", "\t");
					sw.Write(foo);
					sw.Close();
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("An error occured: {0}", e.Message), ConsoleColor.Red);
				}
			}
			else
			{
				return;
			}
		}
	}
	class ReadFile : Command
	{
		public ReadFile()
		{
			call = "readfile";
			help = "reads text from file: readfile <path>";
		}
		private string GetExtension(string path)
		{
			string[] period = { "." };
			return input.Split(period, StringSplitOptions.None)[path.Split(period, StringSplitOptions.None).Length - 1];
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				try
				{
					string p = "";
					int x = 0;
					foreach (string i in input_split)
					{
						if (x == 0)
						{
							x++;
							continue;
						}
						p += i + " ";
					}
					char[] invchars = "`1234567890-=~!@#$%^&*()_+qwertyuiop[]\\QWERTYUIOP{}|asdfghjkl;\'ASDFGHJKL:\"zxcvbnm,./ZXCVBNM<>?ñ¡¿\n\t".ToCharArray();
					foreach (char i in File.ReadAllText(p))
					{
						foreach (char z in invchars)
						{
							if (i != z)
							{
								WriteLineColor(string.Format("Error: 7Sharp cannot read the specified file with type of .{0} extension because it contains the unreadable character {1}...", GetExtension(p), i), ConsoleColor.Red);
								return;
							}
						}
					}
					StreamReader sr = new StreamReader(input_split[1]);
					do
					{
						WriteLine(sr.ReadLine());
					}
					while (sr.EndOfStream == false);
					sr.Close();
					sr.Dispose();
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("An error occured: {0}", e.Message), ConsoleColor.Red);
				}
			}
			else
			{
				return;
			}
			return;
		}
	}
	class Int : Command
	{
		public Int()
		{
			call = "int";
			help = "store a value as an int: int <value> <name>";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				if (input_split.Length != 3)
				{
					WriteLineColor("Format Exception: int takes 2 arguments: int <value> <name>", ConsoleColor.Red);
				}
				else
				{
					try
					{
						ints.Add(new VarInt(Convert.ToInt32(input_split[1]), input_split[2]));
						if (echo)
							WriteLineColor("Set value...", ConsoleColor.Yellow);
					}
					catch (Exception e)
					{
						WriteLineColor("Format Exception: invalid number", ConsoleColor.Red);
					}
				}
			}

		}
	}
	class Exit : Command
	{
		public Exit()
		{
			call = "exit";
			help = "exits 7Sharp";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				WriteLine("Press enter to continue...");
				ReadLine();
				Environment.Exit(0);
			}
		}
	}
	class Help : Command
	{
		public Help()
		{
			call = "help";
			help = "shows this text";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				foreach (Command i in commands)
				{
					Console.WriteLine(i.ToString());
				}
			}
		}
	}
	class Clear : Command
	{
		public Clear()
		{
			call = "clear";
			help = "clears the screen";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				Clear();
			}
		}
	}
	class Open : Command
	{
		public Open()
		{
			call = "open";
			help = "opens a program: open <path>";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				try
				{
					string p = "";
					int x = 0;
					foreach (string i in input_split)
					{
						if (x == 0)
						{
							x++;
							continue;
						}
						p += i + " ";
					}
					Process.Start(p);
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("An error occured: {0}", e.Message), ConsoleColor.Red);
				}
			}
			else
			{
				return;
			}
		}
	}
	class Loop : Command
	{
		public Loop()
		{
			call = "loop";
			help = "loops commands: loop <times> OR loop $ <int name> (enter) (type command, press enter, when done, type \"end\" and press enter)";
		}
		public override void Parse()
		{
			bool invalid = false;
			if (input_split[0].ToLower() == call)
			{
				try
				{
					if (input_split[0].ToLower() == call)
					{
						try
						{
							if (input_split[1] == "$")
							{
								if (!(input_split.Length > 3))
								{
									int count = 0;
									do
									{
										times = ints[count].name == input_split[2] ? ints[count].value : 1;
										count++;
									}
									while (count < ints.Count);
									List<string> z = new List<string>();
									bool retu = false;
									ret:
									if (retu == true)
									{
										return;
									}
									while (input != "end")
									{
										if (echo)
										{
											WriteLine("Loop Commands (type end to exit):");
										}
										input = has_args == true ? srr.ReadLine() : ReadLine();
										if (input.ToLower().Contains("loop") || input.ToLower().Contains("while") || input.ToLower().Contains("if"))
										{
											throw new InvalidOperationException("NO LOOPS, IFS, OR WHILE COMMANDS!");
											retu = true;
											goto ret;
										}
										else
										{
											z.Add(input);
										}
									}
									for (int i = 0; i < times; i++)
									{
										foreach (string x in z)
										{
											input_split = x.Split(' ', '\n');
											foreach (InstalledPackageCmd c in custom_cmds)
											{
												c.Parse();
											}
											foreach (Command c in commands)
											{
												c.Parse();
											}
										}
									}
									times = 0;
								}
								else
								{
									throw new FormatException("Too many arguments...");
								}
							}
							else
							{
								if (!(input_split.Length > 2))
								{
									times = Convert.ToInt32(input_split[1]);
									List<string> z = new List<string>();
									while (input != "end")
									{
										if (echo)
										{
											WriteLine("Loop Commands (type end to exit):");
										}
										input = has_args == true ? srr.ReadLine() : ReadLine();
										if (input.ToLower().Contains("loop") || input.ToLower().Contains("while") || input.ToLower().Contains("if"))
										{
											throw new InvalidOperationException("NO LOOPS, IFS, OR WHILE COMMANDS!");
										}
										else
										{
											z.Add(input);
										}
									}
									for (int i = 0; i < times; i++)
									{
										foreach (string x in z)
										{
											input_split = x.Split(' ', '\n');
											foreach (InstalledPackageCmd c in custom_cmds)
											{
												c.Parse();
											}
											foreach (Command c in commands)
											{
												c.Parse();
											}
										}
									}
									times = 0;
								}
								else
								{
									throw new FormatException("Too many arguments...");
								}
							}
						}
						catch (Exception e)
						{
							WriteLineColor(string.Format("An error occured of {1}: {0}", e.Message, e.GetType().ToString()), ConsoleColor.Red);
							return;
						}
					}
					else
					{
						return;
					}
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("An error occured of {1}: {0}", e.Message, e.GetType().ToString()), ConsoleColor.Red);
					return;
				}
			}
			else
			{
				return;
			}
		}
	}
	class _7sRandom : Command
	{
		public _7sRandom()
		{
			call = "random";
			help = "sets an int\'s value to a random number between a minimum and maximum: random <min> <max> <int name>";
		}
		public override void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				try
				{
					Random rng = new Random();
					int min = Convert.ToInt32(input_split[1]);
					int max = Convert.ToInt32(input_split[2]) + 1;
					foreach (VarInt i in ints)
					{
						if (i.name == input_split[3])
						{
							i.value = rng.Next(min, max);
							if (echo)
								WriteLineColor(string.Format("The random command set the value of {0} to {1}", i.name, i.value), ConsoleColor.Yellow);
							System.Threading.Thread.Sleep(15);
						}
					}
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("An error occured of type {0}: {1}", e.GetType().ToString(), e.Message), ConsoleColor.Red);
				}
			}
			else
			{
				return;
			}
		}
	}
	class If : Command
	{
		public If()
		{
			call = "if";
			help = "executes a command if the condition matches the input boolean value, NO NESTED IF STATEMENTS!!!: if <true/false> (enter) <condition> (enter) <command to execute if condition is true/false>";
		}
		private int x = 0;
		private string eq = "";
		private bool y = false;
		ExpressionEval ee = null;
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				try
				{
					x = 0;
					y = false;
					eq = "";
					if (Convert.ToBoolean(input.Split(' ', '\n')[1]) == true)
					{
						y = true;
					}
					else if (Convert.ToBoolean(input.Split(' ', '\n')[1]) == false)
					{
						y = false;
					}
					else
					{
						throw new FormatException("That\'s not a boolean silly! Should be true or false!");
					}
					if (echo)
					{
						WriteLine("Condition:");
					}
					eq = has_args == true ? srr.ReadLine() : ReadLine();
					if (echo)
					{
						WriteLine("Command to execute if condition is {0}:", y);
					}
					ee = new ExpressionEval(eq);
					input = has_args == true ? srr.ReadLine() : ReadLine();
					input_split = input.Split(' ', '\n');
					if (Convert.ToBoolean(ee.EvaluateBool()) == Convert.ToBoolean(y))
					{
						foreach (Command i in commands)
						{
							if (i.GetType() != typeof(If))
								i.Parse();
						}
					}
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("An error occured of type {0}: {1}", e.GetType().ToString(), e.Message), ConsoleColor.Red);
				}
			}
		}
	}
	class Com : Command
	{
		public Com()
		{
			call = "com";
			help = "displays infomation about your computer and performs actions: com <time/date/os/cominfo/shutdown/restart/logoff>";
		}
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				switch (input_split[1])
				{
					case "time":
						DateTime time = DateTime.UtcNow;
						WriteLine(time.ToLocalTime().ToShortTimeString());
						break;
					case "date":
						DateTime date = DateTime.Today;
						WriteLine(date.ToShortDateString());
						break;
					case "os":
						OperatingSystem os = Environment.OSVersion;
						WriteLine("OS Version: " + os.Version.ToString());
						break;
					case "cominfo":
						//credit https://www.c-sharpcorner.com/uploadfile/mahesh/get-operating-system-data-and-version-in-C-Sharp/
						WriteLine("Operating System Details:");
						OperatingSystem cominfo = Environment.OSVersion;
						WriteLine("OS Version: " + cominfo.Version.ToString());
						WriteLine("OS Platoform: " + cominfo.Platform.ToString());
						WriteLine("OS SP: " + cominfo.ServicePack.ToString());
						WriteLine("OS Version String: " + cominfo.VersionString.ToString());
						break;
					case "shutdown":
						Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + @"\shutdown.exe", "-s -t 00 -c \"7Sharp has done this\"");
						break;
					case "restart":
						Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + @"\shutdown.exe", "-r -t 00 -c \"7Sharp has done this\"");
						break;
					case "logoff":
						Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) + @"\shutdown.exe", "-l -t 00 -c \"7Sharp has done this\"");
						break;
				}
			}
		}
	}
	class CustomCommandBase : ICommandShare
	{
		protected string call { get; set; }
		internal List<string> commands = new List<string>();
		public virtual void Parse()
		{
			if (input_split[0].ToLower() == call)
			{
				//Code goes here
			}
			else
			{
				return;
			}
		}
	}
	class InstallPkg : Command, ICommandShare
	{
		public InstallPkg()
		{
			call = "install-pkg";
			help = "installs a package: install-pkg <path>";
		}
		private int x = 0;
		private StreamReader sr;
		private string path;
		public override void Parse()
		{
			sr = null;
			path = "";
			x = 0;
			if (input_split[0] == call)
			{
				foreach (string i in input_split)
				{
					if (x == 0)
					{
						x++;
						continue;
					}
					else
					{
						path += i + " ";
						x++;
					}
				}
				try
				{
					sr = new StreamReader(path);
					InstalledPackageCmd cmd = new InstalledPackageCmd(sr.ReadLine());
					while (sr.EndOfStream == false)
					{
						cmd.commands.Add(sr.ReadLine().Replace("\\~", " "));
					}
					sr.Close();
					sr.Dispose();
					custom_cmds.Add(cmd);
					if (echo)
					{
						WriteLineColor("Installed Package!", ConsoleColor.Yellow);
					}
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("Error occured of type {0}: {1}...", e.GetType().ToString(), e.Message), ConsoleColor.Red);
				}
			}
		}
	}
	class InstalledPackageCmd : CustomCommandBase, ICommandShare
	{
		public InstalledPackageCmd(string call)
		{
			this.call = call;
		}
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				foreach (string i in commands)
				{
					input = i;
					//				input_split = input.Split(' ', '\n');
					foreach (Command cmd in Program.commands)
					{
						cmd.Parse();
					}
				}
			}
		}
	}
	class Out : Command
	{
		public Out()
		{
			call = "out";
			help = "disables annoying messages: out <on/off>";
		}
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				if (!(input_split.Length == 2))
				{
					WriteLineColor("You did the command wrong... syntax: out <on/off>" , ConsoleColor.Red);
					return;
				}
				if (input_split[1] == "off" && input_split.Length == 2)
				{
					echo = false;
				}
				else if (input_split[1] == "on" && input_split.Length == 2)
				{
					echo = true;
				}
				else
				{
					WriteLineColor("Usage: out <on/off>", ConsoleColor.Red);
				}
			}
		}
	}
	class WhileInt : Command
	{
		public WhileInt()
		{
			call = "while";
			help = "loops until the expression is ture or false: while <expression (use $<intname>$ to get int value)> (enter) <true/false> (enter) <command> (enter)";
		}
		private string eq = "";
		private int x = 0;
		private string eq_noformat = "";
		private bool y = false;
		private ExpressionEval ee = null;
		private List<string> replace = null;
		private int n = 0;
		private bool fail = false;
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				try
				{
					//usage: while$ <expression>, use $<int name>$ to get an ints value
					eq = "";
					fail = false;
					eq_noformat = "";
					n = 0;
					replace = new List<string>();
					x = 0;
					ee = null;
					y = false;
					string[] sep = { "$" };
					fail:
					if (fail == true)
					{
						return;
					}
					if (input_split[0] == call)
					{
						foreach (string i in input_split)
						{
							if (x == 0)
							{
								x++;
								continue;
							}
							eq += i;
							x++;
						}
						eq_noformat = eq;
						foreach (string text in eq.Split(sep, StringSplitOptions.RemoveEmptyEntries))
						{
							foreach (VarInt foo in ints)
							{
								if (foo.name == text)
								{
									replace.Add(foo.value.ToString());
								}
								else
								{
									replace.Add(text);
								}
								n++;
							}
							n = 0;
						}
						foreach (string i in replace)
						{
							eq += i;
						}
						if (echo)
						{
							WriteLine("If {0} == : (true/false)", eq);
						}
						y = Convert.ToBoolean(has_args == true ? srr.ReadLine() : ReadLine());
						if (echo)
						{
							WriteLine("Command to execute if {0} == {1}", eq, y);
						}
						input = has_args == true ? srr.ReadLine() : ReadLine();
						input_split = input.Split(' ', '\n');
						ee = new ExpressionEval(eq);
						if (input.ToLower().Contains("loop") || input.ToLower().Contains("while") || input.ToLower().Contains("if"))
						{
							try
							{
								throw new InvalidOperationException("NO LOOPS, IFS, OR WHILE COMMANDS!");
							}
							catch (Exception e)
							{
								WriteLineColor(string.Format("Error occured of type {0}: {1}...", e.GetType().ToString(), e.Message), ConsoleColor.Red);
								return;
							}
						}
						while (ee.EvaluateBool() == y)
						{
							foreach (Command i in commands)
							{
								i.Parse();
							}
							foreach (InstalledPackageCmd i in custom_cmds)
							{
								i.Parse();
							}
						}
					}
				}
				catch (Exception e)
				{
					WriteLineColor(string.Format("Error occured of type {0}: {1}...", e.GetType().ToString(), e.Message), ConsoleColor.Red);
				}
			}
		}
	}
	interface ICommandShare
	{

	}
}