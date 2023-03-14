using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public sealed class IdentifierLexerTests
{
	[Theory]
	[TestCase("abcdefghijklmnopqrstuvwxyz_ABCDEFGHIJKLMNOPQRSTUVWXYZ", new[] { TokenType.IDENFITIER })]
	[TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz", new[] { TokenType.IDENFITIER })]
	[TestCase("_startsWithUnderscores", new[] { TokenType.IDENFITIER })]
	[TestCase("__", new[] { TokenType.IDENFITIER })]
	[TestCase("numbersAFTER_0123456789", new[] { TokenType.IDENFITIER })]
	public void Valid(string s, TokenType[] tokens) => LexingTester.ExpectTokens(s, tokens);

	[Theory]
	[TestCase("0starts_WITH_number", null)]
	[TestCase("1starts_WITH_number", null)]
	[TestCase("2starts_WITH_number", null)]
	[TestCase("3starts_WITH_number", null)]
	[TestCase("4starts_WITH_number", null)]
	[TestCase("5starts_WITH_number", null)]
	[TestCase("6starts_WITH_number", null)]
	[TestCase("7starts_WITH_number", null)]
	[TestCase("8starts_WITH_number", null)]
	[TestCase("9starts_WITH_number", null)]
	public void Invalid(string s, LexerErrorType? error) => LexingTester.ExpectError(s, error);
}

//public sealed class LexerTests
//{
//	[Theory]
//	[TestCase("", new[] { })]
//	public void Valid(string s, TokenType[] tokens) => LexingTester.Valid(s, tokens);

//	[Theory]
//	[TestCase("", LexerErrorType.)]
//	public void Invalid(string s, LexerErrorType error) => LexingTester.Invalid(s, error);
//}