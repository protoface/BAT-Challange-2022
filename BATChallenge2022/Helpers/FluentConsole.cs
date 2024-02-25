namespace BATChallenge2022.Helpers;
internal class FluentConsole
{
	public FluentConsole Write(string value)
	{
		Console.Out.Write(value);
		return this;
	}
	public FluentConsole Write(char value)
	{
		Console.Out.Write(value);
		return this;
	}

	public ConsoleKey ReadKey() => Console.ReadKey().Key;

	public FluentConsole BackColor(ConsoleColor color)
	{
		Console.BackgroundColor = color;
		return this;
	}

	public FluentConsole ForeColor(ConsoleColor color)
	{
		Console.ForegroundColor = color;
		return this;
	}

	public Position Position() => (Position)Console.GetCursorPosition();
	public FluentConsole Position(Position position) => Position(position.Left, position.Top);
	public FluentConsole Position(int left, int top)
	{
		Console.SetCursorPosition(left, top);
		return this;
	}

	public FluentConsole Fill(ConsoleColor color, char filler = '\u2588')
	{
		string fillerString = string.Empty.PadRight(Console.BufferWidth, filler);
		ForeColor(color);
		for (int y = 0; y < Console.BufferHeight; y++)
		{
			Position(0, y).Write(fillerString);
		}
		return this;
	}

	public int Width => Console.BufferWidth;
	public int Height => Console.BufferHeight;
}
