using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter
{
	internal class InterpreterException : Exception
	{
		public InterpreterException(string text, Exception e) : base($"{text}:\n{e.Message}\n{e.StackTrace}")
		{
		}

		public InterpreterException(string text) : base($"Error: {text}")
		{
		}

		public InterpreterException() : base("Unknown Interpreter Error!")
		{
		}
	}
}
