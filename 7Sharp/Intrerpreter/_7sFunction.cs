using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Sharp.Intrerpreter
{
	internal class _7sFunction
	{
		public readonly string Name;
		public readonly bool InfiniteArgs;
		public readonly Dictionary<int, Delegate> Funcs;

		public _7sFunction(string name, Dictionary<int, Delegate> funcs, bool infArgs = false)
		{
			Name = name ?? throw new ArgumentNullException();
			Funcs = funcs ?? throw new ArgumentNullException();
			InfiniteArgs = infArgs;
			if (infArgs && funcs.Count != 1)
			{
				throw new InvalidOperationException("Error: _7sFunction with infinite args can only have one overload!");
			}
		}

		public object Run(params object[] args)
		{
			if (!Funcs.Keys.Contains(args == null ? 0 : args.Length) && !InfiniteArgs)
			{
				throw new InterpreterException($"Function {Name} only accepts the following number of arguments: {string.Join(", ", Funcs.Count)}");
			}
			try
			{
				if (InfiniteArgs)
				{
					return Funcs.First().Value.DynamicInvoke(args.ToArray());
				}
				return Funcs[args.Count()].DynamicInvoke(args);
			}
			catch (Exception e)
			{
				throw new InterpreterException($"Error in {Name}", e);
			}
		}

		public override string ToString() => $"SysFunc [{string.Join(", ", Funcs.Keys.Select(count => $"{Name}({string.Join(", ", Enumerable.Range(1, count + 1).Select(x => "p" + x))})"))}]";
	}
}