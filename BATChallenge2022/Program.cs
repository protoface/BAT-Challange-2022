using BATChallenge2022.Enums;
using BATChallenge2022.Helpers;
using BATChallenge2022.Interfaces;
using BATChallenge2022.Models;
using System.Diagnostics;

internal class Program
{
	private static void Main(string[] args) => new Program().Run(args);
	FluentConsole Console => new();
	Stats Stats = new();
	Position biomePosition = new();

	private void Run(string[] args)
	{
		Position playerPosition = new(Console.BufferWidth / 2, Console.BufferHeight / 2);
		ConsoleColor playerColor = ConsoleColor.Blue;
		Console.Clear();
		Biome biome = ShowBiome(biomePosition);

		DrawStats();

		DeathReasons? deathReason = null;

		while (true)
		{
			var biomeColor = Console.ForegroundColor;
			Console.Position(playerPosition).ForeColor(playerColor).Write('@');

			ConsoleKey input = Console.ReadKey();
			Console.Position(playerPosition).ForeColor(biomeColor).Write('\u2588');

			switch (input)
			{
				case ConsoleKey.UpArrow or ConsoleKey.W:
					if (playerPosition.Top > 0)
						playerPosition.Top--;
					else
					{
						playerPosition.Top = Console.BufferHeight - 1;
						biomePosition.Top--;
						biome = ShowBiome(biomePosition);
					}
					break;
				case ConsoleKey.DownArrow or ConsoleKey.S:
					if (playerPosition.Top < Console.BufferHeight - 1)
						playerPosition.Top++;
					else
					{
						playerPosition.Top = 0;
						biomePosition.Top++;
						biome = ShowBiome(biomePosition);
					}
					break;
				case ConsoleKey.LeftArrow or ConsoleKey.A:
					if (playerPosition.Left > 0)
						playerPosition.Left--;
					else
					{
						playerPosition.Left = Console.BufferWidth - 1;
						biomePosition.Left--;
						biome = ShowBiome(biomePosition);
					}
					break;
				case ConsoleKey.RightArrow or ConsoleKey.D:
					if (playerPosition.Left < Console.BufferWidth - 1)
						playerPosition.Left++;
					else
					{
						playerPosition.Left = 0;
						biomePosition.Left++;
						biome = ShowBiome(biomePosition);
					}
					break;
				default:
					continue;
			}

			Stats.Hunger--;

			var item = biome.Items.FirstOrDefault(e => e.Position.X == playerPosition.X && e.Position.Y == playerPosition.Y);
			if (item != null)
			{
				if (item is MoneyItem moneyItem)
					Stats.Money += moneyItem.Money;
				else if (item is HungerItem hungerItem)
					Stats.Hunger += hungerItem.Hunger;
				biome.Items.Remove(item);
			}

			DrawStats();

			if (Stats.Hunger <= 0)
			{
				deathReason = DeathReasons.Starved;
				break;
			}
			if (Stats.Hunger > 1000)
			{
				deathReason = DeathReasons.Diabetes;
				break;
			}
			if (Stats.Money > 5000)
			{
				deathReason = DeathReasons.Money;
				break;
			}
		}

		// Show death sequence

		int topLeft = 0, topRight = 0, bottomLeft = Console.BufferHeight - 1, bottomRight = Console.BufferWidth - 1;
		int direction = 0;
		Position pos = new();
		try
		{
			long ticksPerChar = TimeSpan.FromSeconds(3).Ticks / (Console.BufferHeight * Console.BufferWidth);
			while (true)
			{
				var timestamp = Stopwatch.GetTimestamp();
				if (topLeft == (Console.BufferWidth / 2) - 1 && pos.X == topLeft && pos.Y == (Console.BufferHeight / 2) - 1)
					break;
				Console.Position(pos).Write('\u00A0');
				switch (direction)
				{
					case 0:
						pos.Top++;
						if (pos.Top == bottomLeft + 1)
						{
							bottomLeft--;
							direction = 1;
							pos.Top--;
							pos.Left++;
						}
						break;
					case 1:
						pos.Left++;
						if (pos.Left == bottomRight + 1)
						{
							bottomRight--;
							direction = 2;
							pos.Left--;
							pos.Top--;
						}
						break;
					case 2:
						pos.Top--;
						if (pos.Top == topRight - 1)
						{
							topRight++;
							direction = 3;
							pos.Top++;
							pos.Left--;
						}
						break;
					case 3:
						pos.Left--;
						if (pos.Left == topLeft - 1)
						{
							topLeft++;
							direction = 0;
							pos.Left++;
							pos.Top++;
						}
						break;
				}
				while (timestamp + ticksPerChar > Stopwatch.GetTimestamp())
					;
			}
		}
		catch { }
		finally
		{
			string deathMessage = GetDeathMessage(deathReason);
			string restartMessage = "Restart (y/n)?";
			Console
				.Position((Console.BufferWidth - deathMessage.Length) / 2, Console.BufferHeight / 2).Write(deathMessage)
				.Position((Console.BufferWidth - restartMessage.Length) / 2, Console.BufferHeight / 2 + 1).Write(restartMessage)
				.Position(Console.BufferWidth / 2, Console.BufferHeight / 2 + 2);
			bool restart = false;
			while (true)
			{
				var key = Console.ReadKey();
				if (key == ConsoleKey.Y)
					restart = true;
				else if (key != ConsoleKey.N)
					continue;
				break;
			}
			if (restart)
			{
				Stats = new();
				Run(args);
			}
		}
	}

