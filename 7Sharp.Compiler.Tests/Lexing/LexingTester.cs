using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public static class LexingTester
{
	public static void ExpectTokens(string s, TokenType[] expected)
	{
		LexerResult result = Lexer.Lex(s);

		Assert.That(result.IsOk, Is.True, () => $"Lexer Error: {result.Error}");
		Console.Write("[\n\t");
		Console.Write(string.Join(",\n\t", result.Tokens));
		Console.WriteLine("\n]");
		Assert.That(result.Tokens, Has.Count.EqualTo(expected.Length));
		Assert.That(result.Tokens.Select(x => x.Type).ToArray(), Is.EqualTo(expected));
	}

	public static void ExpectError(string s, LexerErrorType? expected)
	{
		LexerResult result = Lexer.Lex(s);

		Assert.That(result.IsOk, Is.False, () => $"Lexing Success: {string.Join(", ", result.Tokens)}");
		Console.WriteLine(result.Error);
		if (expected is not null)
		{
			Assert.That(result.Error.Type, Is.EqualTo(expected));
		}
	}
}

// Template
//public sealed class LexerTests
//{
//	[Theory]
//	[TestCase("", new[] { })]
//	public void Valid(string s, TokenType[] tokens) => LexingTester.Valid(s, tokens);

//	[Theory]
//	[TestCase("", LexerErrorType.)]
//	public void Invalid(string s, LexerErrorType error) => LexingTester.Invalid(s, error);
//}
