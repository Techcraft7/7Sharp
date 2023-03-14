using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public sealed class IntegerLexerTests
{
	[Theory]
	[TestCase("0", new[] { TokenType.INTEGER })]
	[TestCase("0123456789", new[] { TokenType.INTEGER })]
	[TestCase("1234567890", new[] { TokenType.INTEGER })]
	[TestCase("1234567890u8", new[] { TokenType.INTEGER })]
	[TestCase("1234567890u16", new[] { TokenType.INTEGER })]
	[TestCase("1234567890u32", new[] { TokenType.INTEGER })]
	[TestCase("1234567890u64", new[] { TokenType.INTEGER })]
	[TestCase("1234567890s8", new[] { TokenType.INTEGER })]
	[TestCase("1234567890s16", new[] { TokenType.INTEGER })]
	[TestCase("1234567890s32", new[] { TokenType.INTEGER })]
	[TestCase("1234567890s64", new[] { TokenType.INTEGER })]
	[TestCase("-1234567890s8", new[] { TokenType.INTEGER })]
	[TestCase("-1234567890s16", new[] { TokenType.INTEGER })]
	[TestCase("-1234567890s32", new[] { TokenType.INTEGER })]
	[TestCase("-1234567890s64", new[] { TokenType.INTEGER })]
	public void Valid(string s, TokenType[] tokens) => LexingTester.ExpectTokens(s, tokens);

	[Theory]
	[TestCase("1.0u8", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("2.0u16", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("3.0u32", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("4.0u64", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("5.0s8", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("6.0s16", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("7.0s32", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("8.0s64", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("-9.0s8", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("-1.0s16", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("-1.1s32", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	[TestCase("-1.2s64", LexerErrorType.INTEGER_WITH_DECIMAL_POINT)]
	public void Invalid(string s, LexerErrorType error) => LexingTester.ExpectError(s, error);
}
