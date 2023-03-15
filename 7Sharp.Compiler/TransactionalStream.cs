namespace _7Sharp.Compiler;

public sealed class TransactionalStream<T>
{
	public bool AtEnd => index >= list.Count;

	private readonly IReadOnlyList<T> list;
	private int committedIndex = 0;
	private int index = 0;

	public TransactionalStream(IReadOnlyList<T> list)
	{
		ArgumentNullException.ThrowIfNull(list);
		this.list = list;
	}

	public void Rollback() => index = committedIndex;
	public void Commit() => committedIndex = index;

	public T? PeekPrev(int count = 1)
	{
		count--;
		return index >= 0 && index < list.Count - count ? list[index - count] : default;
	}

	public T? Peek(int count = 1)
	{
		count--;
		return index >= 0 && index < list.Count - count ? list[index + count] : default;
	}

	public T Next() => list[index++];
}
