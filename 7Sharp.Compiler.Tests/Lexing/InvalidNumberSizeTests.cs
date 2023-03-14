using _7Sharp.Compiler.Lexing;

namespace _7Sharp.Compiler.Tests.Lexing;

public sealed class InvalidNumberSizeTests
{
	[Test]
	public void InvalidInt8Size([Range(0, 8 - 1)] int size, [Range(0, 1)] int isSigned)
	{
		LexingTester.ExpectError($"123{(isSigned == 1 ? 's' : 'u')}{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}

	[Test]
	public void InvalidInt16Size([Range(8 + 1, 16 - 1)] int size, [Range(0, 1)] int isSigned)
	{
		LexingTester.ExpectError($"123{(isSigned == 1 ? 's' : 'u')}{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}

	[Test]
	public void InvalidInt32Size([Range(16 + 1, 32 - 1)] int size, [Range(0, 1)] int isSigned)
	{
		LexingTester.ExpectError($"123{(isSigned == 1 ? 's' : 'u')}{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}

	[Test]
	public void InvalidInt64Size([Range(32 + 1, 64 - 1)] int size, [Range(0, 1)] int isSigned)
	{
		LexingTester.ExpectError($"123{(isSigned == 1 ? 's' : 'u')}{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}

	[Test]
	public void InvalidIntOtherSize([Range(64 + 1, 100)] int size, [Range(0, 1)] int isSigned)
	{
		LexingTester.ExpectError($"123{(isSigned == 1 ? 's' : 'u')}{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}

	[Test]
	public void InvalidFloat32Size([Range(0, 32 - 1)] int size)
	{
		LexingTester.ExpectError($"123.456f{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}

	[Test]
	public void InvalidFloat64Size([Range(32 + 1, 64 - 1)] int size)
	{
		LexingTester.ExpectError($"123.456f{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}

	[Test]
	public void InvalidFloatOtherSize([Range(64 + 1, 100)] int size)
	{
		LexingTester.ExpectError($"123.456f{size}", LexerErrorType.INVALID_NUMBER_SIZE);
	}
}