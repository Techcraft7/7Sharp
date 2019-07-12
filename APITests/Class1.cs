using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7Sharp.API;

namespace APITests
{
    using static Console;
    public class HelloCommand : Command
    {
        public HelloCommand()
        {
            call = "hello";
            help = "says hello to the user!";
        }

        public override void Parse()
        {
            if (_7sEnvironment.SplitInput[0] == call)
            {
                WriteLine("Hello!");
            }
        }
    }
}
