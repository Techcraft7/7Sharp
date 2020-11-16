using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal class RootNode : BlockNode
	{
		public RootNode() : base(new LexerPosition(0, 0, 0))
		{

		}

		public override string GetName() => "ROOT";
		
		public override void Run(ref InterpreterState state)
		{
			try
			{
				foreach (Node child in Children)
				{
					child.Run(ref state);
				}
			}
			catch (InterpreterException e)
			{
				Utils.PrintError(e);
			}
		}
	}
}
