using System.Text;

namespace _7Sharp.Editor;

public class Editor
{
	private readonly List<StringBuilder> lines = new();
	private int cursorLine, cursorCol, hScroll, vScroll;

	public Editor(string text) => lines.AddRange(text
				.Split('\n')
				.Select(x => x.Length > 0 && x[^1] == '\r' ? x[..^1] : x)
				.Select(x =>
				{
					StringBuilder sb = new();
					_ = sb.Append(x);
					return sb;
				}));

	public string Edit()
	{
		Console.Clear();
		Render();
		Console.CancelKeyPress += OnControlC;
		bool editing = true;
		while (editing)
		{
			int numLines = lines.Count;
			(int w, int h) lastSize = (Console.WindowWidth, Console.WindowHeight);
			ConsoleKeyInfo key = Console.ReadKey(true);
			switch (key.Key)
			{
				case ConsoleKey.Backspace:
				case ConsoleKey.LeftArrow:
					cursorCol--;
					if (cursorCol < 0)
					{
						cursorCol = 0;
						if (cursorLine > 0)
						{
							cursorLine--;
							cursorCol = lines[cursorLine].Length;
							if (key.Key == ConsoleKey.Backspace)
							{
								_ = lines[cursorLine].Append(lines[cursorLine + 1]);
								lines.RemoveAt(cursorLine + 1);
							}
						}
					}
					else if (key.Key == ConsoleKey.Backspace)
					{
						_ = lines[cursorLine].Remove(lines[cursorLine].Length - 1, 1);
					}
					break;
				case ConsoleKey.RightArrow:
					cursorCol++;
					if (cursorCol > lines[cursorLine].Length)
					{
						if (cursorLine < lines.Count - 1)
						{
							cursorCol = 0;
							cursorLine++;
						}
						else
						{
							cursorCol = lines[cursorLine].Length;
						}
					}
					break;
				case ConsoleKey.Delete:
					if (cursorCol < lines[cursorLine].Length)
					{
						_ = lines[cursorLine].Remove(cursorCol, 1);
					}
					else if (cursorLine < lines.Count - 1)
					{
						_ = lines[cursorLine].Append(lines[cursorLine + 1]);
						lines.RemoveAt(cursorLine);
					}
					break;
				case ConsoleKey.Home:
					cursorCol = 0;
					break;
				case ConsoleKey.End:
					cursorCol = lines[cursorLine].Length;
					break;
				case ConsoleKey.UpArrow:
					if (cursorLine > 0)
					{
						cursorLine--;
						cursorCol = Math.Min(cursorCol, lines[cursorLine].Length);
					}
					break;
				case ConsoleKey.DownArrow:
					if (cursorLine < lines.Count - 1)
					{
						cursorLine++;
						cursorCol = Math.Min(cursorCol, lines[cursorLine].Length);
					}
					break;
				case ConsoleKey.Tab:
					_ = lines[cursorLine].Append("    ");
					cursorCol += 4;
					break;
				case ConsoleKey.Enter:
					cursorLine++;
					cursorCol = 0;
					lines.Insert(cursorLine, new());
					break;
				case ConsoleKey.S when key.Modifiers == ConsoleModifiers.Control:
					editing = false;
					break;
				default:
					if (key.KeyChar is >= ' ' and <= '~')
					{
						_ = lines[cursorLine].Insert(cursorCol, key.KeyChar);
						cursorCol++;
					}
					break;
			}
			if (lastSize != (Console.WindowWidth, Console.WindowHeight))
			{
				Console.Clear();
			}
			Scroll();
			Render(lines.Count - numLines);
		}
		Console.CancelKeyPress -= OnControlC;
		Console.Clear();
		return string.Join("\n", lines.Select(sb => sb.ToString()));
	}

	private void Render(int deltaNumLines = 0)
	{
		string emtpyLine = new(' ', Console.WindowWidth);
		// Add 1 if line gets deleted to clear the line
		for (int y = 0; y < Math.Min(Console.WindowHeight - 2, lines.Count + (deltaNumLines < 0 ? 1 : 0)); y++)
		{
			int i = y + vScroll;
			Console.SetCursorPosition(0, y);
			if (i >= 0 && i < lines.Count)
			{
				string s = lines[i].ToString();
				s = s[hScroll..];
				s = s.PadRight(Console.WindowWidth);
				Console.Write(s);
			}
			else
			{
				Console.Write(emtpyLine);
			}
		}
		Console.SetCursorPosition(0, Console.WindowHeight - 2);
		string dashLine = new('-', Console.WindowWidth);
		Console.Write(dashLine);
		Console.Write("Ctrl+S: Exit");
		Console.SetCursorPosition(cursorCol - hScroll, cursorLine - vScroll);
	}

	private void Scroll()
	{
		while (cursorLine - vScroll < 0)
		{
			vScroll--;
		}
		while (cursorLine - vScroll >= Console.WindowHeight - 2)
		{
			vScroll++;
		}
		while (cursorCol - hScroll < 0)
		{
			hScroll--;
		}
		while (cursorCol - hScroll >= Console.WindowWidth)
		{
			hScroll++;
		}
	}

	// Cancel Control+C killing program
	private static void OnControlC(object? sender, ConsoleCancelEventArgs e) => e.Cancel = true;
}
