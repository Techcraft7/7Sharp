using _7Sharp.Compiler.Lexing;
using static _7Sharp.Compiler.Lexing.TokenType;
using static _7Sharp.Compiler.Lexing.LexerErrorType;

namespace _7Sharp.Compiler.Tests.Lexing;

public class SingleTokenTests
{
	[Theory]
	[TestCase("++", INCREMENT)]
	[TestCase("--", DECREMENT)]
	[TestCase("+", PLUS)]
	[TestCase("-", MINUS)]
	[TestCase("*", TIMES)]
	[TestCase("/", DIVIDE)]
	[TestCase("=", ASSIGNMENT)]
	[TestCase("==", EQUALS)]
	[TestCase("!=", NOT_EQUALS)]
	[TestCase("&&", BOOL_AND)]
	[TestCase("||", BOOL_OR)]
	[TestCase("^^", BOOL_XOR)]
	[TestCase("!", BOOL_NOT)]
	[TestCase("&", BIT_AND)]
	[TestCase("|", BIT_OR)]
	[TestCase("^", BIT_XOR)]
	[TestCase("~", BIT_NOT)]
	[TestCase("<<", BIT_LEFT)]
	[TestCase(">>", BIT_RIGHT)]
	[TestCase(">", GREATER_THAN)]
	[TestCase(">=", GREATER_THAN_OR_EQUAL)]
	[TestCase("<", LESS_THAN)]
	[TestCase("<=", LESS_THAN_OR_EQUAL)]
	[TestCase("??", DEFAULT)]
	[TestCase("!??", ERROR_DEFAULT)]
	[TestCase("!>>", ERROR_MAP)]
	[TestCase("?.", VALUE_CHAIN)]
	[TestCase("!.", ERROR_CHAIN)]
	[TestCase(".", DOT)]
	[TestCase("=>", LAMBDA)]
	[TestCase("true", TRUE)]
	[TestCase("false", FALSE)]
	[TestCase("empty", EMPTY)]
	[TestCase("'\\''", CHAR)]
	[TestCase("\"Hello, World! This is \\\"Quoted\\\" Text... here are some 'single quotes too!'\"", STRING)]
	[TestCase("'\\u2764'", CHAR)]
	[TestCase("_1234", IDENFITIER)]
	[TestCase("hElLo123", IDENFITIER)]
	[TestCase("123", INTEGER)]
	[TestCase("-123s8", INTEGER)]
	[TestCase("-123s16", INTEGER)]
	[TestCase("-123s32", INTEGER)]
	[TestCase("-123s64", INTEGER)]
	[TestCase("123u8", INTEGER)]
	[TestCase("123u16", INTEGER)]
	[TestCase("123u32", INTEGER)]
	[TestCase("123u64", INTEGER)]
	[TestCase("123.4f32", FLOAT)]
	[TestCase("123.4f64", FLOAT)]
	public void Test(string s, TokenType expected)
	{
		LexerResult result = Lexer.Lex(s);

		Assert.That(result.IsOk, Is.True, () => $"Lexer Error: {result.Error}");
		Console.Write("[\n\t");
		Console.Write(string.Join(",\n\t", result.Tokens));
		Console.WriteLine("\n]");
		Assert.That(result.Tokens[0].Type, Is.EqualTo(expected));
	}

	[Theory]
	[TestCase("-1u8", NEGATIVE_UINT)]
	[TestCase("-1u16", NEGATIVE_UINT)]
	[TestCase("-1u32", NEGATIVE_UINT)]
	[TestCase("-1u64", NEGATIVE_UINT)]
	[TestCase("1.0s8", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.0s16", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.0s32", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.0s64", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.0u8", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.0u16", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.0u32", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.0u64", INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("1.2.3f32", MULTIPLE_DECIMAL_POINTS)]
	[TestCase("4.5.6f64", MULTIPLE_DECIMAL_POINTS)]
	public void Invalid(string s, LexerErrorType expected)
	{
		LexerResult result = Lexer.Lex(s);

		Assert.That(result.IsOk, Is.False, () => $"Lexing Success: {string.Join(", ", result.Tokens)}");
		Assert.That(result.Error.Type, Is.EqualTo(expected));
	}
}
