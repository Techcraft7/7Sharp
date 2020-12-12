using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class ElseNode : BlockNode
	{
		public ElseNode(LexerPosition linePosition) : base(linePosition)
		{
		}

		public override string GetName() => "Else";

		public override void Run(ref InterpreterState state)
		{
			// If the if statement was successful, dont run
			if (state.LastIfResult)
			{
				return;
			}
			state.LastIfResult = false; // If statement is complete
			base.RunAllNodes(ref state);
		}

		public static bool IsElse(List<Token<TokenType>> tokens) => tokens.Count == 2 &&
				tokens[0].TokenID == TokenType.ELSE &&
				tokens[1].TokenID == TokenType.LBRACE;
	}
}
