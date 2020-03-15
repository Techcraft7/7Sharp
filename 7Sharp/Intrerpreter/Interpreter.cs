using _7Sharp;
using CodingSeb.ExpressionEvaluator;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;
using TokenList = System.Collections.Generic.List<sly.lexer.Token<_7Sharp.Intrerpreter.TokenType>>;

namespace _7Sharp.Intrerpreter
{
	using static Utils;
	using static ColorConsoleMethods;
	using static Console;
	using static ConsoleColor;
	using static TokenType;
	public class Interpreter
	{
		internal Dictionary<string, _7sFunction> functions = new Dictionary<string, _7sFunction>();
		internal Stack<int> loopIndexes = new Stack<int>();
		private ExpressionEvaluator evaluator = new ExpressionEvaluator();
		private ILexer<TokenType> lexer;
		private bool exit = false;

		public Interpreter()
		{
			_ = evaluator.Evaluate("test = 0");
			InitEvaluator();
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

		private void InitEvaluator()
		{
			functions.Clear();
			var self = this;
			SystemFunctions.Init(ref self);
			evaluator.Variables.Clear();
			foreach (var f in functions)
			{
				evaluator.Variables.Add(f.Key, f.Value.Code);
			}
		}

		public void Run(string code)
		{
			exit = false;
			code = evaluator.RemoveComments(code); //just to make parsing easier
			try
			{
				InternalRun(code, true);
			}
			catch (Exception e)
			{
				PrintError(e);
			}
			evaluator = new ExpressionEvaluator();
		}

		private void InternalRun(string code, bool reset = true)
		{

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
				if (!IsNotEndOfExpression(tokens[i]))
				{
					i++;
				}
				//
				//get expression
				TokenList expression = new TokenList();
				if (i >= tokens.Count)
				{
					return;
				}
				do
				{
					if (tokens[i].TokenID != COMMENT)
					{
						expression.Add(tokens[i]);
					}
					i++;
				}
				while (i < tokens.Count && IsNotEndOfExpression(tokens[i]));
				if (i < tokens.Count && !IsNotEndOfExpression(tokens[i]))
				{
					expression.Add(tokens[i]);
				}
				while (!IsNotEndOfExpression(expression.First()))
				{
					expression.RemoveAt(0);
				}
				//evaluate
				if (expression.Count <= 1) //empty expression or just a brace
				{
					continue;
				}
				else if (IsNotEndOfExpression(tokens[i < tokens.Count ? i : tokens.Count - 1]))
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
					switch (expression.First().TokenID)
					{
						case LOOP:
							int times = (int)Evaluate(args);
							loopIndexes.Push(0);
							for (int j = 0; j < times; j++)
							{
								InternalRun(inside, false);
								loopIndexes.Push(loopIndexes.Pop() + 1);
							}
							break;
						case WHILE:
							while ((bool)Evaluate(args))
							{
								InternalRun(inside, false);
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
				else if (IsNotEndOfExpression(tokens[i]))
				{
					WriteLineColor($"Invalid syntax at line {tokens[i].Position.Line + 1}, col {tokens[i].Position.Column + 1}: {tokens[i].Value}", Red);
					exit = true;
					return;
				}
				i--;
			}
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
				next = true;
				end++;
			}
			switch (expression[0].TokenID)
			{
				case IF:
					if ((bool)Evaluate(args))
					{
						InternalRun(inside, false);
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
					InternalRun(inside, false);
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

		private string GetExpressionToToken(TokenList tokens, TokenType type)
		{
			string expr = string.Empty;
			foreach (var t in tokens)
			{
				expr += t.Value;
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
			int count = 0;
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
					j++;
					inside.Add(tokens[j]);
					loc = j;
				}
				inside.RemoveAt(inside.Count - 1); //remove last
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

		private bool IsNotEndOfExpression(Token<TokenType> t) => !new TokenType[] { SEMICOLON, LBRACE, RBRACE, COMMENT }.Contains(t.TokenID);
	}
}
