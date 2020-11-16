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
		public readonly Dictionary<string, object> Variables = new Dictionary<string, object>();
		public readonly List<_7sFunction> Functions = new List<_7sFunction>();
		public readonly ExpressionEvaluator evaluator = new ExpressionEvaluator();
		public readonly Stack<int> LoopIndexes = new Stack<int>();
		public bool LastIfResult = false;

		public T TryParse<T>(List<Token<TokenType>> tokens, string message) => TryParse<T>(tokens.AsString(), message);

		public T TryParse<T>(string value, string message)
		{
			object o = evaluator.Evaluate(value);
			if (!(o is T))
			{
				throw new InterpreterException(message);
			}
			return (T)o;
		}

		public object ReturnValue;
		public LexerPosition Location;

		public static void Init(ref InterpreterState state)
		{
			state.evaluator.PreEvaluateFunction += state.Evaluator_PreEvaluateFunction;
			state.evaluator.PreEvaluateVariable += state.Evaluator_PreEvaluateVariable;
			state.evaluator.Assemblies.Clear();
			state.evaluator.OptionInlineNamespacesEvaluationActive = false;
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

		private void Evaluator_PreEvaluateVariable(object sender, VariablePreEvaluationEventArg e)
		{
			if (Variables.ContainsKey(e.Name))
			{
				e.Value = Variables[e.Name];
				e.CancelEvaluation = false;
			}
		}

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
			if (!Functions.Any(f => f.Name == e.Name))
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
