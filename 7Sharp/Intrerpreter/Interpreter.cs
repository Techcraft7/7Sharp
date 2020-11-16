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
	public sealed class Interpreter
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
			RootNode root = BuildTree<RootNode>(tokens, ref state);

			root.Dump();

			// Run
			root.Run(ref state);
		}

		private T BuildTree<T>(TokenList tokens, ref InterpreterState state) where T : BlockNode, new()
		{
			T root = new T(); // Create a new node

			int i = 0; // i is declared out here so we can use the helper function below
			Func<bool> inBounds = new Func<bool>(() => i < tokens.Count); // Helper function

			for (i = 0; i < tokens.Count; i++)
			{
				TokenList expr = new TokenList();
				// If we hit EOS we are done
				if (tokens[i].IsEOS)
				{
					WriteLineColor("EOS", Cyan);
					break;
				}
				// While i is in bounds of tokens AND we have not hit an "expression ending" (; or {)
				while (inBounds.Invoke() && !tokens[i].IsEnding())
				{
					expr.Add(tokens[i]);
					i++;
				}
				// Add the ending token (; or {)
				if (i < tokens.Count)
				{
					expr.Add(tokens[i]); // I will be on the ending token
				}
				// If expression starts with }, remove it
				if (expr.Count > 0 && expr[0].TokenID == RBRACE)
				{
					expr = expr.Skip(1).ToList();
					i++;
				}
				// Dont run empty expression
				if (expr.Count == 0)
				{
					continue;
				}
				expr.TokenDump();
				LexerPosition exprPos = expr[0].Position.Adjust();
				state.Location = exprPos;
				if (LoopNode.IsLoopNode(expr))
				{
					// Put it in a method because its really long :)
					BuildLoop(ref state, ref expr, ref exprPos, ref tokens, ref i, ref root);
					continue; // Start a new loop iteration
				}
				else if (IfNode.IsIf(tokens) || IfNode.IsElseIf(tokens))
				{
					bool elseIf = IfNode.IsElseIf(tokens);
					TokenList block = BlockNode.GetBlock(tokens, i, out int loc);
					i = loc; // Set i to be after the closing } so we dont evaluate the inside anyways
					List<TokenList> args = GetArgs(expr);
					if (args.Count != 1)
					{
						throw new InterpreterException($"If statement requires a condition at {exprPos}");
					}
					IfNode node = new IfNode(args[0].AsString(), elseIf, exprPos);
					BuildTree<RootNode>(block, ref state).Children.ForEach(c => node.Add(c));
					root.Add(node);
				}
				else if (ElseNode.IsElse(tokens))
				{
					ElseNode node = new ElseNode(exprPos);
					TokenList block = BlockNode.GetBlock(tokens, i, out int loc);
					BuildTree<RootNode>(block, ref state).Children.ForEach(c => node.Add(c));
					root.Add(node);
				}
				else if (AssignmentNode.IsAssignment(expr))
				{
					WriteLineColor("ASSIGNMENT", Yellow);
					// To get just <VALUE>, remove the first 2 and the last tokens
					// IDENTIFIER ASSIGNMENT <VALUE> SEMICOLON
					root.Add(new AssignmentNode(expr[0].StringWithoutQuotes, expr.Skip(2).Reverse().Skip(1).Reverse().ToList().AsString(), exprPos));
				}
				else if (FunctionCallNode.IsFunctionCall(expr))
				{
					WriteLineColor("FUNCTION CALL", Yellow);
					// Check if function exists
					_7sFunction func = FunctionCallNode.GetFunction(state, expr[0].Value);
					if (func == null)
					{
						throw new InterpreterException($"Unknown function \"{expr[0].Value}\" at {exprPos}");
					}
					root.Add(new FunctionCallNode(func, GetArgs(expr), exprPos));
				}
				else // Unknown Expression
				{
					throw new InterpreterException($"Unknown expression at {exprPos}");
				}
			}

			return root;
		}

		private void BuildLoop<T>(ref InterpreterState state, ref TokenList expr, ref LexerPosition exprPos, ref TokenList tokens, ref int i, ref T root) where T : BlockNode
		{
			WriteLineColor("LOOP", Yellow);
			// Get the stuff in the {}'s
			TokenList block = BlockNode.GetBlock(tokens, i, out int loc);
			i = loc; // Set i to be after the closing } so we dont evaluate the inside anyways
			List<TokenList> loopArgs = GetArgs(expr); // Get args
													  // Can only have 1 argument
			if (loopArgs.Count != 1)
			{
				throw new InterpreterException($"Loop has too many arguments at {exprPos}");
			}
			// Make loop node
			LoopNode loop = new LoopNode(loopArgs[0], exprPos);
			BuildTree<RootNode>(block, ref state).Children.ForEach(c => loop.Add(c));
			// Add to tree
			root.Add(loop);
		}

		private static List<TokenList> GetArgs(TokenList expr)
		{
			return expr.GetRange(expr.Select(t => t.TokenID).ToList().IndexOf(LPAREN), expr.Select(t => t.TokenID).ToList().LastIndexOf(RPAREN)).Split(COMMA);
		}		
	}
}