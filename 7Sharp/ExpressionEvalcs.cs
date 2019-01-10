using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Security.Principal;
using System.Threading.Tasks;

namespace _7Sharp.ExpressionEvaluation
{
	public class AdditionalFunctionEventArgs : EventArgs
	{
		private string _name = "";
		private object[] _parameters;
		private object _return;

		public AdditionalFunctionEventArgs(string name, object[] parameters)
		{
			this._name = name;
			this._parameters = parameters;
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public object ReturnValue
		{
			get
			{
				return this._return;
			}
			set
			{
				this._return = value;
			}
		}

		public object[] GetParameters()
		{
			if (this._parameters == null)
				return (object[])null;
			object[] objArray = new object[this._parameters.Length];
			Array.Copy((Array)this._parameters, (Array)objArray, objArray.Length);
			return objArray;
		}
	}
	public interface IExpression
	{
		string Expression { get; set; }

		object Evaluate();
	}
	public class FunctionEval : IExpression
	{
		private string _expression = "";
		private string _function = "";
		internal Hashtable _variables = new Hashtable();
		private bool _bParsed;
		private object[] _params;

		public string Expression
		{
			get
			{
				return this._expression;
			}
			set
			{
				this._expression = value;
				this._function = "";
				this._bParsed = false;
				this._params = (object[])null;
			}
		}

		public FunctionEval()
		{
		}

		public FunctionEval(string expression)
		{
			this.Expression = expression;
		}

		public object Evaluate()
		{
			if (!this._bParsed)
			{
				string input = new StringBuilder(this.Expression).ToString();
				Match m = DefinedRegex.Function.Match(input);
				if (m.Success)
				{
					this._params = this.GetParameters(m);
					this._function = m.Groups["Function"].Value;
				}
				this._bParsed = true;
			}
			return this.ExecuteFunction(this._function, this._params);
		}

		public static object Evaluate(string expression)
		{
			return new FunctionEval(expression).Evaluate();
		}

		public static object Evaluate(string expression, _7Sharp.AdditionalFunctionEventHandler handler)
		{
			FunctionEval functionEval = new FunctionEval(expression);
			if (handler != null)
				functionEval.AdditionalFunctionEventHandler += handler;
			return functionEval.Evaluate();
		}

		public static string Replace(string input)
		{
			return new FunctionEval(input).Replace();
		}

		public static string Replace(string input, _7Sharp.AdditionalFunctionEventHandler handler)
		{
			FunctionEval functionEval = new FunctionEval(input);
			if (handler != null)
				functionEval.AdditionalFunctionEventHandler += handler;
			return functionEval.Replace();
		}

		public string ReplaceEx(string input)
		{
			if ((input ?? "") == "")
				return "";
			this.Expression = input;
			return this.Replace();
		}

		public string Replace()
		{
			StringBuilder stringBuilder = new StringBuilder(this.Expression);
			for (Match match = DefinedRegex.Function.Match(this.Expression); match.Success; match = DefinedRegex.Function.Match(stringBuilder.ToString()))
			{
				int num = 1;
				int index = match.Index + match.Length;
				while (num > 0)
				{
					if (index >= stringBuilder.Length)
						throw new ArgumentException("Missing ')' in Expression");
					if (stringBuilder[index] == ')')
						--num;
					if (stringBuilder[index] == '(')
						++num;
					++index;
				}
				string str = stringBuilder.ToString(match.Index, index - match.Index);
				FunctionEval functionEval = new FunctionEval(str);
				// ISSUE: reference to a compiler-generated field
				functionEval.AdditionalFunctionEventHandler += this.AdditionalFunctionEventHandler;
				functionEval._variables = this._variables;
				stringBuilder.Replace(str, string.Concat(functionEval.Evaluate()));
			}
			for (Match match = DefinedRegex.Variable.Match(stringBuilder.ToString()); match.Success; match = DefinedRegex.Variable.Match(stringBuilder.ToString()))
				stringBuilder.Replace(match.Value, string.Concat(this._variables[(object)match.Groups["Variable"].Value]));
			return stringBuilder.ToString();
		}

		public void SetVariable(string key, object value)
		{
			this.ClearVariable(key);
			this._variables.Add((object)key, value);
		}

		public void ClearVariable(string key)
		{
			if (!this._variables.ContainsKey((object)key))
				return;
			this._variables.Remove((object)key);
		}

