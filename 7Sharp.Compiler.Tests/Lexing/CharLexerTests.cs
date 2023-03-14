using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public sealed class CharLexerTests
{
	[Theory]
	[TestCase(@"' '", new[] { TokenType.CHAR })]
	[TestCase(@"'\0'", new[] { TokenType.CHAR })]
	[TestCase(@"'\n'", new[] { TokenType.CHAR })]
	[TestCase(@"'\r'", new[] { TokenType.CHAR })]
	[TestCase(@"'\t'", new[] { TokenType.CHAR })]
	[TestCase(@"'\\'", new[] { TokenType.CHAR })]
	[TestCase(@"'\u0123'", new[] { TokenType.CHAR })]
	[TestCase(@"'\u4567'", new[] { TokenType.CHAR })]
	[TestCase(@"'\u89AB'", new[] { TokenType.CHAR })]
	[TestCase(@"'\uDEFG'", new[] { TokenType.CHAR })]
	public void Valid(string s, TokenType[] tokens) => LexingTester.ExpectTokens(s, tokens);

	[Theory]
	[TestCase(@"'", LexerErrorType.UNCLOSED_CHAR)]
	[TestCase(@"''", LexerErrorType.INVALID_CHAR_LITERAL)]
	public void Invalid(string s, LexerErrorType? error) => LexingTester.ExpectError(s, error);


	[Theory]
	[TestCaseSource(nameof(GetInvalidEscapeSequences))]
	public void EscapeSequences(char c) => LexingTester.ExpectError($"'\\{c}'", LexerErrorType.INVALID_ESCAPE_SEQUENCE);

	internal static IEnumerable<object[]> GetInvalidEscapeSequences()
	{
		List<object[]> list = new();
		for (int i = ' '; i <= '~'; i++)
		{
			char c = (char)i;
			if (i is not ('0' or 'n' or 'r' or 't' or 'u' or '\\' or '\'' or '"'))
			{
				list.Add(new object[] { c });
			}
		}
		return list;
	}
}