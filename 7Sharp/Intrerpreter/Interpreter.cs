using System;
using System.Collections.Generic;
using System.Linq;
using Techcraft7_DLL_Pack.Text;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using _7Sharp;
using CodingSeb.ExpressionEvaluator;
using sly.lexer;

namespace _7Sharp.Intrerpreter
{
	using static Console;
	using static ConsoleColor;
	using static ColorConsoleMethods;
	using static TokenType;
	public class Interpreter
	{
		internal Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
		internal Dictionary<string, _7sFunction> functions = new Dictionary<string, _7sFunction>();
		internal Stack<int> loopIndexes = new Stack<int>();
		public void Run(string code)
		{
			//create tokens
			var built = LexerBuilder.BuildLexer<TokenType>();
			ILexer<TokenType> lexer = built.Result;
			LexerResult<TokenType> result = lexer.Tokenize(code);
			if (result.IsError)
			{
				WriteLine($"Error: unexpected \'{result.Error.UnexpectedChar}\' at line {result.Error.Line + 1}, char {result.Error.Column + 1}");
				return;
			}
			result.Tokens.RemoveAt(result.Tokens.Count - 1); //last one is always a double? (probably a bug)
			foreach (var i in result.Tokens)
			{
				WriteLine(i);
			}
			//init expression evaluator
			ExpressionEvaluator ee = new ExpressionEvaluator();
			ee.PreEvaluateFunction += Add7sFunctions;
			//create vars
			var tokens = result.Tokens.ToArray();
			for (int i = 0; i < tokens.Length; i++)
			{
				while (tokens[i].TokenID != SEMICOLON)
				{

				}
			}
		}

		private void Add7sFunctions(object sender, FunctionPreEvaluationEventArg e)
		{
			foreach (string f in functions.Keys)
			{
				if (e.Name == f)
				{
					WriteLine($"Found function call {f}");
					if (e.Args.Count != functions[f].NumberOfArguments)
					{
						WriteLineColor($"Function {f} has {functions[f].NumberOfArguments} arguments, you put {e.Args.Count}", Red);
						e.CancelEvaluation = true;
					}
				}
			}
		}
	}
}
