using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Interpreter.Nodes
{
	internal abstract class Node
	{
		public readonly LexerPosition linePosition;

		protected Node(LexerPosition linePosition) => this.linePosition = linePosition;

		public abstract void Run(ref InterpreterState state);
	}
}
