using System.ComponentModel;
using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public sealed class CommentLexerTests
{
	[Theory]
	[TestCase("// hello\n// world", new[] { TokenType.SINGLE_LINE_COMMENT, TokenType.SINGLE_LINE_COMMENT })]
	[TestCase("/* multi\nline\ncomment\n\r\n\n*/", new[] { TokenType.MULTI_LINE_COMMENT })]
	public void Valid(string s, TokenType[] tokens) => LexingTester.ExpectTokens(s, tokens);

	[Theory]
	[TestCase("/* unclosed", LexerErrorType.UNCLOSED_MULTILINE_COMMENT)]
	public void Invalid(string s, LexerErrorType error) => LexingTester.ExpectError(s, error);
}
