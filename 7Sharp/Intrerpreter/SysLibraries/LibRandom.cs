using _7Sharp.Manual;
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
			state.Functions.Add(new _7sFunction("setSeed", new Dictionary<int, Delegate>()
			{
				{ 0, new Action<int>(SetSeed) }
			}));
		}

		[ManualDocs("setSeed", "{\"title\":\"setSeed(seed)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"setSeed(<seed>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Set the RNG seed to \"},{\"text\":\"seed\",\"color\":\"Green\"}]}]}")]
		private void SetSeed(int seed) => RNG = new Random(seed);

		[ManualDocs("next", "{\"title\":\"next()\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"next();\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Get a random 32-bit integer. (-2.147 billion to 2.147 billion)\"}]}]}")]
		private static int Next() => RNG.Next();

		[ManualDocs("nextInt", "{\"title\":\"nextInt(min, max)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"nextInt(<min>, <max>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Get a random integer between \"},{\"text\":\"min\",\"color\":\"Green\"},{\"text\":\" and \"},{\"text\":\"max\",\"color\":\"Green\"}]}]}")]
		private static int NextInt(int min, int max)
		{
			if (max < min)
			{
				throw new InterpreterException("nextInt: max was less than min!");
			}
			return RNG.Next(min, max + 1);
		}

		[ManualDocs("nextDouble", "{\"title\":\"nextDouble(min, max)\",\"sections\":[{\"header\":\"Syntax\",\"text\":[{\"text\":\"nextDouble(<min>, <max>);\"}]},{\"header\":\"Behavior\",\"text\":[{\"text\":\"Get a random doubleeger between \"},{\"text\":\"min\",\"color\":\"Green\"},{\"text\":\" and \"},{\"text\":\"max\",\"color\":\"Green\"}]}]}")]
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
