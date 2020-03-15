using System;
using System.Linq;
using System.IO;
using Techcraft7_DLL_Pack.Text;

namespace _7Sharp.Shell
{
	using static ConsoleColor;
	using static ColorConsoleMethods;
	internal partial class Shell
	{
		public void Load(string[] args)
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

		public void Save(string[] args)
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

		public void Help(string[] args) => commands.ToList().ForEach(c => { WriteLineMultiColor(new string[] { c.Key.Name + ": ", c.Key.Help }, new ConsoleColor[] { Yellow, Cyan }); });

		void RunCode(string[] args) => interpreter.Run(GetCode());

		void Edit(string[] args) => editor.ShowDialog();
	}
}
