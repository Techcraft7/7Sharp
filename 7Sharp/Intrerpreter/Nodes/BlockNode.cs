using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Sharp.Intrerpreter.Nodes
{
	internal abstract class BlockNode : Node
	{
		public readonly List<Node> Children = new List<Node>();

		public BlockNode(LexerPosition linePosition) : base(linePosition)
		{
		}

		public void Add(Node child) => Children.Add(child ?? throw new ArgumentNullException(nameof(child)));

		// Utility for debugging
		public void Dump(int depth = 0)
		{
			string pad = new string(' ', depth * 2); // 2 spaces padding
			Console.WriteLine(pad + ToString());
			for (int i = 0; i < Children.Count; i++)
			{
				if (Children[i] is BlockNode) // If its a block dump it's children
				{
					((BlockNode)Children[i]).Dump(depth + 1);
				}
				else
				{
					Console.WriteLine(pad + "  " + Children[i].ToString());
				}
			}
		}

		public abstract string GetName();
		public static List<Token<TokenType>> GetBlock(List<Token<TokenType>> tokens, int loc, out int end)
		{
			end = -1;
			Token<TokenType> t = tokens[loc];
			TokenType other;
			switch (t.TokenID)
			{
				case TokenType.LPAREN:
					other = TokenType.RPAREN;
					break;
				case TokenType.LBRACE:
					other = TokenType.RBRACE;
					break;
				case TokenType.LBRACKET:
					other = TokenType.RBRACKET;
					break;
				default:
					throw new ArgumentException("Invalid block token");
			}
			int depth = 0;
			for (int i = loc; i < tokens.Count; i++)
			{
				if (tokens[i].TokenID == t.TokenID)
				{
					depth++;
				}
				if (tokens[i].TokenID == other)
				{
					depth--;
					if (depth == 0)
					{
						end = i;
						return tokens.Skip(loc).Take(i - t.PositionInTokenFlow - 2).Skip(1).Reverse().Skip(1).Reverse().ToList();
					}
				}
			}
			throw new InterpreterException($"Block not closed at {t.Position.Adjust()}");
		}

		protected void RunAllNodes(ref InterpreterState state)
		{
			state.PushScope();
			foreach (Node child in Children)
			{
				state.Location = child.linePosition;
				child.Run(ref state);
			}
			state.PopScope();
		}

		public override string ToString() => GetName();
	}
}
