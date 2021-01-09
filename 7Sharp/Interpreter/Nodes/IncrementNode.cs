using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter.Nodes
{
	internal class IncrementNode : Node
	{
		private readonly string variableName;

		public IncrementNode(string varName, LexerPosition linePosition) : base(linePosition)
		{
			variableName = varName;
		}

		public override void Run(ref InterpreterState state)
		{
			try
			{
				state.RunWithVariables((ref Dictionary<string, object> vars) =>
				{
					dynamic x = vars[variableName];
					x++;
					vars[variableName] = x;
				});
			}
			catch
			{
				throw new InterpreterException($"Tried to increment a non-number variable \"{variableName}\" at {linePosition}");
			}
		}

		public override string ToString() => $"Increment {{ {variableName} }}";

		public static bool IsIncrement(List<Token<TokenType>> tokens) => tokens.Count == 3 &&
			tokens[0].TokenID == TokenType.IDENTIFIER &&
			tokens[1].TokenID == TokenType.PLUSPLUS &&
			tokens[2].TokenID == TokenType.SEMICOLON;
	}
}
