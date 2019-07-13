using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.API
{
    internal class InternalEnv
    {
        public static bool _inputs = false;
        public static string[] input_split = { };
        public static bool exit = false;
        public static bool loop = false;
        public static bool parsed = false;
        public static string input = "";
        public static bool has_args = false;
        public static List<_7sInt> ints = new List<_7sInt>();
        public static List<_7sString> strings = new List<_7sString>();
        public static StreamReader srr = null;
        public static int times = 1;
        public static List<Command> commands = new List<Command>();
        public static bool echo = true;
        public static bool RunningEnvCode = false;
        public static string[] EnvCode = null;
        public static int EnvCodeIndex = 0;
    }
}
