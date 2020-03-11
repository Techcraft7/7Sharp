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

namespace _7Sharp.Intrerpreter
{
	using static ColorConsoleMethods;
	using static Console;
	using static ConsoleColor;
	using static TokenType;
	public class Interpreter
	{
		internal Dictionary<string, _7sFunction> functions = new Dictionary<string, _7sFunction>();
		internal Stack<int> loopIndexes = new Stack<int>();
		internal ExpressionEvaluator ee = new ExpressionEvaluator();
		public void Run(string code)
		{
			//Write("\n\nCreating tokens\n\n");
			//create tokens
			var built = LexerBuilder.BuildLexer<TokenType>();
			ILexer<TokenType> lexer = built.Result;
			LexerResult<TokenType> result = lexer.Tokenize(code);
			if (result.IsError)
			{
				WriteLineColor($"Error: unexpected \'{result.Error.UnexpectedChar}\' at line {result.Error.Line + 1}, char {result.Error.Column + 1}", Red);
				return;
			}
			result.Tokens.RemoveAt(result.Tokens.Count - 1); //last one is always a double? (probably a bug)
															 //init expression evaluator
			ee.PreEvaluateFunction += Add7sFunctions;
			//evaluate
			//Write("\n\nParsing\n\n");
			var tokens = result.Tokens.ToArray();
			for (int i = 0; i < tokens.Length; i++)
			{
				//get expression
				List<Token<TokenType>> expression = new List<Token<TokenType>>();
				do
				{
					if (tokens[i].TokenID != COMMENT)
					{
						expression.Add(tokens[i]);
					}
					i++;
				}
				while (IsNotEndOfExpression(expression) && i < tokens.Length);
				//evaluate
				if (IsNotEndOfExpression(expression))
				{
					WriteLineColor($"Expected \';\' at line {expression.Last().Position.Line + 1}, char {expression.Last().Position.Column + 1}", Red);
					return;
				}
				if (IsVarExpression(expression))
				{
					string expr = code.Substring(expression[1].Position.Index + 1, expression.FindLast(t => t.TokenID == SEMICOLON).Position.Index - 5);

					ee.Variables.Add(expression.First().Value, ee.Evaluate(expr));
				}
				else if (IsLoopExpression(expression))
				{
					List<Token<TokenType>> looped = new List<Token<TokenType>>();
					int count = 0;
					for (int j = i; j < tokens.Length; j++)
					{
						while (j < tokens.Length)
						{
							if (tokens[j].TokenID == RBRACE)
							{
								WriteLine("END");
								j = tokens.Length;
								continue;
							}
							looped.Add(tokens[j]);
							WriteLine(tokens[j]);
							j++;
						}
						if (count != 0)
						{
							WriteLineColor("Mismatched braces!", Red);
							return;
						}
					}
					string expr = "";
					foreach (var t in looped)
					{
						if (t.TokenID != COMMENT)
						{
							expr += t.Value;
						}
					}
					//get args
					int start = expression.Find(x => x.TokenID == LPAREN).Position.Index + 1;
					int len = expression.FindLast(x => x.TokenID == RPAREN).Position.Index - start;
					string args = code.Substring(start, len);
					int times = (int)ee.Evaluate(args);
					loopIndexes.Push(0);
					for (int j = 0; j < times; j++)
					{
						loopIndexes.Push(loopIndexes.Pop() + 1);
						Run(expr);
					}
				}
				else if (IsFunctionCall(expression))
				{
					string expr = "";
					foreach (var t in expression)
					{
						if (!new TokenType[] { COMMENT, SEMICOLON }.Contains(t.TokenID))
						{
							expr += t.Value;
						}
					}
					_ = ee.Evaluate(expr);
				}
				i--;
			}
		}

		private bool IsLoopExpression(List<Token<TokenType>> expression)
		{
			bool result = true;
			result &= new TokenType[] { LOOP, WHILE }.Contains(expression.First().TokenID);
			return result;
		}
		private bool IsFunctionCall(List<Token<TokenType>> expression)
		{
			try
			{
				bool result = true;
				result &= expression[0].TokenID == IDENTIFIER; //if first is identifier
				result &= expression[1].TokenID == LPAREN; //if second is (
				return result;
			}
			catch
			{
				return false;
			}
		}

		private bool IsVarExpression(List<Token<TokenType>> expression)
		{
			bool result = true;
			result &= expression[0].TokenID == IDENTIFIER; //if first is identifier
			result &= expression[1].TokenID == ASSIGNMENT; //if second is =
			return result;
		}

		private bool IsNotEndOfExpression(List<Token<TokenType>> list) => list.Count == 0 || !new TokenType[] { SEMICOLON, LBRACE }.Contains(list.Last().TokenID);

		private void Add7sFunctions(object sender, FunctionPreEvaluationEventArg e)
		{
			foreach (string f in functions.Keys)
			{
				if (e.Name == f)
				{
					int nargs = functions[f].NumberOfArguments;
					if (e.Args.Count != nargs)
					{
						WriteLineColor($"Function {f} has {functions[f].NumberOfArguments} arguments, you put {e.Args.Count}", Red);
						e.CancelEvaluation = true;
						return;
					}
					dynamic ret;
					if (nargs != 0)
					{
						functions[f].Run(out ret, nargs != 1 ? e.EvaluateArgs() : e.EvaluateArg(0));
					}
					else
					{
						functions[f].Run(out ret);
					}
					e.Value = ret;
				}
			}
		}
	}
}
