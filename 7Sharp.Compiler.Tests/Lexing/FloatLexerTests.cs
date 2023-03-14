using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public sealed class FloatLexerTests
{
	[Theory]
	[TestCase("0.f32", new[] { TokenType.FLOAT })]
	[TestCase("-0.f32", new[] { TokenType.FLOAT })]
	[TestCase("0123456789f32", new[] { TokenType.FLOAT })]
	[TestCase("1234567890f32", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789f32", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890f32", new[] { TokenType.FLOAT })]
	[TestCase("0123456789.f32", new[] { TokenType.FLOAT })]
	[TestCase("1234567890.f32", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789.f32", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890.f32", new[] { TokenType.FLOAT })]
	[TestCase("0123456789.0123456789f32", new[] { TokenType.FLOAT })]
	[TestCase("1234567890.0123456789f32", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789.0123456789f32", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890.0123456789f32", new[] { TokenType.FLOAT })]
	[TestCase("0123456789.1234567890f32", new[] { TokenType.FLOAT })]
	[TestCase("1234567890.1234567890f32", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789.1234567890f32", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890.1234567890f32", new[] { TokenType.FLOAT })]
	[TestCase("0.f64", new[] { TokenType.FLOAT })]
	[TestCase("-0.f64", new[] { TokenType.FLOAT })]
	[TestCase("0123456789f64", new[] { TokenType.FLOAT })]
	[TestCase("1234567890f64", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789f64", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890f64", new[] { TokenType.FLOAT })]
	[TestCase("0123456789.f64", new[] { TokenType.FLOAT })]
	[TestCase("1234567890.f64", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789.f64", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890.f64", new[] { TokenType.FLOAT })]
	[TestCase("0123456789.0123456789f64", new[] { TokenType.FLOAT })]
	[TestCase("1234567890.0123456789f64", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789.0123456789f64", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890.0123456789f64", new[] { TokenType.FLOAT })]
	[TestCase("0123456789.1234567890f64", new[] { TokenType.FLOAT })]
	[TestCase("1234567890.1234567890f64", new[] { TokenType.FLOAT })]
	[TestCase("-0123456789.1234567890f64", new[] { TokenType.FLOAT })]
	[TestCase("-1234567890.1234567890f64", new[] { TokenType.FLOAT })]
	public void Valid(string s, TokenType[] tokens) => LexingTester.ExpectTokens(s, tokens);

	[Theory]
	[TestCase("1.2.3f32", LexerErrorType.MULTIPLE_DECIMAL_POINTS)]
	[TestCase("-4.5.6f32", LexerErrorType.MULTIPLE_DECIMAL_POINTS)]
	[TestCase("7.8.9f64", LexerErrorType.MULTIPLE_DECIMAL_POINTS)]
	[TestCase("-10.11.12f64", LexerErrorType.MULTIPLE_DECIMAL_POINTS)]
	public void Invalid(string s, LexerErrorType error) => LexingTester.ExpectError(s, error);
}