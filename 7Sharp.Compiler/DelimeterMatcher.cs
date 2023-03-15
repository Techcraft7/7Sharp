namespace _7Sharp.Compiler;

public static class DelimeterMatcher
{
	public static DelimeterMatch FindMatch<T>(IReadOnlyList<T> list, int start, T opener, T closer, T? escape = default)
		where T : IEquatable<T>
	{
		int depth = 0;
		bool wasEscape = true;
		for (int i = start; i < list.Count; i++)
		{
			T t = list[i];
			if (t is null)
			{
				continue;
			}
			if (t.Equals(opener) && !wasEscape)
			{
				depth++;
			}
			else if (t.Equals(closer) && !wasEscape)
			{
				depth--;
				if (depth == 0)
				{
					return i;
				}
				else if (depth < 0)
				{
					return new(DelimeterMatchResult.TOO_MANY_CLOSERS, -1);
				}
			}
			wasEscape = t.Equals(escape);
		}
		return new(DelimeterMatchResult.TOO_MANY_OPENERS, -1);
	}
}
