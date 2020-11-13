using _7Sharp;
using CodingSeb.ExpressionEvaluator;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;
using TokenList = System.Collections.Generic.List<sly.lexer.Token<_7Sharp.Intrerpreter.TokenType>>;
using _7Sharp._7sLib;

namespace _7Sharp.Intrerpreter
{
	using static Utils;
	using static ColorConsoleMethods;
	using static Console;
	using static ConsoleColor;
	using static TokenType;
	public sealed class Interpreter
	{
		internal static readonly Dictionary<string, Dictionary<string, _7sFunction>> SystemLibraries = new Dictionary<string, Dictionary<string, _7sFunction>>()
		{
			{ "random.sys", SystemFunctions.random },
			{ "io.sys", SystemFunctions.io },
		};

		internal Scope scope = new Scope();

		internal Dictionary<string, FunctionDefinition> userFunctions = new Dictionary<string, FunctionDefinition>();
		internal Dictionary<string, _7sFunction> functions = new Dictionary<string, _7sFunction>();
		internal Stack<int> loopIndexes = new Stack<int>();
		private ExpressionEvaluator evaluator = new ExpressionEvaluator();
		private ILexer<TokenType> lexer;
		private bool exit = false;
		private bool exitLoop = false;
		private bool skipLoop = false;

		public Interpreter()
		{
			InitEvaluator();
			evaluator.PreEvaluateFunction += AddUserFunctions;
			var built = LexerBuilder.BuildLexer<TokenType>();
			if (built.Errors.FindAll(x => x.Level != sly.buildresult.ErrorLevel.WARN).Count != 0)
			{
				WriteLineColor($"Could not build lexer!", Red);
				foreach (var e in built.Errors)
				{
					if (e.Level == sly.buildresult.ErrorLevel.WARN)
					{
						continue;
					}
					WriteLineColor($"\t{e.GetType()} level {e.Level}: {e.Message}", Red);
				}
				lexer = null;
			}
			else
			{

				lexer = built.Result;
			}
		}

		private void AddUserFunctions(object sender, FunctionPreEvaluationEventArg e)
		{
			foreach (var kv in userFunctions)
			{
				if (kv.Key == e.Name)
				{
					if (e.Args.Count == kv.Value.NumberOfArgs())
					{
						e.CancelEvaluation = false;
					}
					else
					{
						WriteLineColor($"{e.Name} expects {kv.Value.NumberOfArgs()}, but you put {e.Args.Count}", Red);
						e.CancelEvaluation = true;
					}
					e.Value = kv.Value.Run(e.EvaluateArgs());
				}
			}
		}

		private void InitEvaluator()
		{
			userFunctions.Clear();
			functions.Clear();
			var self = this;
			SystemFunctions.Init(ref self);
			evaluator.Variables.Clear();
			foreach (var f in functions)
			{
				evaluator.Variables.Add(f.Key, f.Value.Code);
			}
			foreach (ConsoleColor c in Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>())
			{
				evaluator.Variables.Add(c.ToString(), c);
			}
			evaluator.Assemblies.Clear();
		}

		public void Run(string code)
		{
			code = evaluator.RemoveComments(code); //just to make parsing easier
			InitEvaluator();
			try
			{
				InternalRun(code, out _, true);
			}
			catch (Exception e)
			{
				PrintError(e);
			}
			InitEvaluator();
		}

