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
			this.Name = name;
			this.argNames = argNames;
			code = nodes;
		}

		public void Run(ref InterpreterState state, params object[] args)
		{
			if (args.Length != argNames.Length)
			{
				throw new InterpreterException($"Function {Name} expects {argNames.Length} args, but got {args.Length}");
			}
			state.OnlyAllowGlobals = true;
			state.PushScope();
			RootNode node = new RootNode();
			code.ForEach(n => node.Add(n));
			node.Run(ref state);
			state.OnlyAllowGlobals = false;
		}
	}
}
