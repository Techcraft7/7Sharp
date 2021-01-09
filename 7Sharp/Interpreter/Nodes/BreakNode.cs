using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter.Nodes
{
	internal class BreakNode : Node
	{
		public BreakNode(LexerPosition linePosition) : base(linePosition)
		{
		}

		public override void Run(ref InterpreterState state) => state.BreakUsed = true;
		public static bool IsBreak(List<Token<TokenType>> expr)
		{
			return expr.Count == 2 &&
				expr.First().TokenID == TokenType.BREAK &&
				expr.Last().TokenID == TokenType.SEMICOLON;
		}
	}
}
