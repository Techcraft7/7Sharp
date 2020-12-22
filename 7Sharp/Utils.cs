using _7Sharp.Intrerpreter;
using sly.lexer;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Techcraft7_DLL_Pack.Text;
using TokenList = System.Collections.Generic.List<sly.lexer.Token<_7Sharp.Intrerpreter.TokenType>>;

namespace _7Sharp
{
	internal static class Utils
	{
		private static readonly Dictionary<Type, string> SIMPLE_TYPE_NAMES = new Dictionary<Type, string>()
		{
			{ typeof(byte), "byte" },
			{ typeof(sbyte), "sbyte" },
			{ typeof(short), "short" },
			{ typeof(ushort), "ushort" },
			{ typeof(int), "int" },
			{ typeof(uint), "uint" },
			{ typeof(long), "long" },
			{ typeof(ulong), "ulong" },
			{ typeof(string), "string" },
			{ typeof(object), "object" }
		};

		//Converts "SomePascalCaseString" -> "Some Pascal Case String"
		internal static string FormatPascalString(string v)
		{

			string output = string.Empty;
			for (int i = 0; i < v.Length; i++)
			{
				//if character is uppercase and not the first character
				if (i > 0 && v[i].ToString() == v[i].ToString().ToUpper())
				{
					output += " ";
				}
				output += v[i];
			}
			return output;
		}

		internal static void PrintError(Exception e)
		{
			if (e is InterpreterException)
			{
				ColorConsoleMethods.WriteLineColor($"{e.Message}", ConsoleColor.Red);
				return;
			}
			ColorConsoleMethods.WriteLineColor($"{e.GetType()}: {e.Message}\n{e.StackTrace}", ConsoleColor.Red);
		}

		public static bool IsEnding(this Token<TokenType> t) => t.TokenID == TokenType.SEMICOLON || t.TokenID == TokenType.LBRACE;

		public static void TokenDump(this TokenList tokens)
		{
			Console.WriteLine("TOKEN DUMP {");
			foreach (Token<TokenType> tok in tokens)
			{
				Console.WriteLine("\t" + tok.ToString());
			}
			Console.WriteLine("}");
		}

		public static LexerPosition Adjust(this LexerPosition p)
		{
			if (p is null)
			{
				throw new ArgumentNullException(nameof(p));
			}

			return new LexerPosition(p.Index, p.Line + 1, p.Column + 1);
		}

		public static List<TokenList> Split(this TokenList tokens, TokenType sep)
		{
			if (!tokens.Any(t => t.TokenID == sep))
			{
				return new List<TokenList>() { tokens };
			}
			List<TokenList> list = new List<TokenList>();
			TokenList current = new TokenList();
			int depth = 0;
			for (int i = 0; i < tokens.Count; i++)
			{
				switch (tokens[i].TokenID)
				{
					case TokenType.LPAREN:
					case TokenType.LBRACE:
					case TokenType.LBRACKET:
						depth++;
						break;
					case TokenType.RPAREN:
					case TokenType.RBRACE:
					case TokenType.RBRACKET:
						depth--;
						break;
				}
				if (depth == 0 && tokens[i].TokenID == sep)
				{
					list.Add(current.AsEnumerable().ToList());
					current.Clear();
					continue;
				}
				current.Add(tokens[i]);
			}
			list.Add(current.AsEnumerable().ToList());
			return list;
		}

		public static object[] ToArray(IEnumerable ie)
		{
			if (ie == null)
			{
				throw new ArgumentNullException(nameof(ie));
			}
			List<object> list = new List<object>();
			foreach (object obj in ie)
			{
				list.Add(obj);
			}
			return list.ToArray();
		}

		public static bool IsConstant(this Token<TokenType> t)
		{
			switch (t.TokenID)
			{
				case TokenType.STRING:
				case TokenType.INT:
				case TokenType.DOUBLE:
				case TokenType.TRUE:
				case TokenType.FALSE:
					return true;
				default:
					return false;
			}
		}

		public static object GetStringValue(this Token<TokenType> t)
		{
			if (t == null || !t.IsConstant())
			{
				return t?.StringWithoutQuotes;
			}
			switch (t.TokenID)
			{
				case TokenType.STRING:
					return t.Value;
				case TokenType.INT:
					return t.IntValue;
				case TokenType.DOUBLE:
					return t.DoubleValue;
				default:
					return null;
			}
		}
		public static string AsString(this TokenList list)
		{
			string s = "";
			for (int i = 0; i < list.Count; i++)
			{
				s += list[i].GetStringValue();
			}
			return s;
		}

		public static void ThrowIfNotSize<T>(this IEnumerable<T> ie, LexerPosition pos, params int[] sizes)
		{
			if (!sizes.Contains(ie.Count()))
			{
				throw new InterpreterException($"Too many arguments at {pos}");
			}
		}

		public static string GetSimpleName(this Type t)
		{
			if (SIMPLE_TYPE_NAMES.ContainsKey(t))
			{
				return SIMPLE_TYPE_NAMES[t];
			}
			return t.Name;
		}

		public static Dictionary<K, V> Clone<K, V>(this Dictionary<K, V> dict) => dict.AsEnumerable().ToDictionary(kv => kv.Key, kv => kv.Value);
	}
}
