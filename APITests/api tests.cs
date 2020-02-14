using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7Sharp.API;
using _7Sharp;

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
            help = "runs \"loop 5\", \"write hi\", and \"end\" that prints \"foo\" 5 times";
        }

        public override void Parse()
        {
            if (_7sEnvironment.SplitInput[0] == call)
            {
                _7sEnvironment.RunCode("loop 5\nwrite foo\nend");
            }
        }
    }

    public class CoolCommand : Command
    {
        public CoolCommand()
        {
            call = "coolcommand";
            help = "does some cool stoof idk";
        }

        public override void Parse()
        {
            if (_7sEnvironment.SplitInput[0] == call)
            {
                WriteLine("COOL STOOF!");
            }
        }
    }

    public class Foo : Command
    {
        public Foo()
        {
            call = "SPAM";
            help = "bar";
        }

        public override void Parse()
        {
            if (_7sEnvironment.SplitInput[0] == call)
            {
                for (int i = 0; i < 100000; i++)
                {
                    Write((char)i);
                }
            }
        }
    }
}
