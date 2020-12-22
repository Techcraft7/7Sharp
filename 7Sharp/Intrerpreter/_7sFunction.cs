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
			// Check arg types
			Delegate del = InfiniteArgs ? Funcs.First().Value : Funcs[args.Count()];
			Type[] types = del.GetType().GetMethod("Invoke").GetParameters().Select(pi => pi.ParameterType).ToArray();
			for (int i = 0; i < args.Count(); i++)
			{
				if (types[i].IsByRef)
				{
					if (!args[i].GetType().IsSubclassOf(types[i].GetElementType()) && !args[i].GetType().IsEquivalentTo(types[i].GetElementType()))
					{
						throw new InterpreterException($"Argument {i + 1} of {Name} should be {types[i].GetElementType().GetSimpleName()}");
					}
				}
				else if (!args[i].GetType().IsSubclassOf(types[i]) && !args[i].GetType().IsEquivalentTo(types[i]))
				{
					throw new InterpreterException($"Argument {i + 1} of {Name} should be {types[i].GetSimpleName()}");
				}
			}
			// Run
			return del.DynamicInvoke(InfiniteArgs ? args.ToArray() : args);
		}

		public override string ToString() => $"SysFunc [{string.Join(", ", Funcs.Keys.Select(count => $"{Name}({string.Join(", ", Enumerable.Range(1, count + 1).Select(x => "p" + x))})"))}]";
	}
}