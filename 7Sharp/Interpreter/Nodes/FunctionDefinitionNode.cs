using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter.Nodes
{
	internal class FunctionDefinitionNode : BlockNode
	{
		public readonly string Name;
		public readonly string[] Args;
		private static readonly TokenType[] PARAMS_TOKENS = new TokenType[]
		{
			TokenType.IDENTIFIER,
			TokenType.COMMA
		};

		public FunctionDefinitionNode(string name, string[] args, LexerPosition linePosition) : base(linePosition)
		{
			Name = name ?? throw new ArgumentNullException();
			Args = args ?? throw new ArgumentNullException();
		}

		public UserFunction GetFunction() => new UserFunction(Name, Args, Children);

		public override string GetName() => "Function Definition";

		public override void Run(ref InterpreterState state) => throw new InvalidOperationException("Cannot run function call node! This is a bug!");

		public override string ToString() => $"{GetName()} {{ {Name}({string.Join(", ", Args)}) }}";

		public static bool IsFunctionDefinition(List<Token<TokenType>> tokens)
		{
			bool result = true;
			result &= tokens.Count >= 4;
			result &= tokens[0].TokenID == TokenType.FUNCTION;
			if (!result)
			{
				return false;
			}
			result &= tokens[1].TokenID == TokenType.IDENTIFIER;
			result &= tokens[2].TokenID == TokenType.LPAREN;

			// Example token layout of a function
			//-8        -7 -6 -5 -4 -3 -2 -1
			// 0         1  2  3  4  5  6  7
			// function  a  (  b  ,  c  )  {
			int i = 3;
			while (i < tokens.Count && PARAMS_TOKENS.Contains(tokens[i].TokenID))
			{
				result &= tokens[i].TokenID == (i % 2 == 0 ? TokenType.COMMA : TokenType.IDENTIFIER);
				i++;
			}
			result &= tokens[tokens.Count - 2].TokenID == TokenType.RPAREN;
			result &= tokens[tokens.Count - 1].TokenID == TokenType.LBRACE;
			return result;
		}
	}
}
