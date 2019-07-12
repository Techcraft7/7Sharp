using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using _7Sharp;
using Techcraft7_DLL_Pack;
using System.Reflection;
using _7Sharp.API;

namespace _7Sharp
{
    using static ColorConsoleMethods;
    using static Console;
	internal class Program
	{
		public static bool _inputs = false;
		public static string[] input_split = { };
		public static bool exit = false;
		public static bool loop = false;
		public static bool parsed = false;
		public static string input = "";
		public static bool has_args = false;
		public static List<VarInt> ints = new List<VarInt>();
		public static List<VarString> strings = new List<VarString>();
		public static StreamReader srr = null;
        public static List<Assembly> PluginAssemblies = new List<Assembly>();
		public static int times = 1;
		public static List<Command> commands = new List<Command>();
		public static bool echo = true;
		static void Main(string[] args)
		{
            //title
            Title = "7Sharp";
            //add commands
			commands.Add(new Write());
			commands.Add(new Help());
			commands.Add(new Clear());
			commands.Add(new Add());
			commands.Add(new Exit());
			commands.Add(new Loop());
			commands.Add(new Int());
			commands.Add(new StringSet());
			commands.Add(new _7sRandom());
			commands.Add(new VarStringSet());
			commands.Add(new VarIntSet());
			commands.Add(new ReadFile());
			commands.Add(new WriteFile());
			commands.Add(new InstallPkg());
			commands.Add(new Out());
			commands.Add(new Open());
			commands.Add(new Com());
			commands.Add(new While());
			commands.Add(new If());
			commands.Add(new Out());
            //initialize plugins
            WriteLineColor("Looking for plugins", ConsoleColor.Yellow);
            if (Directory.Exists(@"plugins\"))
            {
                int successes = 0;
                int n_plugins = 0;
                IEnumerable<string> plugins = Directory.EnumerateFiles(@"plugins\", "*.dll", SearchOption.TopDirectoryOnly);
                WriteLineColor("Found plugins directory! Initalizing plugins!", ConsoleColor.Green);
                foreach (string i in plugins)
                {
                    n_plugins++;
                }
                WriteLineColor("Found " + n_plugins + " plugin(s)!", ConsoleColor.Green);
                foreach (string path in plugins)
                {
                    try
                    {
                        //load dll
                        Assembly asm = Assembly.LoadFrom(path);
                        //load assembly of dll
                        AppDomain.CurrentDomain.Load(asm.GetName());
                        //get all types in dll
                        foreach (Type t in asm.GetExportedTypes())
                        {
                            if (t.IsSubclassOf(typeof(Command)))
                            {
                                //check for no parameter constructor
                                if (t.GetConstructor(Type.EmptyTypes) == null)
                                {
                                    throw new PluginIntializationException("The 7Sharp API does not allow commands that have constructors with parameters!");
                                }
                                //attempt to get parse method
                                if (t.GetMethod("Parse") == null || t.GetMethod("Parse") == typeof(Command).GetMethod("Parse"))
                                {
                                    throw new PluginIntializationException("Plugin " + Path.GetFileName(path) + ": Command " + t.Name + " did not have a parse method!");
                                }
                                //create instance
                                commands.Add((Command)Activator.CreateInstance(t));
                                //woo hoo it works!
                                successes++;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        WriteLineColor(e.GetType() + ": " + e.Message, ConsoleColor.Red);
                    }
                }
            }
            else
            {
                WriteLineColor("Plugin directory not found!", ConsoleColor.Magenta);
            }
            #if DEBUG
                //do nothing
            #else
                Console.Clear();
            #endif
            //parse script if any
            has_args = args.Length == 1;
            UpdateEnvironment();
			while (true)
            {
                if (!has_args)
                {
                    input = ReadLine();
                    input_split = input.Split(' ', '\n');
                    UpdateEnvironment();
                    ParseCommands();
                }
                else
                {
                    while (srr.EndOfStream == false)
                    {
                        input = srr.ReadLine();
                        input_split = input.Split(' ', '\n');
                        UpdateEnvironment();
                        ParseCommands();
                    }
                    srr.Close();
                    UpdateEnvironment();
                }
            }
		}
        private static void ParseCommands()
        {
            foreach (Command i in commands)
            {
                i.Parse();
            }
        }

        private static void UpdateEnvironment()
        {
            InternalEnv.commands = commands;
            InternalEnv.echo = echo;
            InternalEnv.exit = exit;
            InternalEnv.has_args = has_args;
            InternalEnv.input = input;
            InternalEnv.input_split = input.Split(' ', '\n');
            InternalEnv.ints = ints;
            InternalEnv.loop = loop;
            InternalEnv.parsed = parsed;
            InternalEnv.srr = srr;
            InternalEnv.strings = strings;
            InternalEnv.times = times;
            InternalEnv._inputs = _inputs;
            _7sEnvironment.Commands = commands;
            _7sEnvironment.Echo = echo;
            _7sEnvironment.Input = input;
            _7sEnvironment.Ints = ints;
            _7sEnvironment.SplitInput = input.Split(' ', '\n');
            _7sEnvironment.Strings = strings;
            _7sEnvironment.CurrentScriptStream = srr;
        }
    }
}