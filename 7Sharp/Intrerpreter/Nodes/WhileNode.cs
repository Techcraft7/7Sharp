using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.Nodes
{
	class WhileNode : BlockNode
	{
		private readonly string condition;

		public WhileNode(List<Token<TokenType>> condition, LexerPosition linePosition) : base(linePosition)
		{
			this.condition = condition.AsString();
		}

		public override string GetName() => "While";

		public override void Run(ref InterpreterState state)
		{
			// Try to parse condition as bool or error
			while (state.TryParse<bool>(condition, $"{GetName()} condition did not evaluate to a true/false value at {state.Location}"))
			{
				foreach (Node child in Children)
				{
					child.Run(ref state);
				}
			}
		}

		public static bool IsWhile(List<Token<TokenType>> tokens) => tokens.Count >= 4 &&
			// Start
			tokens[0].TokenID == TokenType.WHILE &&
			tokens[1].TokenID == TokenType.LPAREN &&
			// End
			tokens[tokens.Count - 2].TokenID == TokenType.RPAREN &&
			tokens[tokens.Count - 1].TokenID == TokenType.LBRACE;

		public override string ToString() => $"While {{ {condition} }}";
	}
}
