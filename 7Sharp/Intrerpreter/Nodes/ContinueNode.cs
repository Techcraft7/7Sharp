using sly.lexer;
using System;
using System.Linq;
using System.Collections.Generic;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class ContinueNode : Node
	{
		public ContinueNode(LexerPosition linePosition) : base(linePosition)
		{
		}

		public override void Run(ref InterpreterState state) => state.ContinueUsed = true;
		public static bool IsContinue(List<Token<TokenType>> expr)
		{
			return expr.Count == 2 &&
				expr.First().TokenID == TokenType.CONTINUE &&
				expr.Last().TokenID == TokenType.SEMICOLON;
		}
	}
}
