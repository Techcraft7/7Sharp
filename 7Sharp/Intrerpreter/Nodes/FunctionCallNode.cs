using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class FunctionCallNode : Node
	{
		private readonly _7sFunction func;
		private readonly UserFunction userFunc;
		private readonly bool isUserFunc;
		private readonly List<List<Token<TokenType>>> args;

		public FunctionCallNode(UserFunction userFunc, List<List<Token<TokenType>>> args, LexerPosition linePosition) : base(linePosition)
		{
			func = null;
			this.userFunc = userFunc;
			isUserFunc = true;
			this.args = args;
		}

		public FunctionCallNode(_7sFunction func, List<List<Token<TokenType>>> args, LexerPosition linePosition) : base(linePosition)
		{
			this.func = func ?? throw new InterpreterException($"Unknown function at {linePosition}");
			this.args = args;
		}

		public override void Run(ref InterpreterState state)
		{
			if (isUserFunc)
			{
				userFunc.Run(ref state, ParseArgs(args, state));
			}
			else
			{
				state.ReturnValue = func.Run(ParseArgs(args, state));
			}
		}

		private static object[] ParseArgs(List<List<Token<TokenType>>> args, InterpreterState state)
		{
			if (args.Count == 1 && args[0].Count == 0)
			{
				return new object[0];
			}
			return args
				.Select(list => state.evaluator.Evaluate(list.AsString()))
				.ToArray();
		}

		public static bool IsFunctionCall(List<Token<TokenType>> tokens) => tokens.Count >= 4 &&
				// Start
				tokens[0].TokenID == TokenType.IDENTIFIER &&
				tokens[1].TokenID == TokenType.LPAREN &&
				// End
				tokens[tokens.Count - 2].TokenID == TokenType.RPAREN &&
				tokens[tokens.Count - 1].TokenID == TokenType.SEMICOLON;
		public static _7sFunction GetFunction(InterpreterState state, string name)
		{
			// If function exists, return it, otherwise return null
			foreach (var f in state.Functions)
			{
				if (f.Name.Equals(name))
				{
					return f;
				}
			}
			return null;
		}

		public override string ToString() => $"Function Call {{ {(isUserFunc ? userFunc.Name : func.Name)}({string.Join(", ", args.Select(list => list.AsString()))}) }} ";
	}
}