		internal void LoadLibrary(string path)
		{
			_7sLibrary library = _7sLibManager.Load(path);
			if (library != null)
			{
				var res = lexer.Tokenize(library.Content);
				if (res == null || res.IsError)
				{
					WriteLineColor("Error lexing library!", Red);
					return;
				}
				var tokens = res.Tokens;
				for (int i = 0; i < tokens.Count; i++)
				{
					TokenList expression = GetExpression(tokens, ref i);
					if (IsFunctionDef(expression))
					{
						string expr = GetExpressionToToken(expression, LBRACE);
						string[] args = GetArgs(expr).Replace(" ", string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
						int end = GetClosingBrace(tokens, i, out _, out string inside);
						userFunctions.Add(expression[1].Value, new FunctionDefinition(inside, this, args));
						i = end;
					}
				}
			}
			else
			{
				WriteLineColor("Library could not be loaded!", Red);
				return;
			}
		}

		internal void InternalRun(string code, out dynamic returnValue, bool reset = true, params KeyValuePair<string, dynamic>[] passedParams)
		{
			scope.PushScope(evaluator.Variables);
			if (passedParams != null && passedParams.Length > 0)
			{
				foreach (var kv in passedParams)
				{
					evaluator.Variables.Add(kv.Key, kv.Value);
				}
			}
			returnValue = null;
			exitLoop = skipLoop = exit = false;
			if (exit)
			{
				return;
			}
			if (reset)
			{
				InitEvaluator();
			}
			if (lexer == null)
			{
				WriteLineColor($"The lexer was null! Cannot run code! Check {new string(typeof(TokenType).ToString().Replace('.', '\\').Skip(1).ToArray())}.cs for lexer errors!", Red);
				exit = true;
				return;
			}
			//create tokens
			LexerResult<TokenType> result = lexer.Tokenize(code);
			if (result.IsError)
			{
				WriteLineColor($"Error: unexpected \'{result.Error.UnexpectedChar}\' at line {result.Error.Line + 1}, char {result.Error.Column + 1}", Red);
				exit = true;
				return;
			}
			result.Tokens.RemoveAt(result.Tokens.Count - 1); //last one is always a double? (probably a bug)
			//evaluate
			var tokens = result.Tokens;
			for (int i = 0; i < tokens.Count; i++)
			{
				if (skipLoop || exitLoop)
				{
					return;
				}
				if (!IsNotEndOfExpression(new TokenList() { tokens[i] }))
				{
					i++;
				}
				//get expression
				TokenList expression = GetExpression(tokens, ref i);
				//evaluate
				if (expression.Count <= 1) //empty expression or just a brace
				{
					continue;
				}
				else if (IsNotEndOfExpression(expression))
				{
					string got = string.Empty;
					expression.ForEach(t =>
					{
						got += t.Value;
					});
					WriteLineColor($"Expected \';\' at line {expression.Last().Position.Line + 1}, char {expression.Last().Position.Column + 1}, but got \"{expression.First().Value}\"", Red);
					exit = true;
					return;
				}
				else if (IsImport(expression))
				{
					string path = GetExpressionToToken(expression.Skip(1).ToList(), SEMICOLON);
					path = path.Substring(1, path.Length - 2);
					if (!File.Exists(path))
					{
						if (SystemLibraries.ContainsKey(path))
						{
							foreach (var kv in SystemLibraries[path])
							{
								evaluator.Variables.Add(kv.Key, kv.Value.Code);
							}
						}
						else
						{
							WriteLineColor($"File does not exist: \"{path}\"", Red);
							exit = true;
							return;
						}
					}
					else
					{
						LoadLibrary(path);
					}
				}
				else if (IsFlowStatement(expression))
				{
					switch (expression[0].TokenID)
					{
						case CONTINUE:
						case BREAK:
							exitLoop = expression[0].TokenID == BREAK;
							skipLoop = !exitLoop;
							return;
						case RETURN:
							expression = expression.Skip(1).ToList(); //remove "return" keyword
							string expr = GetExpressionToToken(expression, SEMICOLON);
							try
							{
								returnValue = Evaluate(expr);
							}
							catch
							{
								returnValue = null;
							}
							return;
					}
				}
				else if (IsFunctionDef(expression))
				{
					string expr = GetExpressionToToken(expression, LBRACE);
					string[] args = GetArgs(expr).Replace(" ", string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
					int end = GetClosingBrace(tokens, i, out TokenList ts, out string inside);
					userFunctions.Add(expression[1].Value, new FunctionDefinition(inside, this, args));
					i = end;
					continue;
				}
				else if (IsArrayExpression(expression))
				{
					
				}
				else if (IsVarExpression(expression))
				{
					string expr = GetExpressionToToken(expression, SEMICOLON);
					switch (expression[1].TokenID)
					{
						case PLUSPLUS:
						case MINUSMINUS:
							expr = $"{expression.First().Value} = {expression.First().Value} {(expression[1].TokenID == PLUSPLUS ? "+" : "-")} 1";
							break;
					}
					var x = Evaluate(expr);
				}
				else if (IsLoopExpression(expression))
				{
					int loc = GetClosingBrace(tokens, i, out var looped, out string inside);
					if (loc < 0 || looped == null)
					{
						WriteLineColor("Error parsing loop/while block!", Red);
						exit = true;
						return;
					}
					string expr = GetExpressionToToken(expression, LBRACE);
					string args = GetArgs(expr);
					exitLoop = skipLoop = false;
					switch (expression.First().TokenID)
					{
						case LOOP:
							loopIndexes.Push(0);
							try
							{
								int times = (int)Evaluate(args);
								for (int j = 0; j < times && !exitLoop; j++)
								{
									InternalRun(inside, out _, false);
									loopIndexes.Push(loopIndexes.Pop() + 1);
									skipLoop = false;
								}
								exitLoop = false;
							}
							catch (Exception error)
							{
								PrintError(error);
							}
							finally
							{
								_ = loopIndexes.Pop(); //remove loop index because we are done with it
							}
							break;
						case WHILE:
							while ((bool)Evaluate(args) && !exitLoop)
							{
								InternalRun(inside, out _, false);
							}
							break;
					}
					i = loc;
					continue;
				}
				else if (IsFunctionCall(expression))
				{
					_ = Evaluate(GetExpressionToToken(expression, SEMICOLON));
				}
				else if (IsIfElseBlock(expression))
				{
					ProcessIf(expression, tokens, code, ref i);
				}
				else if (IsNotEndOfExpression(new TokenList() { tokens[i < tokens.Count ? i : tokens.Count - 1] }))
				{
					WriteLineColor($"Invalid syntax at line {tokens[i].Position.Line + 1}, col {tokens[i].Position.Column + 1}: {tokens[i].Value}", Red);
					exit = true;
					return;
				}
				i--;
			}
			if (passedParams != null && passedParams.Length > 0)
			{
				foreach (var kv in passedParams)
				{
					evaluator.Variables.Remove(kv.Key);
				}
			}
			if (!scope.IsEmpty())
			{
				evaluator.Variables = scope.PopScope();
			}
		}

		private bool IsArrayExpression(TokenList expression)
		{
			bool result = false;
			result |= expression[0].TokenID == IDENTIFIER;//array
			result &= expression[1].TokenID == ASSIGNMENT;//=
			result &= expression[2].TokenID == LBRACKET;//[
			return result;
		}

		private TokenList GetExpression(TokenList tokens, ref int i)
		{
			TokenList expression = new TokenList();
			if (i >= tokens.Count)
			{
				return expression;
			}
			do
			{
				if (tokens[i].TokenID != COMMENT)
				{
					expression.Add(tokens[i]);
				}
				i++;
			}
			while (i < tokens.Count && IsNotEndOfExpression(expression));
			if (i < tokens.Count && !IsNotEndOfExpression(new TokenList() { tokens[i] }))
			{
				expression.Add(tokens[i]);
			}
			while (!IsNotEndOfExpression(new TokenList() { expression.First() }))
			{
				expression.RemoveAt(0);
			}
			return expression;
		}

		private bool IsImport(TokenList expression)
		{
			bool result = false;
			if (expression.Count == 3)
			{
				result |= expression[0].TokenID == IMPORT;
				result &= expression[1].TokenID == STRING;
				result &= expression[2].TokenID == SEMICOLON;
			}
			return result;
		}

		private bool IsFunctionDef(TokenList expression)
		{
			bool result = false;
			if (expression.Count >= 3)
			{
				result |= expression[0].TokenID == FUNCTION;
				result &= expression[1].TokenID == IDENTIFIER;
				result &= expression[2].TokenID == LPAREN;
			}
			return result;
		}

		private void ProcessIf(TokenList expression, TokenList tokens, string code, ref int i)
		{
			string expr = GetExpressionToToken(expression, LBRACE);
			string args = null;
			if (expression.Count > 2)
			{
				args = GetArgs(expr);
			}
			int end = GetClosingBrace(tokens, i, out _, out string inside);
			bool next = false;
			if (end + 1 < tokens.Count)
			{
				if (new TokenType[] { ELSE, IF }.Contains(tokens[end + 1].TokenID))
				{
					next = true;
					end++;
				}
				
			}
			switch (expression[0].TokenID)
			{
				case IF:
					if ((bool)Evaluate(args))
					{
						InternalRun(inside, out _, false);
						i = end;
					}
					else
					{
						i = end;
						if (next)
						{
							TokenList ts = new TokenList();
							while (i < tokens.Count && tokens[i].TokenID != LBRACE)
							{
								ts.Add(tokens[i]);
								i++;
							}
							ts.Add(tokens[i]);
							if (ts.First().TokenID == ELSE && ts.Count > 2)
							{
								ts = ts.Skip(1).ToList();
							}
							ProcessIf(ts, tokens, code, ref i);
							return;
						}
					}
					break;
				case ELSE:
					i = end;
					InternalRun(inside, out _, false);
					break;
			}
		}

		private object Evaluate(string expr)
		{
			try
			{
				return evaluator.Evaluate(expr);
			}
			catch (Exception e)
			{
				PrintError(e);
				return null;
			}
		}

		private bool IsLoopExpression(TokenList expression)
		{
			if (expression.Count < 1)
			{
				return false;
			}
			return new TokenType[] { LOOP, WHILE }.Contains(expression.First().TokenID);
		}

		private bool IsFunctionCall(TokenList expression)
		{
			bool result = false;
			if (expression.Count >= 2)
			{
				result |= expression[0].TokenID == IDENTIFIER; //if first is identifier
				result &= expression[1].TokenID == LPAREN; //if second is (
			}
			return result;
		}

		private bool IsVarExpression(TokenList expression)
		{
			bool result = false;
			result |= expression[0].TokenID == IDENTIFIER; //if first is identifier
			result &= new TokenType[] { ASSIGNMENT, PLUSPLUS, MINUSMINUS }.Contains(expression[1].TokenID);
			return result;
		}

		private bool IsIfElseBlock(TokenList expression)
		{
			bool result = false;
			if (expression.Count >= 2)
			{
				result |= new TokenType[] { IF, ELSE }.Contains(expression[0].TokenID); //if first is part of an if else block
				result &= expression[1].TokenID == LPAREN; //if second is (
				result &= expression.Last().TokenID == LBRACE;
			}
			return result;
		}

		private bool IsFlowStatement(TokenList expression)
		{
			bool result = false;
			if (expression.Count == 2)
			{
				result |= new TokenType[] { BREAK, CONTINUE }.Contains(expression[0].TokenID);
				result &= expression[1].TokenID == SEMICOLON;
			}
			return result;
		}

		private string GetExpressionToToken(TokenList tokens, TokenType type)
		{
			string expr = string.Empty;
			foreach (var t in tokens)
			{
				expr += t.Value;
				if (new TokenType[] { FUNCTION, IF, ELSE, RETURN, NEW }.Contains(t.TokenID))
				{
					expr += " ";
				}
			}
			var ts = lexer.Tokenize(expr).Tokens;
			var f = ts.Find(x => x.TokenID == type);
			int l = f.Position.Index;
			expr = expr.Remove(l, expr.Length - l);
			return expr;
		}

		private int GetClosingBrace(TokenList tokens, int start, out TokenList inside, out string insideCode)
		{
			string c = string.Empty;
			inside = new TokenList();
			int count = 1;
			int loc = 0;
			for (int j = start; j < tokens.Count; j++)
			{
				while (j < tokens.Count)
				{
					if (tokens[j].TokenID == LBRACE)
					{
						count++;
					}
					else if (tokens[j].TokenID == RBRACE && count == 0)
					{
						j = tokens.Count;
						continue;
					}
					else if (tokens[j].TokenID == RBRACE && count > 0)
					{
						count--;
						if (count == 0)
						{
							j = tokens.Count;
							continue;
						}
					}
					inside.Add(tokens[j]);
					j++;
					loc = j;
				}
				if (count != 0)
				{
					WriteLineColor("Mismatched braces!", Red);
					inside = null;
					insideCode = null;
					return -1;
				}
			}
			TokenType[] line = new TokenType[] { LBRACE, RBRACE, SEMICOLON, COMMENT };
			TokenType[] space = new TokenType[] { ELSE };
			inside.ForEach(x =>
			{
				c += x.Value;
				if (line.Contains(x.TokenID))
				{
					c += "\n";

				}
				if (space.Contains(x.TokenID))
				{
					c += " ";
				}
			});
			insideCode = c;
			return loc;
		}

		private string GetArgs(string expr) => expr.Substring(expr.IndexOf('(') + 1, expr.LastIndexOf(')') - expr.IndexOf('(') - 1);

		private bool IsNotEndOfExpression(TokenList t)
		{
			bool v = false;
			if (t.Count >= 2)
			{
				v = t[t.Count - 2].TokenID == RBRACKET && t.Last().TokenID == LBRACE;
			}
			return new TokenType[] { SEMICOLON, LBRACE, RBRACE, COMMENT }.Contains(t.Last().TokenID) == false || v;
				
		}
	}
}