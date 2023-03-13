namespace _7Sharp.Compiler.Lexing;

public sealed class LexerResult
{
	public bool IsOk { get; }
	public IReadOnlyList<Token> Tokens => IsOk ? tokens : throw new InvalidOperationException("LexerResult is not OK");
	public LexerError Error => !IsOk ? error : throw new InvalidOperationException("LexerResult is OK");

	private readonly LexerError error;
	private readonly IReadOnlyList<Token> tokens;

	public LexerResult(IReadOnlyList<Token> tokens)
	{
		this.tokens = tokens;
		IsOk = true;
	}

	public LexerResult(LexerError error)
	{
		this.error = error;
		IsOk = false;
		tokens = Array.Empty<Token>();
	}

	public static implicit operator bool(LexerResult result)
	{
		return result.IsOk;
	}

	public static implicit operator LexerResult(LexerError error) => new(error);
}
