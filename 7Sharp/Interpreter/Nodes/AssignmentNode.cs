using sly.lexer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Sharp.Interpreter.Nodes
{
	internal class AssignmentNode : Node
	{
		private readonly string name;
		private readonly List<Token<TokenType>> valueTokens;
		private readonly bool isArrayAssignment;
		private readonly bool isIndexer;
		private readonly List<Token<TokenType>> indexTokens;

		public AssignmentNode(string name, List<Token<TokenType>> valueTokens, bool isArrayAssignment, bool isIndexer, List<Token<TokenType>> indexTokens, LexerPosition linePosition) : base(linePosition)
		{
			this.name = name;
			this.valueTokens = valueTokens;
			this.isArrayAssignment = isArrayAssignment;
			this.isIndexer = isIndexer;
			this.indexTokens = indexTokens;
		}

		public override void Run(ref InterpreterState state)
		{
			InterpreterState iState = state;
			state.RunWithVariables((ref Dictionary<string, object> vars) =>
			{
				object value = null;
				if (isArrayAssignment)
				{
					value = valueTokens
						.Skip(1).Reverse()                          // Remove [
						.Skip(1).Reverse()                          // Remove ]
						.ToList().Split(TokenType.COMMA)            // Split by ,
						.Select(list => list.AsString()).ToList()   // Convert to strings
						.Select(str => iState.Evaluate(str))        // Evaluate
						.ToArray();
				}
				else
				{
					value = iState.Evaluate(valueTokens);
				}
				if (isIndexer)
				{
					if (!iState.Variables.Peek().ContainsKey(name))
					{
						throw new InterpreterException($"Cannot assign to value in array \"{name}\" because it does not exist!");
					}
					object obj = iState.Variables.Peek()[name];
					int index = iState.TryParse<int>(indexTokens, $"Index to assign to array \"{name}\" must be an integer!");
					if (obj is object[] array)
					{
						array[index] = value;
						iState.Variables.Peek()[name] = array;
					}
					else if (obj is string s)
					{
						char[] arr = s.ToCharArray();
						if (value is char c)
						{
							arr[index] = c;
						}
						else if (value is string s2)
						{
							if (s2.Length != 1)
							{
								throw new InterpreterException("Attempted to replace a character in a string with a string that was not one character long!");
							}
							arr[index] = s2[0];
						}
						else
						{
							throw new InterpreterException("Attempted to replace a character in a string with a non-character!");
						}
						vars[name] = new string(arr);
					}
					else
					{
						throw new InterpreterException($"Variable \"{name}\" is not an array or string!");
					}
				}
				else
				{
					// Update value if it exists
					if (vars.ContainsKey(name))
					{
						vars[name] = value;
						return;
					}
					// Otherwise add it
					vars.Add(name, value);
				}
			});
			state = iState;
		}

		public static bool IsAssignment(List<Token<TokenType>> tokens, out bool isIndexer)
		{
			isIndexer = false;
			if (tokens.Count <= 3)
			{
				return false;
			}
			bool result = tokens[0].TokenID == TokenType.IDENTIFIER;
			result &= tokens[tokens.Count - 1].TokenID == TokenType.SEMICOLON;
			switch (tokens[1].TokenID)
			{
				case TokenType.ASSIGNMENT:
					return result;
				case TokenType.LBRACKET:
					isIndexer = true;
					int end = -1;
					int depth = 0;
					for (int i = 1; i < tokens.Count; i++)
					{
						switch (tokens[i].TokenID)
						{
							case TokenType.LBRACKET:
								depth++;
								break;
							case TokenType.RBRACKET:
								depth--;
								if (depth < 0)
								{
									throw new InterpreterException("']' is missing a '['");
								}
								if (depth == 0)
								{
									end = i;
									i = tokens.Count;
								}
								break;
						}
					}
					if (end < 0)
					{
						throw new InterpreterException("'[' is missing a ']'");
					}
					result &= tokens[end].TokenID == TokenType.RBRACKET;
					result &= tokens[end + 1].TokenID == TokenType.ASSIGNMENT;
					break;
				default:
					return false;
			}
			return result;
		}

		public override string ToString() => $"Assignment {{ {name} = {valueTokens.AsString()}, isArrayAssignment = {isArrayAssignment} }}";

		public static bool IsArrayAssignment(List<Token<TokenType>> tokens, out bool isIndexer)
		{
			if (!IsAssignment(tokens, out isIndexer))
			{
				return false;
			}
			//x = [ ... ];
			return tokens[2].TokenID == TokenType.LBRACKET &&
				tokens[tokens.Count - 2].TokenID == TokenType.RBRACKET;
		}

		public static List<Token<TokenType>> GetIndex(List<Token<TokenType>> expr)
		{
			int start = 0;
			int length = -1;
			int depth = 0;
			for (int i = 1; i < expr.Count; i++)
			{
				switch (expr[i].TokenID)
				{
					case TokenType.LBRACKET:
						start = i + 1;
						depth++;
						break;
					case TokenType.RBRACKET:
						depth--;
						if (depth < 0)
						{
							throw new InterpreterException("']' is missing a '['");
						}
						if (depth == 0)
						{
							i = expr.Count;
							continue;
						}
						break;
				}
				length++;
			}
			if (length < 0)
			{
				throw new InterpreterException("'[' is missing a ']'");
			}
			return expr.GetRange(start, length);
		}

		public static List<Token<TokenType>> GetValueOfIndexerAssignment(List<Token<TokenType>> expr)
		{
			int end = -1;
			int depth = 0;
			for (int i = 1; i < expr.Count; i++)
			{
				switch (expr[i].TokenID)
				{
					case TokenType.LBRACKET:
						depth++;
						break;
					case TokenType.RBRACKET:
						depth--;
						if (depth < 0)
						{
							throw new InterpreterException("']' is missing a '['");
						}
						if (depth == 0)
						{
							end = i;
							i = expr.Count;
						}
						break;
				}
			}
			if (end < 0)
			{
				throw new InterpreterException("'[' is missing a ']'");
			}
			//           end
			//            v
			// blah [ ... ] = ... ;
			return expr.Skip(end + 2).Reverse().Skip(1).Reverse().ToList();
		}
	}
}
