namespace _7Sharp.Compiler;

public readonly record struct DelimeterMatch(DelimeterMatchResult Result, int Index)
{
	public static implicit operator bool(DelimeterMatch value) => value.Result == DelimeterMatchResult.FOUND;
	public static implicit operator DelimeterMatch(int i) => new(DelimeterMatchResult.FOUND, i);
}
