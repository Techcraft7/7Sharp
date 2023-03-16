using _7Sharp.Editor;
using System.Text;

string editorText = string.Empty;

while (true)
{
	WriteColor("7", ConsoleColor.Yellow);
	WriteColor("Sharp", ConsoleColor.Green);
	WriteColor("> ", ConsoleColor.Cyan);
	string? line = Console.ReadLine();
	if (line is null)
	{
		break;
	}
	string[] split = Split(line);
	if (split.Length < 1)
	{
		continue;
	}
    switch (split[0].ToLower())
	{
		case "edit":
		case "editor":
			editorText = new Editor(editorText).Edit();
			break;
		case "exit":
			return;
		case "help":
			Console.WriteLine("help - show this message");
			Console.WriteLine("exit - exit the CLI");
			break;
		default:
			WriteLineColor("Invalid command", ConsoleColor.Red);
			break;
	}
}

static string[] Split(string s)
{
	List<string> result = new();
	StringBuilder sb = new();
	Queue<char> chars = new(s);
	bool inString = false;
	while (chars.TryDequeue(out char c))
	{
		if (inString && c == '\\' && chars.TryPeek(out char next) && next == '\"')
		{
			sb.Append('\"');
			_ = chars.Dequeue(); // Eat "
		}
		else if (c == '\"')
		{
			inString = !inString;
		}
		else if (c == ' ' && !inString && sb.Length > 0)
		{
			result.Add(sb.ToString());
			sb.Clear();
		}
		else
		{
			sb.Append(c);
		}
	}
	if (sb.Length > 0)
	{
		result.Add(sb.ToString());
	}
	return result.ToArray();
}

static void WriteLineColor(string str, ConsoleColor fg, ConsoleColor bg = ConsoleColor.Black)
{
	Console.BackgroundColor = bg;
	Console.ForegroundColor = fg;
	Console.WriteLine(str);
	Console.ResetColor();
}

static void WriteColor(string str, ConsoleColor fg, ConsoleColor bg = ConsoleColor.Black)
{
	Console.BackgroundColor = bg;
	Console.ForegroundColor = fg;
	Console.Write(str);
	Console.ResetColor();
}