using CodingSeb.ExpressionEvaluator;
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
		private readonly List<Token<TokenType>> valueTokens;
		private readonly bool isArrayAssignment;

		public AssignmentNode(string name, List<Token<TokenType>> valueTokens, bool isArrayAssignment, LexerPosition linePosition) : base(linePosition)
		{
			this.name = name;
			this.valueTokens = valueTokens;
			this.isArrayAssignment = isArrayAssignment;
		}

		public override void Run(ref InterpreterState state)
		{
			ExpressionEvaluator evaluator = state.evaluator;
			state.RunWithVariables((ref Dictionary<string, object> vars) =>
			{
				object value = null;
				if (isArrayAssignment)
				{
					value = valueTokens
						.Skip(1).Reverse()							// Remove [
						.Skip(1).Reverse()							// Remove ]
						.ToList().Split(TokenType.COMMA)			// Split by ,
						.Select(list => list.AsString()).ToList()	// Convert to strings
						.Select(s => evaluator.Evaluate(s))			// Evaluate
						.ToArray();
				}
				else
				{
					value = evaluator.Evaluate(valueTokens.AsString());
				}
				// Update value if it exists
				if (vars.ContainsKey(name))
				{
					vars[name] = value;
					return;
				}
				// Otherwise add it
				vars.Add(name, value);
			});
		}

		public static bool IsAssignment(List<Token<TokenType>> tokens) => tokens.Count > 3 &&
				// Start
				tokens[0].TokenID == TokenType.IDENTIFIER &&
				tokens[1].TokenID == TokenType.ASSIGNMENT &&
				// End
				tokens[tokens.Count - 1].TokenID == TokenType.SEMICOLON;

		public override string ToString() => $"Assignment {{ {name} = {valueTokens.AsString()}, isArrayAssignment = {isArrayAssignment} }}";

		public static bool IsArrayAssignment(List<Token<TokenType>> tokens)
		{
			if (!IsAssignment(tokens))
			{
				return false;
			}
			//x = [ ... ];
			return tokens[2].TokenID == TokenType.LBRACKET &&
				tokens[tokens.Count - 2].TokenID == TokenType.RBRACKET;
		}
	}
}
