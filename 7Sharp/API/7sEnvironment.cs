using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.API
{
    public static class _7sEnvironment
    {
        public static string[] SplitInput = { };
        public static string Input = "";
        public static bool Echo = true;
        public static StreamReader CurrentScriptStream = null;
        public static List<_7sString> Strings;
        public static List<_7sInt> Ints;
        public static List<Command> Commands;
        public static bool ForceRunningCode = false;
        public static string[] CodeBeingForceRun = null;
        public static int ForceRunningCodeIndex = 0;

        public static void RunCode(string code)
        {
            if (Program.RunningEnvCode)
            {
                Techcraft7_DLL_Pack.ColorConsoleMethods.WriteLineColor("Already force running code!", ConsoleColor.Red);
                return;
            }
            string[] list = code.Split('\n');
            Program.RunningEnvCode = true;
            Program.EnvCode = list;
            Program.UpdateEnvironment();
        }
    }
}
