using _7Sharp.Intrerpreter.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter
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
			state.Variables.Push(Enumerable.Range(0, argNames.Length)
				.Select(i => Tuple.Create(argNames[i], args[i]))
				.ToDictionary(t => t.Item1, t => t.Item2));
			foreach (Node n in code)
			{
				n.Run(ref state);
			}
			state.PushScope();
			state.InsideFunction = false;
		}
	}
}
