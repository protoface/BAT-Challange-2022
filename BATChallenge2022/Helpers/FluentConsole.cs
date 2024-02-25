﻿namespace BATChallenge2022.Helpers;
internal class FluentConsole
{
	public FluentConsole Write(string value)
	{
		Console.Write(value);
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
	public FluentConsole Position(int left, int top) => Position(new(left, top));
	public FluentConsole Position(Position position)
	{
		Console.SetCursorPosition(position.Left, position.Top);
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

	public ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => ForeColor(value); }
	public ConsoleColor BackgroundColor { get => Console.BackgroundColor; set => BackColor(value); }
	public int BufferWidth => Console.BufferWidth;
	public int BufferHeight => Console.BufferHeight;
}
