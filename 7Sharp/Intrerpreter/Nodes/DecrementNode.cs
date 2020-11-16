using sly.lexer;
using System.Collections.Generic;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class DecrementNode : Node
	{
		private readonly string variableName;

		public DecrementNode(string varName, LexerPosition linePosition) : base(linePosition)
		{
			variableName = varName;
		}

		public override void Run(ref InterpreterState state)
		{
			try
			{
				dynamic x = state.Variables[variableName];
				x--;
				state.Variables[variableName] = x;
			}
			catch
			{
				throw new InterpreterException($"Tried to decrement a non-number variable \"{variableName}\" at {linePosition}");
			}
		}

		public override string ToString() => $"Decrement {{ {variableName} }}";

		public static bool IsDecrement(List<Token<TokenType>> tokens) => tokens.Count == 3 &&
			tokens[0].TokenID == TokenType.IDENTIFIER &&
			tokens[1].TokenID == TokenType.MINUSMINUS &&
			tokens[2].TokenID == TokenType.SEMICOLON;
	}
}
