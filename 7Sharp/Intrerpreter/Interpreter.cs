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
	public class Interpreter
	{
		internal Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
		internal Dictionary<string, _7sFunction> functions = new Dictionary<string, _7sFunction>();
		public void Run(string code)
		{
			ExpressionEvaluator ee = new ExpressionEvaluator();
			//create tokens
			var built = LexerBuilder.BuildLexer<TokenType>();
			ILexer<TokenType> lexer = built.Result;
			LexerResult<TokenType> result = lexer.Tokenize(code);
			if (result.IsError)
			{
				WriteLine($"Error: unexpected \'{result.Error.UnexpectedChar}\' at line {result.Error.Line + 1}, char {result.Error.Column + 1}");
				return;
			}
			result.Tokens.RemoveAt(result.Tokens.Count - 1);
			foreach (var i in result.Tokens)
			{
				WriteLine(i);
			}
			//create vars
		}
	}
}
