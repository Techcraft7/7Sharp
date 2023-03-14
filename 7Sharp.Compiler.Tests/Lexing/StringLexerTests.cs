using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public sealed class StringLexerTests
{
	[Theory]
	[TestCase("\"\"", new[] { TokenType.STRING })]
	[TestCase("\"hello world\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\nworld\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\\\"world\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\\\'world\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\\nworld\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\\rworld\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\\tworld\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\\0world\"", new[] { TokenType.STRING })]
	[TestCase("\"hello\\\\world\"", new[] { TokenType.STRING })]
	public void Valid(string s, TokenType[] tokens) => LexingTester.ExpectTokens(s, tokens);

	[Theory]
	[TestCase("\"unclosed string", LexerErrorType.UNCLOSED_STRING)]
	public void Invalid(string s, LexerErrorType error) => LexingTester.ExpectError(s, error);

	[Theory]
	[TestCaseSource(typeof(CharLexerTests), nameof(CharLexerTests.GetInvalidEscapeSequences))]
	public void Escapes(char c) => LexingTester.ExpectError($"\"\\{c}\"", LexerErrorType.INVALID_ESCAPE_SEQUENCE);
}