	string GetDeathMessage(DeathReasons? reason)
	=> reason switch
	{
		DeathReasons.Money => "Du bist in deinem Geldsee ertrunken",
		DeathReasons.Diabetes => "Du bist an Zucker gestorben.",
		DeathReasons.Starved => "Du bist verhungert.",
		_ => "Erstaunlich! Du bist dem Tode zu Opfer gefallen."
	};

	int lastStatWidth = 0;
	private void DrawStats()
	{
		int top = Console.BufferHeight - 6;
		var oldColor = Console.ForegroundColor;

		string money = $"Money: {Stats.Money} $";
		string hunger = $"Hunger: {Stats.Hunger}";
		var length = Math.Max(money.Length, hunger.Length) + 2;

		if (length < lastStatWidth)
		{
			Console.ForeColor(GetBiome(biomePosition).Color);
			for (int x = length + 2; x < lastStatWidth + 2; x++)
			{
				for (int y = top; y < Console.BufferHeight; y++)
				{
					Console.Position(x, y).Write('\u2588');
				}
			}
		}

		void EmptyLine(int y, char left, char middle, char right)
		{
			Console.Position(0, y).Write(left);
			for (int i = 0; i < length; i++)
				Console.Write(middle);
			Console.Write(right);
		}
		void DrawStat(int y, string stat) => Console.Position(0, y).Write($"║\u00A0{stat.PadRight(length - 2, '\u00A0')}\u00A0║");

		Console.ForeColor(ConsoleColor.White);

		EmptyLine(top++, '╔', '═', '╗');
		// Empty line
		EmptyLine(top++, '║', '\u00A0', '║');

		// Money Stat
		DrawStat(top++, money);

		// Hunger Stat
		DrawStat(top++, hunger);

		// Empty line
		EmptyLine(top++, '║', '\u00A0', '║');

		EmptyLine(top++, '╚', '═', '╝');

		Console.ForeColor(oldColor);

		lastStatWidth = length;
	}

	void Fill(ConsoleColor color, char filler = '\u2588')
	{
		var cursorPos = Console.ForeColor(color).Position();
		for (int x = 0; x < Console.BufferWidth; x++)
		{
			for (int y = 0; y < Console.BufferHeight; y++)
			{
				Console.Position(x, y).Write(filler);
			}
		}
		Console.Position(cursorPos);
	}
	Biome GetBiome(Position pos)
	{
		if (!Stats.Biomes.ContainsKey(pos))
			Stats.Biomes.Add(pos, new((ConsoleColor)Random.Shared.Next(1, 15), Enumerable.Range(0, Random.Shared.Next(3, 10)).Select<int, IItem>(e => Random.Shared.Next(10) >= 5 ? new MoneyItem() : new HungerItem()).ToList()));
		return Stats.Biomes[pos];
	}
	Biome ShowBiome(Position pos)
	{
		Biome biome = GetBiome(pos);
		Fill(biome.Color);
		foreach (var item in biome.Items)
		{
			Console.Position(item.Position).ForeColor(item.Color).Write(item.Character);
		}
		Console.ForegroundColor = biome.Color;
		return biome;
	}
}