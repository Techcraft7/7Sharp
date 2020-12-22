using _7Sharp;
using _7Sharp._7sLib;
using _7Sharp.Intrerpreter.Nodes;
using CodingSeb.ExpressionEvaluator;
using sly.lexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;
// Alias for List<Token<TokenType>> because its a pain to type
using TokenList = System.Collections.Generic.List<sly.lexer.Token<_7Sharp.Intrerpreter.TokenType>>;

namespace _7Sharp.Intrerpreter
{
	// So we dont have to do ColorConsoleMethods all the time
	using static ColorConsoleMethods;
	using static ConsoleColor;
	using static TokenType;
	using static Utils;
	internal sealed class Interpreter
	{
		private ILexer<TokenType> lexer;

		public Interpreter()
		{
			// Build lexer
			sly.buildresult.BuildResult<ILexer<TokenType>> built = LexerBuilder.BuildLexer<TokenType>();
			// Print out lexer errors
			if (built.Errors.FindAll(x => x.Level != sly.buildresult.ErrorLevel.WARN).Count != 0)
			{
				WriteLineColor($"Could not build lexer!", Red);
				foreach (sly.buildresult.InitializationError e in built.Errors)
				{
					if (e.Level == sly.buildresult.ErrorLevel.WARN)
					{
						continue;
					}
					WriteLineColor($"\t{e.GetType()} level {e.Level}: {e.Message}", Red);
				}
				lexer = null;
			}
			else
			{
				// Lexer was build successfully
				lexer = built.Result;
			}
		}

		public void Run(string code)
		{
			if (lexer == null)
			{
				WriteLineColor("INTERNAL ERROR: LEXER COULD NOT BUILD! THIS IS A BUG!", Red);
				return;
			}
			// Remove comments
			code = new ExpressionEvaluator().RemoveComments(code);
			try
			{
				// Run code
				InternalRun(code);
			}
			catch (Exception e)
			{
				PrintError(e);
			}
		}

		private void InternalRun(string code)
		{
			// Convert code to tokens
			LexerResult<TokenType> result = lexer.Tokenize(code);
			// Error out if lexer fails
			if (result.IsError)
			{
				WriteLineColor($"Parsing Error: {result.Error}", Red);
				return;
			}

			// Get tokens
			TokenList tokens = result.Tokens;

			// Build tree
			InterpreterState state = new InterpreterState();
			InterpreterState.Init(ref state);
			RootNode root = BuildTree<RootNode>(new Queue<Token<TokenType>>(tokens), ref state);

#if DEBUG
			root.Dump();
#endif

			// Run
			root.Run(ref state);
		}

		private T BuildTree<T>(Queue<Token<TokenType>> tokens, ref InterpreterState state, bool getNodesOnly = false) where T : BlockNode, new()
		{
			T root = new T(); // Create a new node

			while (tokens.Count > 0)
			{
				TokenList expr = new TokenList();
				// If we hit EOS we are done
				Token<TokenType> token = tokens.Dequeue();
				if (token == null || token.IsEOS)
				{
#if DEBUG
					WriteLineColor("EOS", Cyan);
#endif
					break;
				}
				// While i is in bounds of tokens AND we have not hit an "expression ending" (; or {)
				while (tokens.Count() > 0 && !token.IsEnding() && !token.IsEOS)
				{
					expr.Add(token);
					token = tokens.Dequeue();
				}
				expr.Add(token);
				// If expression starts with }, remove it
				if (expr.Count > 0 && expr[0].TokenID == RBRACE)
				{
					expr = expr.Skip(1).ToList();
				}
				// Dont run empty expression
				if (expr.Count == 0)
				{
					continue;
				}
#if DEBUG
				expr.TokenDump();
#endif
				LexerPosition exprPos = expr[0].Position.Adjust();
				state.Location = exprPos;
				bool built = false;
				// Loop over every expression type
				foreach (ExpressionType type in Enum.GetValues(typeof(ExpressionType)).Cast<ExpressionType>())
				{
					if (type.Matches(expr))
					{
						Node node = type.GetNode(expr, exprPos, ref state);
						if (type.IsBlock())
						{
							BlockNode tmp = (BlockNode)node;
							Queue<Token<TokenType>> block = GetBlock(ref tokens);
							BuildTree<RootNode>(block, ref state, getNodesOnly).Children.ForEach(c => tmp.Add(c));
							node = tmp;
						}
						if (type.WillRun() || getNodesOnly)
						{
							root.Add(node);
						}
						if (!getNodesOnly)
						{
							if (node is FunctionDefinitionNode funcDefNode)
							{
								state.UserFuncs.Add(new UserFunction(
									funcDefNode.Name,
									funcDefNode.Args,
									funcDefNode.Children
								));
							}
							if (node is ImportNode impNode)
							{
								state.Import(impNode.Library);
							}
						}
						built = true;
						break;
					}
				}
				if (!built) // Unknown Expression
				{
					throw new InterpreterException($"Unknown expression at {exprPos}");
				}
			}

			return root;
		}

		private Queue<Token<TokenType>> GetBlock(ref Queue<Token<TokenType>> tokens)
		{
			TokenList block = new TokenList();
			LexerPosition start = tokens.Peek().Position;
			int depth = 1;
			while (tokens.Count > 0)
			{
				Token<TokenType> token = tokens.Dequeue();
				block.Add(token);
				if (token.TokenID == LBRACE)
				{
					depth++;
				}
				if (token.TokenID == RBRACE)
				{
					depth--;
					if (depth == 0)
					{
						return new Queue<Token<TokenType>>(block);
					}
				}
			}
			throw new InterpreterException($"Block at {start} doesn't end!");
		}

		internal UserFunction[] GetFuncsFromCode(string code, ref InterpreterState state)
		{
			// Convert code to tokens
			LexerResult<TokenType> result = lexer.Tokenize(code);
			// Error out if lexer fails
			if (result.IsError)
			{
				throw new InterpreterException($"Parsing Error: {result.Error}");
			}

			Queue<Token<TokenType>> tokens = new Queue<Token<TokenType>>(result.Tokens);

			RootNode node = BuildTree<RootNode>(tokens, ref state, true);
			List<FunctionDefinitionNode> funcs = new List<FunctionDefinitionNode>();

			GetFuncsFromTree(node, ref funcs);

			return funcs.Select(f => new UserFunction(f.Name, f.Args, f.Children)).ToArray();
		}

		private void GetFuncsFromTree(BlockNode tree, ref List<FunctionDefinitionNode> funcs)
		{
			foreach (Node child in tree.Children)
			{
				if (child is FunctionDefinitionNode funcDef)
				{
					funcs.Add(funcDef);
				}
				else if (child is BlockNode block)
				{
					GetFuncsFromTree(block, ref funcs);
				}
			}
		}

		internal static List<TokenList> GetArgs(TokenList expr)
		{
			int start = expr.Select(t => t.TokenID)
				.ToList()
				.IndexOf(LPAREN) + 1;
			return expr.GetRange(
					start,
					expr.Select(t => t.TokenID)
						.ToList()
						.LastIndexOf(RPAREN) - start)
				.Split(COMMA);
		}
	}
}