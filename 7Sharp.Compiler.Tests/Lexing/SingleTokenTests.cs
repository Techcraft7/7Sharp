using _7Sharp.Compiler.Lexing;
using static _7Sharp.Compiler.Lexing.TokenType;

namespace _7Sharp.Compiler.Tests.Lexing;

public class SingleTokenTests
{
	[Theory]
	[TestCase("++", INCREMENT)]
	[TestCase("--", DECREMENT)]
	[TestCase("+", PLUS)]
	[TestCase("-", MINUS)]
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
	[TestCase("=>", LAMBDA)]
	[TestCase("true", TRUE)]
	[TestCase("false", FALSE)]
	[TestCase("empty", EMPTY)]
	[TestCase("'\\''", CHAR)]
	[TestCase("\"Hello, World! This is \\\"Quoted\\\" Text... here are some 'single quotes too!'\"", STRING)]
	[TestCase("'\\u2764'", CHAR)]
	[TestCase("_1234", IDENFITIER)]
	[TestCase("hElLo123", IDENFITIER)]
	public void Test(string s, TokenType expected)
	{
		LexerResult result = Lexer.Lex(s);

		Assert.That(result.IsOk, Is.True, () => $"Lexer Error: {result.Error}");
		Console.Write("[\n\t");
		Console.Write(string.Join(",\n\t", result.Tokens));
		Console.WriteLine("\n]");
		Assert.That(result.Tokens[0].Type, Is.EqualTo(expected));
	}
}
