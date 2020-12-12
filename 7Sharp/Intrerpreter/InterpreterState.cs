using _7Sharp.Manual;
using CodingSeb.ExpressionEvaluator;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter
{
	internal class InterpreterState
	{
		public readonly Stack<Dictionary<string, object>> Variables = new Stack<Dictionary<string, object>>();
		public readonly List<_7sFunction> Functions = new List<_7sFunction>();
		public readonly List<UserFunction> UserFuncs = new List<UserFunction>();
		public readonly ExpressionEvaluator evaluator = new ExpressionEvaluator();
		public readonly Stack<int> LoopIndexes = new Stack<int>();
		public bool LastIfResult = false;
		public bool OnlyAllowGlobals = false;

		// Utility to run code with a dictionary of variables in the current scope
		public void RunWithVariables(RunWithVarsDelegate func)
		{
			Dictionary<string, object> vars = Variables.Pop();
			func.Invoke(ref vars);
			Variables.Push(vars);
		}

		public delegate void RunWithVarsDelegate(ref Dictionary<string, object> vars);

		public void Reset()
		{
			Variables.Clear();
			Variables.Push(new Dictionary<string, object>());
			OnlyAllowGlobals = false;
			LastIfResult = false;
		}

		public T TryParse<T>(List<Token<TokenType>> tokens, string message) => TryParse<T>(tokens.AsString(), message);

		public T TryParse<T>(string value, string message)
		{
			try
			{
				object o = evaluator.Evaluate(value);
				if (!(o is T))
				{
					throw new InterpreterException(message);
				}
				return (T)o;
			}
			catch
			{
				throw new InterpreterException(message);
			}
		}

		public object ReturnValue;
		public LexerPosition Location;

		public static void Init(ref InterpreterState state)
		{
			state.Variables.Push(new Dictionary<string, object>());
			state.evaluator.PreEvaluateFunction += state.Evaluator_PreEvaluateFunction;
			state.evaluator.PreEvaluateVariable += state.Evaluator_PreEvaluateVariable;
			state.evaluator.OptionInlineNamespacesEvaluationActive = false;
			state.evaluator.Assemblies.Clear();
			state.evaluator.Variables.Clear();
			state.evaluator.Variables.Add("PI", Math.PI);
			state.evaluator.Variables.Add("E", Math.E);
			state.Functions.Add(new _7sFunction("write", new Dictionary<int, Delegate>()
			{
				{ 1, new Action<object>(SysFunctions.Write)}
			}));
			state.Functions.Add(new _7sFunction("writeraw", new Dictionary<int, Delegate>()
			{
				{ 1, new Action<object>(SysFunctions.WriteRaw)}
			}));
			state.Functions.Add(new _7sFunction("getLoopIndex", new Dictionary<int, Delegate>()
			{
				{ 0, new Func<int>(state.GetFirstLoopIndex) },
				{ 1, new Func<int, int>(state.GetLoopIndex) }
			}));
			state.Functions.Add(new _7sFunction("sin", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<double, double>(SysFunctions.Sin)}
			}));
			state.Functions.Add(new _7sFunction("cos", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<double, double>(SysFunctions.Cos)}
			}));
			state.Functions.Add(new _7sFunction("tan", new Dictionary<int, Delegate>()
			{
				{ 1, new Func<double, double>(SysFunctions.Tan)}
			}));
		}

		public void PushScope() => Variables.Push(Variables.Peek().Clone());

		public void PopScope()
		{
			if (Variables.Count < 2)
			{
				throw new InterpreterException("Attempted to pop scope in an invalid context!");
			}
			Dictionary<string, object> oldVars = Variables.Pop(); // Current scope
			Dictionary<string, object> newVars = Variables.Pop(); // Previous scope

			Dictionary<string, object> temp = newVars.Clone();

			// If variable in previous scope is changed in the current scope, update it
			foreach (KeyValuePair<string, object> kv in newVars)
			{
				if (oldVars.ContainsKey(kv.Key))
				{
					temp[kv.Key] = oldVars[kv.Key];
				}
			}
			Variables.Push(temp);
		}

		private void Evaluator_PreEvaluateVariable(object sender, VariablePreEvaluationEventArg e)
		{
			if (OnlyAllowGlobals)
			{
				Dictionary<string, object> globals = Variables.Last();
				if (globals.ContainsKey(e.Name))
				{
					e.Value = globals[e.Name];
					e.CancelEvaluation = false;
				}
				return;
			}
			Dictionary<string, object> top = Variables.Pop();
			if (top.ContainsKey(e.Name))
			{
				e.Value = top[e.Name];
				e.CancelEvaluation = false;
			}
			else
			{
				throw new InterpreterException($"Variable \"{e.Name}\" not defined in the current scope at {Location}");
			}
			Variables.Push(top);
		}

		[ManualDocs("getLoopIndex", "{\"title\":\"getLoopIndex() | getLoopIndex(n)\",\"sections\":[{\"header\":\"getLoopIndex()\",\"text\":[{\"text\":\"Returns the index of the current \"},{\"text\":\"loop (times)\",\"color\":\"Green\"},{\"text\":\" loop. Equivalent of \"},{\"text\":\"getLoopIndex(0)\",\"color\":\"Cyan\"}]},{\"header\":\"getLoopIndex(n)\",\"text\":[{\"text\":\"Returns the index of the  \"},{\"text\":\"n\",\"color\":\"Green\"},{\"text\":\"th \"},{\"text\":\"loop (times)\",\"color\":\"Green\"},{\"text\":\" loop. \"},{\"text\":\"getLoopIndex(1)\",\"color\":\"Cyan\"},{\"text\":\" will get the index of the loop outside of the current \"},{\"text\":\"loop (times)\",\"color\":\"Green\"},{\"text\":\" loop, \"},{\"text\":\"getLoopIndex(2)\",\"color\":\"Cyan\"},{\"text\":\" will get the index of the loop outside of the loop outside of current \"},{\"text\":\"loop (times)\",\"color\":\"Green\"},{\"text\":\" loop, and so on.\"}]}]}")]
		private int GetFirstLoopIndex() => GetLoopIndex(0);

		public int GetLoopIndex(int i)
		{
			if (LoopIndexes.Count == 0)
			{
				throw new InterpreterException($"Not in a loop at {Location}");
			}
			try
			{
				return LoopIndexes.ToArray()[i];
			}
			catch (IndexOutOfRangeException)
			{
				throw new InterpreterException($"Could not get loop index {i} at { Location}");
			}
		}

		private void Evaluator_PreEvaluateFunction(object sender, FunctionPreEvaluationEventArg e)
		{
			if (!Functions.Any(f => f.Name == e.Name) && !UserFuncs.Any(f => f.Name == e.Name))
			{
				throw new InterpreterException($"Unkown function {e.Name} at {Location}");
			}
			_7sFunction func = Functions.First(f => f.Name == e.Name);
			object[] args = new object[0];
			if (e.Args.Count > 0)
			{
				args = e.Args.Select(a => evaluator.Evaluate(a)).ToArray();
			}
			e.Value = func.Run(args);
			e.FunctionReturnedValue = true;
		}
	}
}
