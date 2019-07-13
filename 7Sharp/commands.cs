///library from codeproject used! Link: https://www.codeproject.com/Articles/9114/math-function-boolean-string-expression-evaluator

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Techcraft7_DLL_Pack;
using ExpressionEvaluation;
using System.Windows.Forms;
using _7Sharp.API;
using Sys = _7Sharp.Program;

namespace _7Sharp
{
    using static InternalEnv;
	using static ColorConsoleMethods;
	using static Console;
	class StartWithArgs : Command, ISysCommand
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
	class Write : Command, ISysCommand
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
					foreach (_7sInt i in ints)
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
						foreach (_7sString i in strings)
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
	class StringSet : Command, ISysCommand
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
					strings.Add(new _7sString(input_split[1], val));
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
	class VarStringSet : Command, ISysCommand
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
				foreach (_7sString i in strings)
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
	class Add : Command, ISysCommand
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
					foreach (_7sInt i in ints)
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
	class VarIntSet : Command, ISysCommand
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
				foreach (_7sInt y in ints)
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
	class WriteFile : Command, ISysCommand
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
                    string foo = has_args == true ? srr.ReadLine() : RunningEnvCode == true ? EnvCode[EnvCodeIndex + 1] : ReadLine();
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
	class ReadFile : Command, ISysCommand
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
	class Int : Command, ISysCommand
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
						ints.Add(new _7sInt(Convert.ToInt32(input_split[1]), input_split[2]));
						if (echo)
							WriteLineColor("Set value...", ConsoleColor.Yellow);
					}
					catch (Exception e)
					{
						WriteLineColor(e.GetType() + ": " + e.Message, ConsoleColor.Red);
					}
				}
			}

		}
	}
	class Exit : Command, ISysCommand
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
				Environment.Exit(0);
			}
		}
	}
	class Help : Command, ISysCommand
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
	class Clear : Command, ISysCommand
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
	class Open : Command, ISysCommand
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
	class Loop : Command, ISysCommand
	{
		public Loop()
		{
			call = "loop";
			help = "loops commands: loop <times> OR loop $ <int name> (enter) (type command, press enter, when done, type \"end\" and press enter)";
		}
		public override void Parse()
		{
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
									if (retu == true)
									{
										return;
									}
									while (input != "end")
									{
										if (echo)
										{
											WriteLine("Loop Commands (type end to exit loop):");
                                            Sys.UpdateInputFromEnvCode();
                                        }
										else
										{
											z.Add(input);
                                            Sys.UpdateInputFromEnvCode();

                                        }
									}
									for (int i = 0; i < times; i++)
									{
										foreach (string x in z)
										{
											Sys.input_split = x.Split(' ', '\n');
                                            Sys.UpdateEnvironment();
                                            
											foreach (Command c in commands)
											{
												c.Parse();
											}
										}
									}
									Sys.times = 0;
                                    Sys.UpdateEnvironment();
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
									Sys.times = Convert.ToInt32(input_split[1]);
                                    Sys.UpdateEnvironment();
									List<string> z = new List<string>();
									while (input != "end")
									{
										if (echo)
										{
											WriteLine("Loop Commands (type end to exit):");
										}
                                        Sys.UpdateInputFromEnvCode();
                                        z.Add(input);
									}
									for (int i = 0; i < times; i++)
									{
										foreach (string x in z)
										{
                                            Sys.input = x;
											Sys.input_split = x.Split(' ', '\n');
                                            Sys.UpdateEnvironment(true);
											foreach (Command c in commands)
											{
												c.Parse();
											}
										}
									}
									Sys.times = 0;
                                    Sys.UpdateEnvironment();
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
	class _7sRandom : Command, ISysCommand
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
					foreach (_7sInt i in ints)
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
	class If : Command, ISysCommand
	{
		public If()
		{
			call = "if";
			help = "executes a command if the condition matches the input boolean value, NO NESTED IF STATEMENTS!!!: if <true/false> (enter) <condition> (enter) <command to execute if condition is true/false>";
		}
		private string eq = "";
		private bool y = false;
		ExpressionEval ee = null;
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				try
				{
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
                    Sys.UpdateInputFromEnvCode();
                    eq = input;
					if (echo)
					{
						WriteLine("Command to execute if condition is {0}:", y);
					}
					ee = new ExpressionEval(eq);
                    Sys.UpdateInputFromEnvCode();
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
	class Com : Command, ISysCommand
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
	class InstallPkg : Command, ISysCommand, ICommandShare
	{
		public InstallPkg()
		{
			call = "install-pkg";
			help = "installs a package: install-pkg <path> (DEPRECATED)";
		}
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				try
				{
                    throw new DeprecatedException("This command is deprecated... please use the 7Sharp API or use an ealier build!");
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
                WriteLine("Deprecated! Use the 7Sharp API!");
			}
		}
	}
	class Out : Command, ISysCommand
	{
		public Out()
		{
			call = "out";
			help = "disables annoying messages: out <on/off>";
		}
		public override void Parse()
		{
			if (input_split[0] == call && input_split.Length == 2)
			{
				if (input_split.Length != 2)
				{
					WriteLineColor("You did the command wrong... syntax: out <on/off>" , ConsoleColor.Red);
					return;
				}
				if (input_split[1] == "off" && input_split.Length == 2)
				{
					echo = false;
                    Sys.UpdateEnvironment();
				}
				else if (input_split[1] == "on" && input_split.Length == 2)
				{
					echo = true;
                    Sys.UpdateEnvironment();
                }
				else
				{
					WriteLineColor("Usage: out <on/off>", ConsoleColor.Red);
				}
			}
		}
	}
	class While : Command, ISysCommand
	{
		public While()
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
		public override void Parse()
		{
			if (input_split[0] == call)
			{
				try
				{
					//usage: while$ <expression>, use $<int name>$ to get an ints value
					eq = "";
					eq_noformat = "";
					n = 0;
					replace = new List<string>();
					x = 0;
					ee = null;
					y = false;
					string[] sep = { "$" };
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
							foreach (_7sInt foo in ints)
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
						y = Convert.ToBoolean(has_args == true ? srr.ReadLine() : RunningEnvCode == true ? EnvCode[EnvCodeIndex + 1] : ReadLine());
						if (echo)
						{
							WriteLine("Command to execute if {0} == {1}", eq, y);
						}
                        Sys.UpdateInputFromEnvCode();
						ee = new ExpressionEval(eq);
						while (ee.EvaluateBool() == y)
						{
							foreach (Command i in commands)
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