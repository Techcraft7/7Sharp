using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.SysLibraries
{
	internal class LibRandom : SysLibrary
	{
		internal static Random RNG;

		public override void Import(ref InterpreterState state)
		{
			RNG = new Random();
			state.Functions.Add(new _7sFunction("nextInt", new Dictionary<int, Delegate>()
			{
				{ 2, new Func<int, int, int>(NextInt) }
			}));
			state.Functions.Add(new _7sFunction("nextDouble", new Dictionary<int, Delegate>()
			{
				{ 2, new Func<double, double, double>(NextDouble) }
			}));
			state.Functions.Add(new _7sFunction("next", new Dictionary<int, Delegate>()
			{
				{ 0, new Func<int>(Next) }
			}));
		}

		private static int Next() => RNG.Next();

		private static int NextInt(int min, int max)
		{
			if (max < min)
			{
				throw new InterpreterException("nextInt: max was less than min!");
			}
			return RNG.Next(min, max + 1);
		}

		private static double NextDouble(double min, double max)
		{
			if (max < min)
			{
				throw new InterpreterException("nextDouble: max was less than min!");
			}
			return (RNG.NextDouble() * (max - min)) + min;
		}

		public override string GetName() => "random.sys";
	}
}
