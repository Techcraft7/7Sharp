using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace _7Sharp.Intrerpreter
{
	internal class Scope
	{
		Stack<IDictionary<string, object>> vars = new Stack<IDictionary<string, object>>();
		public IDictionary<string, object> Peek() => vars.Peek();
		public void PushScope(IDictionary<string, object> pairs)
		{
			var prev = vars.Count > 0 ? vars.Peek() : new Dictionary<string, object>();
			string[] keys = prev.Keys.Concat(pairs.Keys).ToArray();
			foreach (string key in keys)
			{
				if (!prev.ContainsKey(key) && pairs.ContainsKey(key))
				{
					prev.Add(pairs.ToList().Find(x => x.Key.Equals(key)));
				}
			}
			vars.Push(prev);
		}

		public IDictionary<string, object> PopScope()
		{
			var ret = vars.Pop();
			var prev = vars.Pop();
			string[] keys = prev.Keys.ToArray();
			foreach (string key in keys)
			{
				if (!ret.ContainsKey(key))
				{
					continue;
				}
				prev[key] = ret[key];
			}
			vars.Push(prev);
			return prev;
		}
	}
}