		public override string ToString()
		{
			return this.Expression;
		}

		private object[] GetParameters(Match m)
		{
			int index1 = m.Index + m.Length;
			int num = 1;
			int startIndex = 0;
			bool flag1 = false;
			ArrayList arrayList = new ArrayList();
			while (num > 0)
			{
				if (index1 >= this.Expression.Length)
					throw new ArgumentException("Missing ')' in Expression");
				if (!flag1 && this.Expression[index1] == ')')
					--num;
				if (!flag1 && this.Expression[index1] == '(')
					++num;
				if (this.Expression[index1] == '"' && (index1 == 0 || this.Expression[index1 - 1] != '\\'))
					flag1 = !flag1;
				if (num > 0)
					++index1;
			}
			string str = this.Expression.Substring(m.Index + m.Length, index1 - (m.Index + m.Length));
			if ((str ?? "") == "")
				return (object[])null;
			bool flag2 = false;
			int index2;
			for (index2 = 0; index2 < str.Length; ++index2)
			{
				if (!flag2 && str[index2] == ')')
					--num;
				if (!flag2 && str[index2] == '(')
					++num;
				if (str[index2] == '"' && (index2 == 0 || str[index2 - 1] != '\\'))
					flag2 = !flag2;
				if (!flag2 && num == 0 && str[index2] == ',')
				{
					arrayList.Add((object)str.Substring(startIndex, index2 - startIndex));
					startIndex = index2 + 1;
				}
			}
			arrayList.Add((object)str.Substring(startIndex, index2 - startIndex));
			for (int index3 = 0; index3 < arrayList.Count; ++index3)
			{
				ExpressionEval expressionEval = new ExpressionEval(arrayList[index3].ToString());
				// ISSUE: reference to a compiler-generated field
				expressionEval.AdditionalFunctionEventHandler += this.AdditionalFunctionEventHandler;
				expressionEval._variables = this._variables;
				arrayList[index3] = (object)expressionEval;
			}
			return arrayList.ToArray();
		}

