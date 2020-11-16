using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class AssignmentNode : Node
	{
		private readonly string name;
		private readonly string value;

		public AssignmentNode(string name, string value, LexerPosition linePosition) : base(linePosition)
		{
			this.name = name;
			this.value = value;
		}

		public override void Run(ref InterpreterState state)
		{
			// Update value if it exists
			if (state.Variables.ContainsKey(name))
			{
				state.Variables[name] = state.evaluator.Evaluate(value);
				return;
			}
			// Otherwise add it
			state.Variables.Add(name, state.evaluator.Evaluate(value));
		}

		public static bool IsAssignment(List<Token<TokenType>> tokens) => tokens.Count > 3 &&
				// Start
				tokens[0].TokenID == TokenType.IDENTIFIER &&
				tokens[1].TokenID == TokenType.ASSIGNMENT &&
				// End
				tokens[tokens.Count - 1].TokenID == TokenType.SEMICOLON;

		public override string ToString() => $"Assignment {{ {name} = {value} }}";
	}
}
