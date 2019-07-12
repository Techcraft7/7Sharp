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
        public static List<VarString> Strings;
        public static List<VarInt> Ints;
        public static List<Command> Commands;
    }
}
