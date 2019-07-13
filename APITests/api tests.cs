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

    public class RunCodeCommand : Command
    {
        public RunCodeCommand()
        {
            call = "runtest";
            help = "runs \"loop 5\", \"write hi\", and \"end\" that prints \"hi\" 5 times";
        }

        public override void Parse()
        {
            if (_7sEnvironment.SplitInput[0] == call)
            {
                _7sEnvironment.RunCode("loop 5\nwrite hi\nend");
            }
        }
    }
}
