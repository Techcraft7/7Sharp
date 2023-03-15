using System.Data;
using System.Text;

namespace _7Sharp.Compiler.Lexing;

public static class Lexer
{
	private static readonly string IDENTIFIER_START_CHARS = "_" +
		new string(Enumerable.Range(0, 26).Select(i => (char)(i + 'A')).ToArray()) +
		new string(Enumerable.Range(0, 26).Select(i => (char)(i + 'a')).ToArray());
	private static readonly string IDENTIFIER_END_CHARS = IDENTIFIER_START_CHARS + "0123456789";

	public static LexerResult Lex(string code)
	{
		TransactionalCharStream stream = new(code);

		List<Token> tokens = new();

		Span<char> threeCharBuf = stackalloc char[3];

		while (!stream.AtEnd)
		{
			char current = stream.Next();
			switch (current)
			{
				case '/' when stream.Peek() == '/':
					_ = stream.Next(); // Eat second slash
					tokens.Add(LexSingleLineComment(stream));
					stream.Commit();
					break;
				case '/' when stream.Peek() == '*':
				{
					_ = stream.Next(); // eat *
					if (!TryLexMultiLineComment(stream, out Token comment, out LexerError error))
					{
						return error;
					}
					tokens.Add(comment);
					stream.Commit();
					break;
				}
				case '\"':
				{
					if (!TryLexString(stream, out Token str, out LexerError error))
					{
						return error;
					}
					tokens.Add(str);
					stream.Commit();
					break;
				}
				case '\'':
				{
					if (!TryLexChar(stream, out Token chr, out LexerError error))
					{
						return error;
					}
					tokens.Add(chr);
					stream.Commit();
					break;
				}
				case '-' when stream.Peek() is >= '0' and <= '9':
				case >= '0' and <= '9':
				{
					if (!TryLexNumber(stream, out Token number, out LexerError error))
					{
						return error;
					}
					tokens.Add(number);
					stream.Commit();
					break;
				}
				case '_' or (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'):
				{
					if (!TryLexIdentifier(stream, out Token id, out LexerError error))
					{
						return error;
					}
					tokens.Add(id);
					stream.Commit();
					break;
				}
				case ' ' or '\t' or '\r' or '\n':
					stream.Commit();
					continue;
				default:
				{
					threeCharBuf[0] = current;
					threeCharBuf[1] = stream.Peek() ?? '\0';
					threeCharBuf[2] = stream.Peek(2) ?? '\0';
					(int count, TokenType type) = threeCharBuf switch
					{
						// 3 chars
						['!', '>', '>'] => (3, TokenType.ERROR_MAP),
						['!', '?', '?'] => (3, TokenType.ERROR_DEFAULT),
						// 2 chars
						['?', '?', _] => (2, TokenType.DEFAULT),
						['?', '.', _] => (2, TokenType.VALUE_CHAIN),
						['!', '.', _] => (2, TokenType.ERROR_CHAIN),
						['!', '=', _] => (2, TokenType.NOT_EQUALS),
						['=', '=', _] => (2, TokenType.EQUALS),
						['>', '=', _] => (2, TokenType.GREATER_THAN_OR_EQUAL),
						['>', '>', _] => (2, TokenType.BIT_RIGHT),
						['<', '=', _] => (2, TokenType.LESS_THAN_OR_EQUAL),
						['<', '<', _] => (2, TokenType.BIT_LEFT),
						['=', '>', _] => (2, TokenType.LAMBDA),
						['+', '+', _] => (2, TokenType.INCREMENT),
						['-', '-', _] => (2, TokenType.DECREMENT),
						['&', '&', _] => (2, TokenType.BOOL_AND),
						['|', '|', _] => (2, TokenType.BOOL_OR),
						['^', '^', _] => (2, TokenType.BOOL_XOR),
						// 1 char
						['>', _, _] => (1, TokenType.GREATER_THAN),
						['<', _, _] => (1, TokenType.LESS_THAN),
						['!', _, _] => (1, TokenType.BOOL_NOT),
						['=', _, _] => (1, TokenType.ASSIGNMENT),
						['+', _, _] => (1, TokenType.PLUS),
						['-', _, _] => (1, TokenType.MINUS),
						['&', _, _] => (1, TokenType.BIT_AND),
						['|', _, _] => (1, TokenType.BIT_OR),
						['^', _, _] => (1, TokenType.BIT_XOR),
						['~', _, _] => (1, TokenType.BIT_NOT),
						['.', _, _] => (1, TokenType.DOT),
						['*', _, _] => (1, TokenType.TIMES),
						['/', _, _] => (1, TokenType.DIVIDE),
						['%', _, _] => (1, TokenType.MOD),
						[';', _, _] => (1, TokenType.SEMICOLON),
						['(', _, _] => (1, TokenType.OPEN_PAREN),
						[')', _, _] => (1, TokenType.CLOSE_PAREN),
						['[', _, _] => (1, TokenType.OPEN_BRACKET),
						[']', _, _] => (1, TokenType.CLOSE_BRACKET),
						['{', _, _] => (1, TokenType.OPEN_BRACE),
						['}', _, _] => (1, TokenType.CLOSE_BRACE),
						_ => (0, default)
					};
					if (count == 0)
					{
						return new LexerError(LexerErrorType.INVALID_CHAR, new(), 1);
					}
					tokens.Add(new Token(type, new(threeCharBuf[0..count]), new(stream.Line, stream.Column)));
					// Eat extra chars
					for (int i = 1; i < count; i++)
					{
						_ = stream.Next();
					}
					stream.Commit();
					break;
				}
			}
		}

		return new(tokens);
	}

	private static bool TryLexIdentifier(TransactionalCharStream stream, out Token id, out LexerError error)
	{
		int startLine = stream.Line, startCol = stream.Column; // pos of first char
		id = default;
		error = default;
		// Start with first char
		StringBuilder sb = new(stream.Peek(0).ToString());
		char c = stream.Peek() ?? '\0';
		while (!stream.AtEnd && c is '_' or (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or (>= '0' and <= '9'))
		{
			c = stream.Next();
			_ = sb.Append(c);
		}
		string s = sb.ToString();
		id = new(s switch
		{
			"true" => TokenType.TRUE,
			"false" => TokenType.FALSE,
			"empty" => TokenType.EMPTY,
			"if" => TokenType.IF,
			"else" => TokenType.ELSE,
			"while" => TokenType.WHILE,
			"function" => TokenType.FUNCTION,
			"_" => TokenType.CATCH_ALL,
			_ => TokenType.IDENFITIER
		}, s, new(startLine, startCol));
		return true;
	}

	private static bool TryLexString(TransactionalCharStream stream, out Token str, out LexerError error)
	{
		int startLine = stream.Line, startCol = stream.Column; // pos of first "
		error = default;
		str = default;
		StringBuilder sb = new();
		bool closed = false;
		int count = 0;
		while (!stream.AtEnd)
		{
			char c = stream.Next();
			count++;
			if (c == '"')
			{
				closed = true;
				break;
			}
			else if (c == '\\')
			{
				if (!TryLexEscapeSequence(stream, out string esc))
				{
					error = new(LexerErrorType.INVALID_ESCAPE_SEQUENCE, new(stream.Line, stream.Column), esc.Length + 1, esc);
					return false;
				}
				else
				{
					_ = sb.Append(esc);
				}
			}
			else
			{
				_ = sb.Append(c);
			}
		}
		if (!closed)
		{
			error = new(LexerErrorType.UNCLOSED_STRING, new(startLine, startCol), count + 1);
			return false;
		}
		str = new(TokenType.STRING, sb.ToString(), new(startLine, startCol));
		return true;
	}

	private static bool TryLexChar(TransactionalCharStream stream, out Token token, out LexerError error)
	{
		int startLine = stream.Line, startCol = stream.Column; // pos of first '
		token = default;
		error = default;
		if (stream.AtEnd)
		{
			error = new(LexerErrorType.UNCLOSED_CHAR, new(startLine, startCol), 1);
			return false;
		}
		char c = stream.Next();
		switch (c)
		{
			case '\'':
				error = new(LexerErrorType.INVALID_CHAR_LITERAL, new(startLine, startCol), 2);
				return false;
			case '\\':
				if (!TryLexEscapeSequence(stream, out string esc))
				{
					startCol++;
					error = new(LexerErrorType.INVALID_ESCAPE_SEQUENCE, new(startLine, startCol), stream.Column - startCol, esc);
					return false;
				}
				token = new(TokenType.CHAR, esc, new(startLine, startCol));
				break;
			default:
				token = new(TokenType.CHAR, c.ToString(), new(startLine, startCol));
				break;
		}
		if (stream.Next() != '\'')
		{
			error = new(LexerErrorType.UNCLOSED_CHAR, new(startLine, startCol), token.Value.Length + 1);
			return false;
		}
		return true;
	}

	private static bool TryLexEscapeSequence(TransactionalCharStream stream, out string esc)
	{
		int startLine = stream.Line, startCol = stream.Column; // pos of \
		esc = string.Empty;
		char c = stream.Next();
		bool r = false;
		(esc, r) = c switch
		{
			'\'' => ("'", true),
			'"' => ("\"", true),
			'n' => ("\n", true),
			't' => ("\t", true),
			'r' => ("\r", true),
			'\\' => ("\\", true),
			'0' => ("\0", true),
			_ => (string.Empty, false)
		};
		if (c == 'u')
		{
			char? n1 = stream.Peek(1);
			char? n2 = stream.Peek(2);
			char? n3 = stream.Peek(3);
			char? n4 = stream.Peek(4);
			esc = $"\\u{n1?.ToString() ?? string.Empty}{n2?.ToString() ?? string.Empty}{n3?.ToString() ?? string.Empty}{n4?.ToString() ?? string.Empty}";
			Span<char?> chars = stackalloc char?[] { n1, n2, n3, n4 };
			foreach (char? i in chars)
			{
				if (n1 is (not >= '0' or not <= '9') and (not >= 'A' or not <= 'F') and (not >= 'a' or not <= 'f'))
				{
					return false;
				}
			}
			const int OFFSET = 'A' - '9' - 1;
			n1 ??= '\0';
			n2 ??= '\0';
			n3 ??= '\0';
			n4 ??= '\0';
			int d1 = (n1.Value >= 'A' ? n1.Value - OFFSET : n1.Value) - '0';
			int d2 = (n2.Value >= 'A' ? n2.Value - OFFSET : n2.Value) - '0';
			int d3 = (n3.Value >= 'A' ? n3.Value - OFFSET : n3.Value) - '0';
			int d4 = (n4.Value >= 'A' ? n4.Value - OFFSET : n4.Value) - '0';
			int h = d4 | (d3 << 4) | (d2 << 8) | (d1 << 12);
			Span<byte> bytes = stackalloc byte[4];
			switch (h)
			{
				case <= 0x7F:
					bytes = bytes[0..1];
					bytes[0] = (byte)(h & 0x7F);
					break;
				case <= 0x7FF:
					bytes = bytes[0..2];
					bytes[0] |= 0b10000000;
					bytes[0] |= (byte)(h & 0b00111111);
					bytes[1] |= 0b11000000;
					bytes[1] |= (byte)((h >> 6) & 0b00011111);
					break;
				case <= 0xFFFF:
					bytes = bytes[0..3];
					bytes[0] |= 0b10000000;
					bytes[0] |= (byte)(h & 0b00111111);
					bytes[1] |= 0b10000000;
					bytes[1] |= (byte)((h >> 6) & 0b00111111);
					bytes[2] |= 0b11100000;
					bytes[2] |= (byte)((h >> 12) & 0b00001111);
					break;
				default:
					return false;
			}
			bytes.Reverse();
			try
			{
				esc = Encoding.UTF8.GetString(bytes);
				// eat escape sequence
				_ = stream.Next();
				_ = stream.Next();
				_ = stream.Next();
				_ = stream.Next();
				return true;
			}
			catch
			{
				return false;
			}
		}
		return r;
	}

	private static Token LexSingleLineComment(TransactionalCharStream stream)
	{
		int startLine = stream.Line, startCol = stream.Column;
		StringBuilder sb = new();
		char current = '\0';
		while (!stream.AtEnd && current is not '\n')
		{
			current = stream.Next();
			_ = sb.Append(current);
		}
		_ = sb.Remove(sb.Length - 1, 1);
		string s = sb.ToString();
		return new(TokenType.SINGLE_LINE_COMMENT, s, new(startLine, startCol));
	}

	private static bool TryLexMultiLineComment(TransactionalCharStream stream, out Token token, out LexerError error)
	{
		int startLine = stream.Line, startCol = stream.Column;
		token = default;
		error = default;
		StringBuilder sb = new();
		bool closed = false;
		while (!stream.AtEnd)
		{
			char c = stream.Next();
			char c2 = stream.Peek() ?? '\0';
			if (c == '*' && c2 == '/')
			{
				closed = true;
				break;
			}
			else
			{
				_ = sb.Append(c);
			}
		}
		if (!closed)
		{
			error = new(LexerErrorType.UNCLOSED_MULTILINE_COMMENT, new(startLine, startCol), 2);
			return false;
		}
		_ = sb.Remove(sb.Length - 1, 1); // remove * (we break when we are at * and see /)
										 // Eat second /
		_ = stream.Next();
		string s = sb.ToString();
		token = new(TokenType.MULTI_LINE_COMMENT, s, new(startLine, startCol));
		return true;
	}

	private static bool TryLexNumber(TransactionalCharStream stream, out Token number, out LexerError error)
	{
		int startLine = stream.Line, startCol = stream.Column;
		number = default;
		error = default;
		StringBuilder sb = new();
		_ = sb.Append(stream.Peek(0)!.Value);
		// Assume s32
		bool isInt = true;
		int size = 32;
		bool signed = true;
		int? dotPos = default;
		while (!stream.AtEnd)
		{
			char c = stream.Next();
			switch (c)
			{
				case >= '0' and <= '9':
					_ = sb.Append(c);
					break;
				case '.':
					isInt = false;
					if (dotPos.HasValue)
					{
						error = new(LexerErrorType.MULTIPLE_DECIMAL_POINTS, new(stream.Line, stream.Column), 1);
						return false;
					}
					dotPos = stream.Column;
					break;
				case 'u' or 's':
				{
					signed = c is 's';
					isInt = true;
					char? next1 = stream.Peek(1);
					char? next2 = stream.Peek(2);
					if (next1 is '8' && (next2 ?? '\0') is < '0' or > '9')
					{
						size = 8;
						_ = stream.Next();
					}
					else if (next1 is '1' && next2 is '6')
					{
						size = 16;
						_ = stream.Next();
						_ = stream.Next();
					}
					else if (next1 is '3' && next2 is '2')
					{
						size = 32;
						_ = stream.Next();
						_ = stream.Next();
					}
					else if (next1 is '6' && next2 is '4')
					{
						size = 64;
						_ = stream.Next();
						_ = stream.Next();
					}
					else
					{
						error = new(
							LexerErrorType.INVALID_NUMBER_SIZE,
							new(),
							(next1.HasValue ? 1 : 0) + (next2.HasValue ? 1 : 0),
							$"{c}{next1?.ToString() ?? string.Empty}{next2?.ToString() ?? string.Empty}");
						return false;
					}
					break;
				}
				case 'f':
				{
					isInt = false;
					signed = true;
					char? next1 = stream.Peek(1);
					char? next2 = stream.Peek(2);
					if (next1 is '3' && next2 is '2') { }
					else if (next1 is '6' && next2 is '4') { }
					else
					{
						error = new(
							LexerErrorType.INVALID_NUMBER_SIZE,
							new(),
							(next1.HasValue ? 1 : 0) + (next2.HasValue ? 1 : 0),
							$"{c}{next1?.ToString() ?? string.Empty}{next2?.ToString() ?? string.Empty}");
						return false;
					}
					break;
				}
				default:
					error = new(LexerErrorType.INVALID_CHAR, new(stream.Line, stream.Column), 1, c.ToString());
					return false;
			}
		}
		if (isInt && dotPos.HasValue)
		{
			error = new LexerError(LexerErrorType.INTEGER_WITH_DECIMAL_POINT, new(startLine, dotPos.Value), 1);
			return false;
		}
		_ = sb.Append(isInt ? (signed ? 's' : 'u') : 'f');
		_ = sb.Append(size);
		if (sb.Length > 0 && sb[0] == '-' && !signed)
		{
			error = new LexerError(LexerErrorType.NEGATIVE_UINT, number.Location, 1);
			return false;
		}
		number = new(isInt ? TokenType.INTEGER : TokenType.FLOAT, sb.ToString(), new(startLine, startCol));
		return true;
	}
}