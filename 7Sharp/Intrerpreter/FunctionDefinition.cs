using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter
{
	internal class FunctionDefinition
	{
		string Code;
		string[] ArgNames;
		Interpreter Interpreter;

		public FunctionDefinition(string code, Interpreter i, string[] argNames)
		{
			Code = code;
			Interpreter = i;
			ArgNames = argNames;
		}

		public dynamic Run(params dynamic[] args)
		{
			List<KeyValuePair<string, dynamic>> kvs = new List<KeyValuePair<string, dynamic>>();
			int i = 0;
			foreach (string n in ArgNames)
			{
				kvs.Add(new KeyValuePair<string, dynamic>(n, args[i]));
				i++;
			}
			if (ArgNames.Length == kvs.Count)
			{
				Interpreter.InternalRun(Code, out dynamic ret, false, kvs.ToArray());
				return ret;
			}
			return null;
		}

		public int NumberOfArgs() => ArgNames.Length;
	}
}
