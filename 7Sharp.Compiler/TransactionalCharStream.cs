namespace _7Sharp.Compiler;

// A TranscationalStream<char> that keeps track of lines and columns
public sealed class TransactionalCharStream
{
	public bool AtEnd => stream.AtEnd;
	public int Line { get; private set; } = 1;
	public int Column { get; private set; } = 1;

	private readonly TransactionalStream<char> stream;
	private int committedLine = 1, committedCol = 1;

	public TransactionalCharStream(string s)
	{
		stream = new(s?.ToArray()!);
	}

	public void Rollback()
	{
		stream.Rollback();
		Line = committedLine;
		Column = committedCol;
	}

	public void Commit()
	{
		stream.Commit();
		committedLine = Line;
		committedCol = Column;
	}

	public char Next()
	{
		Column++;
		char c = stream.Next();
		if (c is '\n')
		{
			Column = 1;
			Line++;
		}
		return c;
	}

	public char? PeekPrev(int count = -1) => stream.PeekPrev(count);
	public char? Peek(int count = 1) => stream.Peek(count);
}