while (true)
{
	WritePrompt();
	string? line = Console.ReadLine();
	if (line is null || line.StartsWith("exit"))
	{
		break;
	}
}

static void WritePrompt()
{
	Console.ForegroundColor = ConsoleColor.Green;
	Console.Write("7Sharp");
	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.Write("> ");
	Console.ForegroundColor = ConsoleColor.White;
}