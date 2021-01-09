using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter.Nodes
{
	internal class ImportNode : Node
	{
		public readonly string Library;

		public ImportNode(string lib, LexerPosition linePosition) : base(linePosition)
		{
			Library = lib;
		}

		public override void Run(ref InterpreterState state) => throw new InterpreterException("Do not run ImportNode! This is a bug!");
		public static bool IsImport(List<Token<TokenType>> expr)
		{
			return expr.Count == 3 &&
				expr[0].TokenID == TokenType.IMPORT &&
				expr[1].TokenID == TokenType.STRING &&
				expr[2].TokenID == TokenType.SEMICOLON;
		}
	}
}
