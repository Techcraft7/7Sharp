using _7Sharp.Interpreter.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter
{
	internal class UserFunction
	{
		public readonly string Name;
		private readonly string[] argNames;
		private readonly List<Node> code;

		public UserFunction(string name, string[] argNames, List<Node> nodes)
		{
			Name = name;
			this.argNames = argNames;
			if (argNames.Length == 1)
			{
				if (string.IsNullOrWhiteSpace(argNames[0]))
				{
					this.argNames = new string[0];
				}
			}
			code = nodes;
		}

		public void Run(ref InterpreterState state, params object[] args)
		{
			if (args.Length != argNames.Length)
			{
				throw new InterpreterException($"Function {Name} expects {argNames.Length} args, but got {args.Length}");
			}
			state.InsideFunction = true;
			state.PushScope();
			state.FuncParams = Enumerable.Range(0, argNames.Length)
				.Select(i => Tuple.Create(argNames[i], args[i]))
				.ToDictionary(t => t.Item1, t => t.Item2);
			foreach (Node n in code)
			{
				state.Location = n.linePosition;
				n.Run(ref state);
				if (state.ExitFunc) // If return is used
				{
					break;
				}
			}
			state.PopScope();
			state.InsideFunction = false;
			state.FuncParams = null;
		}
	}
}
