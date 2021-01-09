using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter.Nodes
{
	internal class LoopNode : BlockNode
	{
		private readonly List<Token<TokenType>> timesUnparsed;

		public LoopNode(List<Token<TokenType>> timesUnparsed, LexerPosition linePosition) : base(linePosition)
		{
			this.timesUnparsed = timesUnparsed;
		}

		public override string GetName() => "LOOP";

		public override void Run(ref InterpreterState state)
		{
			// Try to parse the argument as an integer, otherwise error
			int times = state.TryParse<int>(timesUnparsed, $"Loop times was not an integer at {state.Location}");
			// Dont allow negatives
			if (times < 0)
			{
				throw new InterpreterException($"Loop value cannot be negative at {state.Location}");
			}
			// Create loop index
			state.LoopIndexes.Push(0);
			for (int i = 0; i < times; i++)
			{
				state.PushScope();
				foreach (Node child in Children)
				{
					state.Location = child.linePosition;
					child.Run(ref state);
					//continue;
					if (state.ContinueUsed)
					{
						state.ContinueUsed = false;
						break;
					}
					//break;
					if (state.BreakUsed)
					{
						state.BreakUsed = false;
						i = times;
						break;
					}
				}
				state.PopScope();
				// Add 1 to current loop index
				state.LoopIndexes.Push(state.LoopIndexes.Pop() + 1);
			}
			_ = state.LoopIndexes.Pop(); // Remove loop index
		}

		public static bool IsLoopNode(List<Token<TokenType>> tokens) => tokens.Count >= 4 &&
			// Start
			tokens[0].TokenID == TokenType.LOOP &&
			tokens[1].TokenID == TokenType.LPAREN &&
			// End
			tokens[tokens.Count - 2].TokenID == TokenType.RPAREN &&
			tokens[tokens.Count - 1].TokenID == TokenType.LBRACE;

		public override string ToString() => $"Loop {{ {timesUnparsed.AsString()} }}";
	}
}