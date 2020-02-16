using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter
{
    using static _7Sharp.Program;
    using static Console;
    internal static class SystemFunctions
    {
        internal static void Init()
        {
            interpreter.functions.Add("write", new _7sFunction("write", 1, new Action<dynamic>(Write)));
        }

        static void Write(dynamic s)
        {
            WriteLine(s);
        }
    }
}
