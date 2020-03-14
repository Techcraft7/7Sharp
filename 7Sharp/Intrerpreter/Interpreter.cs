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
		internal ExpressionEvaluator evaluator = new ExpressionEvaluator();
		internal ILexer<TokenType> lexer;

		public Interpreter()
		{
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
			evaluator.Variables.Clear();
			foreach (var f in functions)
			{
				evaluator.Variables.Add(f.Key, f.Value.Code);
			}
		}

		public void Run(string code)
		{
			code = evaluator.RemoveComments(code);
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
			if (reset)
			{
				InitEvaluator();
			}
			if (lexer == null)
			{
				WriteLineColor($"The lexer was null! Cannot run code! Check {new string(typeof(TokenType).ToString().Replace('.', '\\').Skip(1).ToArray())}.cs for lexer errors!", Red);
				return;
			}
			//create tokens
			LexerResult<TokenType> result = lexer.Tokenize(code);
			if (result.IsError)
			{
				WriteLineColor($"Error: unexpected \'{result.Error.UnexpectedChar}\' at line {result.Error.Line + 1}, char {result.Error.Column + 1}", Red);
				return;
			}
			result.Tokens.RemoveAt(result.Tokens.Count - 1); //last one is always a double? (probably a bug)
			//evaluate
			var tokens = result.Tokens;
			for (int i = 0; i < tokens.Count; i++)
			{
				//get expression
				TokenList expression = new TokenList();
				do
				{
					if (tokens[i].TokenID != COMMENT)
					{
						expression.Add(tokens[i]);
					}
					i++;
				}
				while (IsNotEndOfExpression(expression) && i < tokens.Count);
				//evaluate
				if (IsNotEndOfExpression(expression))
				{
					string got = string.Empty;
					expression.ForEach(t =>
					{
						got += t.Value;
					});
					WriteLineColor($"Expected \';\' at line {expression.Last().Position.Line + 1}, char {expression.Last().Position.Column + 1}, but got \"{got}\"", Red);
					return;
				}
				if (IsVarExpression(expression))
				{
					string expr = GetExpressionToToken(expression, SEMICOLON);
					var x = Evaluate(expr);
					try
					{
						evaluator.Variables.Add(expression.First().Value, x);
					}
					catch
					{
						evaluator.Variables[expression.First().Value] = x;
					}
				}
				else if (IsLoopExpression(expression))
				{
					int loc = GetClosingBrace(tokens, i, out var looped, out string inside);
					if (loc < 0 || looped == null)
					{
						WriteLineColor("Error parsing loop/while block!", Red);
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
								InternalRun(expr, false);
								loopIndexes.Push(loopIndexes.Pop() + 1);
							}
							break;
						case WHILE:
							while ((bool)Evaluate(args))
							{
								InternalRun(expr, false);
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
				i--;
			}
		}

		private void ProcessIf(TokenList expression, TokenList tokens, string code, ref int i)
		{
			
			i -= expression.Count;
			int offset = expression.First().PositionInTokenFlow;
			while (!new TokenType[] { IF, ELSE }.Contains(tokens[offset].TokenID))
			{
				offset++;
			}
			int jump = GetClosingBrace(tokens, offset, out _, out _);
			while (tokens[offset].TokenID != LBRACE)
			{
				offset++;
			}
			offset++;
			if (tokens[jump + 1].TokenID == ELSE) //if another block
			{
				lookAgain:
				try
				{
					while (tokens[offset].TokenID != LBRACE)
					{
						offset++;
						_ = tokens[offset]; // throw exception if out of bounds
					}
					offset = GetClosingBrace(tokens, offset, out _, out _);
					if (!new TokenType[] { ELSE }.Contains(tokens[offset + 1].TokenID))
					{
						throw new Exception("bad code!");
					}
					goto lookAgain;
				}
				catch
				{
					//do nothing, you found the end
				}
			}
			_ = evaluator.ScriptEvaluate(code.Substring(expression.First().Position.Index, tokens[offset].Position.Index - expression.First().Position.Index + 1));
			ReadLine();
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
			if (expression.Count >= 2)
			{
				result |= expression[0].TokenID == IDENTIFIER; //if first is identifier
				result &= expression[1].TokenID == ASSIGNMENT; //if second is =
			}
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
			int l = lexer.Tokenize(expr).Tokens.Find(x => x.TokenID == type).Position.Index;
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
			TokenType[] types = new TokenType[] { LBRACE, SEMICOLON, COMMENT };
			inside.ForEach(x =>
			{
				c += x.Value;
				if (types.Contains(x.TokenID))
				{
					c += "\n";
				}
			});
			insideCode = c;
			return loc;
		}

		private string GetArgs(string expr) => expr.Substring(expr.IndexOf('(') + 1, expr.LastIndexOf(')') - expr.IndexOf('(') - 1);

		private bool IsNotEndOfExpression(TokenList list) => list.Count == 0 || !new TokenType[] { SEMICOLON, LBRACE, RBRACE }.Contains(list.Last().TokenID);
	}
}
