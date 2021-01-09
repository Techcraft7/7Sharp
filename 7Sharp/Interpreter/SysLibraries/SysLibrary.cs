using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter.SysLibraries
{
	internal abstract class SysLibrary
	{
		public abstract string GetName();
		public abstract void Import(ref InterpreterState state);

		public static SysLibrary[] GetAllLibraries() => new SysLibrary[]
		{
			new LibRandom(),
			new LibIO()
		};
	}
}
