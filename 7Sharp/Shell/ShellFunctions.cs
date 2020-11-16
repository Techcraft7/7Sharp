using _7Sharp._7sLib;
using _7Sharp.Manual;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Techcraft7_DLL_Pack.Text;

namespace _7Sharp.Shell
{
	using static ColorConsoleMethods;
	using static ConsoleColor;
	internal sealed partial class Shell
	{
		private void Man(string[] args)
		{
			switch (args.Length)
			{
				case 1:
					if (args[0].ToLower().Equals("list"))
					{
						foreach (string key in Documentation.Keys)
						{
							WriteLineColor(key, Cyan);
						}
						return;
					}	
					string topic = args[0].ToLower();
					if (Documentation.ContainsKey(topic))
					{
						try
						{
							ManualObject obj = JsonConvert.DeserializeObject<ManualObject>(Documentation[topic]);
							Console.WriteLine(obj.title);
							Console.WriteLine("--------------\n");
							foreach (ManualSection s in obj.sections)
							{
								Console.WriteLine(s.header);
								Console.WriteLine();
								foreach (ManualTextComponent c in s.text)
								{
									if (c.color == null)
									{
										c.color = Gray.ToString();
									}
									if (c.back == null)
									{
										c.back = Black.ToString();
									}
									Console.BackgroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), c.back);
									Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), c.color);
									Console.Write(c.text);
									Console.ResetColor();
								}
								Console.WriteLine();
							}
						}
						catch (Exception e)
						{
							Utils.PrintError(e);
						}
					}
					else
					{
						WriteLineColor("Topic not found!\nType man list for a list of topics!", Red);
						return;
					}
					break;
				default:
					WriteLineColor("Invalid syntax! man <topic>\nType man list for a list of topics!", Red);
					break;
			}
		}

		private void Export(string[] args) => _7sLibManager.Save(string.Join(" ", args));

		private void Load(string[] args)
		{
			string path = string.Join(" ", args);
			if (File.Exists(path))
			{
				try
				{
					using (StreamReader sr = new StreamReader(path))
					{
						SetCode(sr.ReadToEnd());
					}
					WriteLineColor("File loaded into the editor!", Green);
				}
				catch (Exception e)
				{
					WriteLineColor($"An error occured loading the file into the editor! {e.GetType()}: {e.Message}", Red);
				}
			}
			else
			{
				WriteLineColor("Could not find the file or the path was invalid!", Red);
			}
		}

		private void Save(string[] args)
		{
			bool replace = false;
			if (args[0] == "-o")
			{
				replace = true;
				args = args.Skip(1).ToArray();
			}
			string path = string.Join(" ", args);
			if (File.Exists(path) && !replace)
			{
				WriteLineColor("The file already exists! Do save -o <path> to overrite files!", Red);
			}
			else
			{
				try
				{
					using (StreamWriter sw = new StreamWriter(path))
					{
						sw.Write(GetCode());
					}
					WriteLineColor("Code saved!", Green);
				}
				catch
				{
					WriteLineColor("An error occured saving the file! Is the path valid?", Red);
				}
			}
		}

		private void Help(string[] args)
		{
			for (int i = 0; i < commands.Keys.Count; i++)
			{
				CommandInfo key = commands.Keys.ToArray()[i];
				if (key == null)
				{
					key = new CommandInfo(" NULL COMMAND ", " The command help could not be loaded");
				}
				if (commands[key] == null)
				{
					commands[key] = new Action<string[]>(_ => { WriteLineColor("The command code could not be loaded", Red); });
				}
				bool isSys = key.GetType() == typeof(SysCommandInfo);
				WriteLineMultiColor(new string[] { isSys ? "(System) " : string.Empty, key.Name + ": ", key.Help }, new ConsoleColor[] { Green, Yellow, Cyan });
			}
		}

		private void RunCode(string[] args) => interpreter.Run(GetCode());

		private void Edit(string[] args) => _ = editor.ShowDialog();
	}
}
