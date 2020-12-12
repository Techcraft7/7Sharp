using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class ReturnNode : Node
	{
		private readonly List<Token<TokenType>> valueTokens;

		public ReturnNode(List<Token<TokenType>> valueTokens, LexerPosition linePosition) : base(linePosition)
		{
			this.valueTokens = valueTokens;
		}

		public override void Run(ref InterpreterState state)
		{
			state.ReturnValue = valueTokens.Count > 0 ? state.TryParse<object>(valueTokens, $"Error returning at {linePosition}") : null;
			state.ExitFunc = true;
		}

		public static bool IsReturn(List<Token<TokenType>> expr)
		{
			return expr.Count >= 2 &&
				expr.First().TokenID == TokenType.RETURN &&
				expr.Last().TokenID == TokenType.SEMICOLON;
		}
	}
}
