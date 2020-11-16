using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class IfNode : BlockNode
	{
		private readonly string condition;
		private readonly bool isElseIf;

		public IfNode(List<Token<TokenType>> condition, bool isElseIf, LexerPosition linePosition) : base(linePosition)
		{
			this.condition = condition.AsString();
			this.isElseIf = isElseIf;
		}

		public override string GetName() => (isElseIf ? "Else " : string.Empty) + "If";
		
		public override void Run(ref InterpreterState state)
		{
			// If this node is an else if, AND the last if was successful, then dont run
			if (isElseIf && state.LastIfResult)
			{
				return;
			}
			// Reset the last result if we are a not an else if statment
			if (!isElseIf)
			{
				state.LastIfResult = false;
			}
			// Try to parse condition as bool or error
			state.LastIfResult = state.TryParse<bool>(condition, $"{GetName()} condition did not evaluate to a true/false value at {state.Location}");
			if (state.LastIfResult)
			{
				foreach (Node child in Children)
				{
					child.Run(ref state);
				}
			}
		}

		public static bool IsIf(List<Token<TokenType>> tokens) => tokens.Count >= 4 &&
			// Start
			tokens[0].TokenID == TokenType.IF &&
			tokens[1].TokenID == TokenType.LPAREN &&
			// End
			tokens[tokens.Count - 2].TokenID == TokenType.RPAREN &&
			tokens[tokens.Count - 1].TokenID == TokenType.LBRACE;
		
		public static bool IsElseIf(List<Token<TokenType>> tokens) => tokens.Count >= 5 &&
			// Start
			tokens[0].TokenID == TokenType.ELSE &&
			tokens[1].TokenID == TokenType.IF &&
			tokens[2].TokenID == TokenType.LPAREN &&
			// End
			tokens[tokens.Count - 2].TokenID == TokenType.RPAREN &&
			tokens[tokens.Count - 1].TokenID == TokenType.LBRACE;
		
		public override string ToString() => $"{GetName()} {{ {condition} }}";
	}
}