		private object ExecuteFunction(string name, object[] p)
		{
			object[] parameters = (object[])null;
			if (p != null)
			{
				parameters = (object[])p.Clone();
				for (int index = 0; index < parameters.Length; ++index)
				{
					if (parameters[index] is IExpression)
						parameters[index] = ((IExpression)parameters[index]).Evaluate();
				}
			}
			switch (name.ToLower(CultureInfo.CurrentCulture))
			{
				case "abs":
					return (object)Math.Abs(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "acos":
					return (object)Math.Acos(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "adddays":
					return (object)Convert.ToDateTime(parameters[0]).AddDays((double)Convert.ToInt32(parameters[1]));
				case "addhours":
					return (object)Convert.ToDateTime(parameters[0]).AddHours((double)Convert.ToInt32(parameters[1]));
				case "addminutes":
					return (object)Convert.ToDateTime(parameters[0]).AddMinutes((double)Convert.ToInt32(parameters[1]));
				case "addmonths":
					return (object)Convert.ToDateTime(parameters[0]).AddMonths(Convert.ToInt32(parameters[1]));
				case "addseconds":
					return (object)Convert.ToDateTime(parameters[0]).AddSeconds((double)Convert.ToInt32(parameters[1]));
				case "addyears":
					return (object)Convert.ToDateTime(parameters[0]).AddYears(Convert.ToInt32(parameters[1]));
				case "asin":
					return (object)Math.Asin(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "atan":
					return (object)Math.Atan(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "case":
					return FunctionEval.Case(parameters);
				case "cdatetime":
					return (object)Convert.ToDateTime(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture);
				case "cdbl":
					return (object)Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture);
				case "ciel":
					return (object)Math.Ceiling(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "cint":
					return (object)Convert.ToInt32(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture);
				case "clong":
					return (object)Convert.ToInt64(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture);
				case "cos":
					return (object)Math.Cos(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "cosh":
					return (object)Math.Cosh(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "cot":
					return (object)(1.0 / Math.Tan(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture)));
				case "csc":
					return (object)(1.0 / Math.Sin(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture)));
				case "cuint":
					return (object)Convert.ToUInt32(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture);
				case "culong":
					return (object)Convert.ToUInt64(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture);
				case "currentuserid":
					return (object)WindowsIdentity.GetCurrent().Name.ToLower();
				case "e":
					return (object)Math.E;
				case "exp":
					return (object)Math.Exp(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "expr":
					ExpressionEval expressionEval = new ExpressionEval(string.Concat(parameters[0]));
					// ISSUE: reference to a compiler-generated field
					expressionEval.AdditionalFunctionEventHandler += this.AdditionalFunctionEventHandler;
					expressionEval._variables = this._variables;
					return expressionEval.Evaluate();
				case "floor":
					return (object)Math.Floor(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "fmtdate":
					return (object)Convert.ToDateTime(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture).ToString(string.Concat(parameters[1]), (IFormatProvider)CultureInfo.CurrentCulture);
				case "fmtnum":
					return (object)Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture).ToString(string.Concat(parameters[1]), (IFormatProvider)CultureInfo.CurrentCulture);
				case "iif":
					return FunctionEval.Iif(parameters);
				case "log":
					return (object)(parameters.Length > 1 ? Math.Log(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture), Convert.ToDouble(parameters[1], (IFormatProvider)CultureInfo.CurrentCulture)) : Math.Log(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture)));
				case "log10":
					return (object)Math.Log10(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "max":
					return (object)Math.Max(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture), Convert.ToDouble(parameters[1], (IFormatProvider)CultureInfo.CurrentCulture));
				case "maxdate":
					return (object)DateTime.MaxValue;
				case "min":
					return (object)Math.Min(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture), Convert.ToDouble(parameters[1], (IFormatProvider)CultureInfo.CurrentCulture));
				case "mindate":
					return (object)DateTime.MinValue;
				case "monthname":
					return (object)new DateTime(2000, Convert.ToInt32(parameters[0]), 1).ToString("MMMM");
				case "now":
					return (object)DateTime.Now;
				case "pi":
					return (object)Math.PI;
				case "pow":
					return (object)Math.Pow(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture), Convert.ToDouble(parameters[1], (IFormatProvider)CultureInfo.CurrentCulture));
				case "round":
					return (object)(parameters.Length > 1 ? Math.Round(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture), Convert.ToInt32(parameters[1], (IFormatProvider)CultureInfo.CurrentCulture)) : Math.Round(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture)));
				case "sec":
					return (object)(1.0 / Math.Cos(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture)));
				case "sin":
					return (object)Math.Sin(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "sinh":
					return (object)Math.Sinh(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "sqrt":
					return (object)Math.Sqrt(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "str":
					return (object)parameters[0].ToString();
				case "tan":
					return (object)Math.Tan(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "tanh":
					return (object)Math.Tanh(Convert.ToDouble(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture));
				case "today":
					return (object)DateTime.Today;
				default:
					return this.AdditionalFunctionHelper(name, parameters);
			}
		}

		protected object AdditionalFunctionHelper(string name, object[] parameters)
		{
			AdditionalFunctionEventArgs e = new AdditionalFunctionEventArgs(name, parameters);
			// ISSUE: reference to a compiler-generated field
			if (this.AdditionalFunctionEventHandler != null)
			{
				// ISSUE: reference to a compiler-generated field
				this.AdditionalFunctionEventHandler((object)this, e);
			}
			return e.ReturnValue;
		}

		public static object Iif(params object[] parameters)
		{
			if (parameters.Length < 3)
				return (object)"Invalid Number of Parameters: iif(condition, val if true, val if false)";
			if (Convert.ToBoolean(parameters[0], (IFormatProvider)CultureInfo.CurrentCulture))
				return parameters[1];
			return parameters[2];
		}

		public static object Case(params object[] parameters)
		{
			if (parameters.Length % 2 != 0 && (uint)parameters.Length > 0U)
				return (object)"Invalid Number of Parameters: case(condition, val, condition2, val2, ...)";
			int index = 0;
			while (index < parameters.Length)
			{
				if (string.Concat(parameters[index]) == "else" || Convert.ToBoolean(parameters[index], (IFormatProvider)CultureInfo.CurrentCulture))
					return parameters[index + 1];
				index += 2;
			}
			return (object)null;
		}

		public event _7Sharp.AdditionalFunctionEventHandler AdditionalFunctionEventHandler;
	}
	internal class DefinedRegex
	{
		internal static Regex Numeric = new Regex("(?:[0-9]+)?(?:\\.[0-9]+)?(?:E-?[0-9]+)?(?=\\b)", RegexOptions.Compiled);
		internal static Regex Hexadecimal = new Regex("0x([0-9a-fA-F]+)", RegexOptions.Compiled);
		internal static Regex Boolean = new Regex("true|false", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		internal static Regex UnaryOp = new Regex("(?<=(?:<<|>>|\\+|-|\\*|/|%|&&|\\|\\||&|\\||\\^|==|!=|>=|<=|=|<|>)\\s*|\\A)(?:(?:\\+|-|!|~)(?=\\w|\\())", RegexOptions.Compiled);
		internal static Regex BinaryOp = new Regex("(?<!(?:<<|>>|\\+|-|\\*|/|%|&&|\\|\\||&|\\||\\^|==|!=|>=|<=|=|<|>)\\s*|^\\A)(?:<<|>>|\\+|-|\\*|/|%|&&|\\|\\||&|\\||\\^|==|!=|>=|<=|=|<|>)", RegexOptions.Compiled);
		internal static Regex Parenthesis = new Regex("\\(", RegexOptions.Compiled);
		internal static Regex Function = new Regex("\\$(?<Function>\\w+)\\(", RegexOptions.Compiled);
		internal static Regex Variable = new Regex("\\@\\((?<Variable>[\\@\\w\\s]+)\\)", RegexOptions.Compiled);
		internal static Regex DateTime = new Regex("@dt\\((?<DateString>\\d{1,2}[-/]\\d{1,2}[-/](?:\\d{4}|\\d{2})(?:\\s+\\d{1,2}\\:\\d{2}(?:\\:\\d{2})?\\s*(?:AM|PM)?)?)\\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		internal static Regex TimeSpan = new Regex("@ts\\((?:(?<Days>\\d+)\\.|)(?<Hours>\\d{1,2})\\:(?<Minutes>\\d{1,2})(?:\\:(?<Seconds>\\d{1,3})(?:\\.(?<Milliseconds>\\d{1,3})|)|)\\)", RegexOptions.Compiled);
		internal static Regex String = new Regex("\\\"\\\"|\\\"(?<String>.*?[^\\\\])\\\"", RegexOptions.Compiled);
		internal static Regex WhiteSpace = new Regex("\\s+", RegexOptions.Compiled);
		private const string c_strNumeric = "(?:[0-9]+)?(?:\\.[0-9]+)?(?:E-?[0-9]+)?(?=\\b)";
		private const string c_strHex = "0x([0-9a-fA-F]+)";
		private const string c_strBool = "true|false";
		private const string c_strFunction = "\\$(?<Function>\\w+)\\(";
		private const string c_strVariable = "\\@\\((?<Variable>[\\@\\w\\s]+)\\)";
		private const string c_strString = "\\\"\\\"|\\\"(?<String>.*?[^\\\\])\\\"";
		private const string c_strUnaryOp = "(?:\\+|-|!|~)(?=\\w|\\()";
		private const string c_strBinaryOp = "<<|>>|\\+|-|\\*|/|%|&&|\\|\\||&|\\||\\^|==|!=|>=|<=|=|<|>";
		private const string c_strDate = "\\d{1,2}[-/]\\d{1,2}[-/](?:\\d{4}|\\d{2})(?:\\s+\\d{1,2}\\:\\d{2}(?:\\:\\d{2})?\\s*(?:AM|PM)?)?";
		private const string c_strTimeSpan = "(?:(?<Days>\\d+)\\.|)(?<Hours>\\d{1,2})\\:(?<Minutes>\\d{1,2})(?:\\:(?<Seconds>\\d{1,3})(?:\\.(?<Milliseconds>\\d{1,3})|)|)";
		private const string c_strWhiteSpace = "\\s+";
	}
	public delegate void AdditionalFunctionEventHandler(object sender, AdditionalFunctionEventArgs e);
	class ExpressionEval
	{
		private ArrayList _expressionlist = new ArrayList();
		private string _expression = "";
		internal Hashtable _variables = new Hashtable();
		private bool _bParsed;

		public ExpressionEval()
		{
		}

		public ExpressionEval(string expression)
		{
			this.Expression = expression;
		}

		public string Expression
		{
			get
			{
				return this._expression;
			}
			set
			{
				this._expression = value.Trim();
				this._bParsed = false;
				this._expressionlist.Clear();
			}
		}

		public object Evaluate()
		{
			if ((this.Expression ?? "") == "")
				return (object)0;
			return this.ExecuteEvaluation();
		}

		public bool EvaluateBool()
		{
			return Convert.ToBoolean(this.Evaluate(), (IFormatProvider)CultureInfo.CurrentCulture);
		}

		public int EvaluateInt()
		{
			return Convert.ToInt32(this.Evaluate(), (IFormatProvider)CultureInfo.CurrentCulture);
		}

		public double EvaluateDouble()
		{
			return Convert.ToDouble(this.Evaluate(), (IFormatProvider)CultureInfo.CurrentCulture);
		}

		public long EvaluateLong()
		{
			return Convert.ToInt64(this.Evaluate(), (IFormatProvider)CultureInfo.CurrentCulture);
		}

		public static object Evaluate(string expressionString)
		{
			return new ExpressionEval(expressionString).Evaluate();
		}

		public static object Evaluate(string expression, _7Sharp.AdditionalFunctionEventHandler handler)
		{
			ExpressionEval expressionEval = new ExpressionEval(expression);
			expressionEval.AdditionalFunctionEventHandler += handler;
			return expressionEval.Evaluate();
		}

		public void SetVariable(string key, object value)
		{
			this.ClearVariable(key);
			this._variables.Add((object)key, value);
		}

		public void ClearVariable(string key)
		{
			if (!this._variables.ContainsKey((object)key))
				return;
			this._variables.Remove((object)key);
		}

		public override string ToString()
		{
			return "(" + this.Expression + ")";
		}

		private object ExecuteEvaluation()
		{
			if (!this._bParsed)
			{
				int nIdx = 0;
				while (nIdx < this.Expression.Length)
					nIdx = this.NextToken(nIdx);
			}
			this._bParsed = true;
			return this.EvaluateList();
		}

		private int NextToken(int nIdx)
		{
			Match match1 = (Match)null;
			object obj = (object)null;
			Match match2 = DefinedRegex.WhiteSpace.Match(this.Expression, nIdx);
			if (match2.Success && match2.Index == nIdx)
				return nIdx + match2.Length;
			Match match3 = DefinedRegex.Parenthesis.Match(this.Expression, nIdx);
			if (match3.Success)
				match1 = match3;
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.Function.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
					match1 = match4;
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.Variable.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)new ExpressionEval.Variable(match4.Groups["Variable"].Value, this._variables);
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.UnaryOp.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)new ExpressionEval.UnaryOp(match4.Value);
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.Hexadecimal.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)Convert.ToInt32(match4.Value, 16);
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.Boolean.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)bool.Parse(match4.Value);
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.DateTime.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)Convert.ToDateTime(match4.Groups["DateString"].Value, (IFormatProvider)CultureInfo.CurrentCulture);
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.TimeSpan.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)new TimeSpan(int.Parse("0" + match4.Groups["Days"].Value), int.Parse(match4.Groups["Hours"].Value), int.Parse(match4.Groups["Minutes"].Value), int.Parse("0" + match4.Groups["Seconds"].Value), int.Parse("0" + match4.Groups["Milliseconds"].Value));
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.Numeric.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					while (match4.Success && (match4.Value ?? "") == "")
						match4 = match4.NextMatch();
					if (match4.Success)
					{
						match1 = match4;
						obj = (object)double.Parse(match4.Value, (IFormatProvider)CultureInfo.CurrentCulture);
					}
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.String.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)match4.Groups["String"].Value.Replace("\\\"", "\"");
				}
			}
			if (match1 == null || match1.Index > nIdx)
			{
				Match match4 = DefinedRegex.BinaryOp.Match(this.Expression, nIdx);
				if (match4.Success && (match1 == null || match4.Index < match1.Index))
				{
					match1 = match4;
					obj = (object)new ExpressionEval.BinaryOp(match4.Value);
				}
			}
			if (match1 == null)
				throw new ArgumentException("Invalid expression construction: \"" + this.Expression + "\".");
			if (match1.Index != nIdx)
				throw new ArgumentException("Invalid token in expression: [" + this.Expression.Substring(nIdx, match1.Index - nIdx).Trim() + "]");
			int index;
			if (match1.Value == "(" || match1.Value.StartsWith("$"))
			{
				index = match1.Index + match1.Length;
				int num = 1;
				bool flag = false;
				while (num > 0)
				{
					if (index >= this.Expression.Length)
						throw new ArgumentException("Missing " + (flag ? "\"" : ")") + " in Expression");
					if (!flag && this.Expression[index] == ')')
						--num;
					if (!flag && this.Expression[index] == '(')
						++num;
					if (this.Expression[index] == '"' && (index == 0 || this.Expression[index - 1] != '\\'))
						flag = !flag;
					++index;
				}
				if (match1.Value == "(")
				{
					ExpressionEval expressionEval = new ExpressionEval(this.Expression.Substring(match1.Index + 1, index - match1.Index - 2));
					// ISSUE: reference to a compiler-generated field
					if (this.AdditionalFunctionEventHandler != null)
					{
						// ISSUE: reference to a compiler-generated field
						expressionEval.AdditionalFunctionEventHandler += this.AdditionalFunctionEventHandler;
					}
					expressionEval._variables = this._variables;
					this._expressionlist.Add((object)expressionEval);
				}
				else
				{
					FunctionEval functionEval = new FunctionEval(this.Expression.Substring(match1.Index, index - match1.Index));
					// ISSUE: reference to a compiler-generated field
					if (this.AdditionalFunctionEventHandler != null)
					{
						// ISSUE: reference to a compiler-generated field
						functionEval.AdditionalFunctionEventHandler += this.AdditionalFunctionEventHandler;
					}
					functionEval._variables = this._variables;
					this._expressionlist.Add((object)functionEval);
				}
			}
			else
			{
				index = match1.Index + match1.Length;
				this._expressionlist.Add(obj);
			}
			return index;
		}

		private object EvaluateList()
		{
			ArrayList expressionlist = (ArrayList)this._expressionlist.Clone();
			for (int index = 0; index < expressionlist.Count; ++index)
			{
				if (expressionlist[index] is ExpressionEval.UnaryOp)
				{
					expressionlist[index] = ExpressionEval.PerformUnaryOp((ExpressionEval.UnaryOp)expressionlist[index], expressionlist[index + 1]);
					expressionlist.RemoveAt(index + 1);
				}
			}
			ExpressionEval.BinaryOpQueue binaryOpQueue = new ExpressionEval.BinaryOpQueue(expressionlist);
			string str = "";
			int index1 = 1;
			while (index1 < expressionlist.Count)
			{
				if (expressionlist[index1] is ExpressionEval.BinaryOp)
				{
					if (index1 + 1 == expressionlist.Count)
						throw new ArgumentException("Expression cannot end in a binary operation: [" + expressionlist[index1].ToString() + "]");
				}
				else
				{
					str += string.Format("\n{0} [?] {1}", expressionlist[index1 - 1] is string ? (object)("\"" + expressionlist[index1 - 1] + "\"") : expressionlist[index1 - 1], expressionlist[index1] is string ? (object)("\"" + expressionlist[index1] + "\"") : expressionlist[index1]);
					--index1;
				}
				index1 += 2;
			}
			if (str != "")
				throw new ArgumentException("Missing binary operator: " + str);
			for (ExpressionEval.BinaryOp binaryOp = binaryOpQueue.Dequeue(); binaryOp != null; binaryOp = binaryOpQueue.Dequeue())
			{
				int index2 = expressionlist.IndexOf((object)binaryOp);
				expressionlist[index2 - 1] = ExpressionEval.PerformBinaryOp((ExpressionEval.BinaryOp)expressionlist[index2], expressionlist[index2 - 1], expressionlist[index2 + 1]);
				expressionlist.RemoveAt(index2);
				expressionlist.RemoveAt(index2);
			}
			return !(expressionlist[0] is IExpression) ? expressionlist[0] : ((IExpression)expressionlist[0]).Evaluate();
		}

		private static int OperatorPrecedence(string strOp)
		{
			switch (strOp)
			{
				case "!=":
				case "=":
				case "==":
					return 4;
				case "%":
				case "*":
				case "/":
					return 0;
				case "&":
					return 5;
				case "&&":
					return 8;
				case "+":
				case "-":
					return 1;
				case "<":
				case "<=":
				case ">":
				case ">=":
					return 3;
				case "<<":
				case ">>":
					return 2;
				case "^":
					return 6;
				case "|":
					return 7;
				case "||":
					return 9;
				default:
					throw new ArgumentException("Operator " + strOp + "not defined.");
			}
		}

		private static object PerformBinaryOp(ExpressionEval.BinaryOp op, object v1, object v2)
		{
			IExpression expression1 = v1 as IExpression;
			if (expression1 != null)
				v1 = expression1.Evaluate();
			IExpression expression2 = v2 as IExpression;
			if (expression2 != null)
				v2 = expression2.Evaluate();
			switch (op.Op)
			{
				case "!=":
				case "+":
				case "-":
				case "<":
				case "<=":
				case "=":
				case "==":
				case ">":
				case ">=":
					return ExpressionEval.DoSpecialOperator(op, v1, v2);
				case "%":
					return (object)(Convert.ToInt64(v1, (IFormatProvider)CultureInfo.CurrentCulture) % Convert.ToInt64(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case "&":
					return (object)(ulong)((long)Convert.ToUInt64(v1, (IFormatProvider)CultureInfo.CurrentCulture) & (long)Convert.ToUInt64(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case "&&":
					return (object)(bool)(!Convert.ToBoolean(v1, (IFormatProvider)CultureInfo.CurrentCulture) ? false : (Convert.ToBoolean(v2, (IFormatProvider)CultureInfo.CurrentCulture) ? true : false));
				case "*":
					return (object)(Convert.ToDouble(v1, (IFormatProvider)CultureInfo.CurrentCulture) * Convert.ToDouble(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case "/":
					return (object)(Convert.ToDouble(v1, (IFormatProvider)CultureInfo.CurrentCulture) / Convert.ToDouble(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case "<<":
					return (object)(Convert.ToInt64(v1, (IFormatProvider)CultureInfo.CurrentCulture) << Convert.ToInt32(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case ">>":
					return (object)(Convert.ToInt64(v1, (IFormatProvider)CultureInfo.CurrentCulture) >> Convert.ToInt32(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case "^":
					return (object)(ulong)((long)Convert.ToUInt64(v1, (IFormatProvider)CultureInfo.CurrentCulture) ^ (long)Convert.ToUInt64(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case "|":
					return (object)(ulong)((long)Convert.ToUInt64(v1, (IFormatProvider)CultureInfo.CurrentCulture) | (long)Convert.ToUInt64(v2, (IFormatProvider)CultureInfo.CurrentCulture));
				case "||":
					return (object)(bool)(Convert.ToBoolean(v1, (IFormatProvider)CultureInfo.CurrentCulture) ? true : (Convert.ToBoolean(v2, (IFormatProvider)CultureInfo.CurrentCulture) ? true : false));
				default:
					throw new ArgumentException("Binary Operator " + op.Op + "not defined.");
			}
		}

		private static object DoSpecialOperator(ExpressionEval.BinaryOp op, object v1, object v2)
		{
			if (v1 is string || v2 is string)
			{
				string str = string.Concat(v1);
				string strB = string.Concat(v2);
				switch (op.Op)
				{
					case "!=":
						return (object)(str != strB);
					case "+":
						return (object)(str + strB);
					case "-":
						throw new ArgumentException("Operator '-' invalid for strings.");
					case "<":
						return (object)(str.CompareTo(strB) < 0);
					case "<=":
						return (object)(bool)(str.CompareTo(strB) < 0 ? true : (str == strB ? true : false));
					case "=":
					case "==":
						return (object)(str == strB);
					case ">":
						return (object)(str.CompareTo(strB) > 0);
					case ">=":
						return (object)(bool)(str.CompareTo(strB) > 0 ? true : (str == strB ? true : false));
				}
			}
			if (v1 is DateTime || v2 is DateTime)
			{
				DateTime dateTime1 = Convert.ToDateTime(v1, (IFormatProvider)CultureInfo.CurrentCulture);
				DateTime dateTime2 = Convert.ToDateTime(v2, (IFormatProvider)CultureInfo.CurrentCulture);
				switch (op.Op)
				{
					case "!=":
						return (object)(dateTime1 != dateTime2);
					case "+":
						throw new ArgumentException("Operator '+' invalid for dates.");
					case "-":
						return (object)(dateTime1 - dateTime2);
					case "<":
						return (object)(dateTime1 < dateTime2);
					case "<=":
						return (object)(dateTime1 <= dateTime2);
					case "=":
					case "==":
						return (object)(dateTime1 == dateTime2);
					case ">":
						return (object)(dateTime1 > dateTime2);
					case ">=":
						return (object)(dateTime1 >= dateTime2);
				}
			}
			double num1 = Convert.ToDouble(v1, (IFormatProvider)CultureInfo.CurrentCulture);
			double num2 = Convert.ToDouble(v2, (IFormatProvider)CultureInfo.CurrentCulture);
			switch (op.Op)
			{
				case "!=":
					return (object)(num1 != num2);
				case "+":
					return (object)(num1 + num2);
				case "-":
					return (object)(num1 - num2);
				case "<":
					return (object)(num1 < num2);
				case "<=":
					return (object)(num1 <= num2);
				case "=":
				case "==":
					return (object)(num1 == num2);
				case ">":
					return (object)(num1 > num2);
				case ">=":
					return (object)(num1 >= num2);
				default:
					throw new ArgumentException("Operator '" + op.Op + "' not specified.");
			}
		}

		private static object PerformUnaryOp(ExpressionEval.UnaryOp op, object v)
		{
			IExpression expression = v as IExpression;
			if (expression != null)
				v = expression.Evaluate();
			string op1 = op.Op;
			if (op1 == "+")
				return (object)Convert.ToDouble(v, (IFormatProvider)CultureInfo.CurrentCulture);
			if (op1 == "-")
				return (object)-Convert.ToDouble(v, (IFormatProvider)CultureInfo.CurrentCulture);
			if (op1 == "!")
				return (object)!Convert.ToBoolean(v, (IFormatProvider)CultureInfo.CurrentCulture);
			if (op1 == "~")
				return (object)(ulong)~(long)Convert.ToUInt64(v, (IFormatProvider)CultureInfo.CurrentCulture);
			throw new ArgumentException("Unary Operator " + op.Op + "not defined.");
		}

		public event _7Sharp.AdditionalFunctionEventHandler AdditionalFunctionEventHandler;

		internal class BinaryOp
		{
			private string _strOp;
			private int _nPrecedence;

			public string Op
			{
				get
				{
					return this._strOp;
				}
			}

			public int Precedence
			{
				get
				{
					return this._nPrecedence;
				}
			}

			public BinaryOp(string strOp)
			{
				this._strOp = strOp;
				this._nPrecedence = ExpressionEval.OperatorPrecedence(strOp);
			}

			public override string ToString()
			{
				return this.Op;
			}
		}

		internal class BinaryOpQueue
		{
			private ArrayList _oplist = new ArrayList();

			public BinaryOpQueue(ArrayList expressionlist)
			{
				foreach (object obj in expressionlist)
					this.Enqueue(obj as ExpressionEval.BinaryOp);
			}

			public void Enqueue(ExpressionEval.BinaryOp op)
			{
				if (op == null)
					return;
				bool flag = false;
				for (int index = 0; index < this._oplist.Count && !flag; ++index)
				{
					if (((ExpressionEval.BinaryOp)this._oplist[index]).Precedence > op.Precedence)
					{
						this._oplist.Insert(index, (object)op);
						flag = true;
					}
				}
				if (flag)
					return;
				this._oplist.Add((object)op);
			}

			public ExpressionEval.BinaryOp Dequeue()
			{
				if (this._oplist.Count == 0)
					return (ExpressionEval.BinaryOp)null;
				ExpressionEval.BinaryOp binaryOp = (ExpressionEval.BinaryOp)this._oplist[0];
				this._oplist.RemoveAt(0);
				return binaryOp;
			}

			public int Count
			{
				get
				{
					return this._oplist.Count;
				}
			}
		}

		internal class UnaryOp
		{
			private string _strOp;

			public string Op
			{
				get
				{
					return this._strOp;
				}
			}

			public UnaryOp(string strOp)
			{
				this._strOp = strOp;
			}
		}

		internal class Variable : IExpression
		{
			private string _expr;
			private Hashtable _vals;

			internal Variable(string expression, Hashtable vals)
			{
				this._expr = expression;
				this._vals = vals;
			}

			public string Expression
			{
				get
				{
					return this._expr;
				}
				set
				{
					this._expr = value;
				}
			}

			public object Evaluate()
			{
				return this._vals[(object)this._expr];
			}
		}
	}
}
