using BATChallenge2022.Enums;
using BATChallenge2022.Helpers;
using BATChallenge2022.Interfaces;
using BATChallenge2022.Models;
using System.Diagnostics;

internal class Program
{
	static FluentConsole Console = new();

	static Func<IItem>[] ItemRegistry = [() => new MoneyItem(), () => new HungerItem()];
	static Func<Stats, string>[] StatRegistry = [(stats) => $"Money: {stats.Money} $", (stats) => $"Hunger: {stats.Hunger}"];

	private static void Main(string[] args)
	{
		Stats stats = new();
		Position playerPosition = new(Console.BufferWidth / 2, Console.BufferHeight / 2), currentBiome = new();
		ConsoleColor playerColor = GetRandomColor();

		Biome biome = ShowBiome(currentBiome, stats);

		DrawStats(currentBiome, stats);

		DeathReasons? deathReason = null;

		while (deathReason == null)
		{
			Console.Position(playerPosition).ForeColor(playerColor).Write('@');

			ConsoleKey input = Console.ReadKey();
			Console.Position(playerPosition).ForeColor(biome.Color).Write('\u2588');

			switch (input)
			{
				case ConsoleKey.UpArrow or ConsoleKey.W:
					if (playerPosition.Top > 0)
						playerPosition.Top--;
					else
					{
						playerPosition.Top = Console.BufferHeight - 1;
						currentBiome.Top--;
						biome = ShowBiome(currentBiome, stats);
					}
					break;
				case ConsoleKey.DownArrow or ConsoleKey.S:
					if (playerPosition.Top < Console.BufferHeight - 1)
						playerPosition.Top++;
					else
					{
						playerPosition.Top = 0;
						currentBiome.Top++;
						biome = ShowBiome(currentBiome, stats);
					}
					break;
				case ConsoleKey.LeftArrow or ConsoleKey.A:
					if (playerPosition.Left > 0)
						playerPosition.Left--;
					else
					{
						playerPosition.Left = Console.BufferWidth - 1;
						currentBiome.Left--;
						biome = ShowBiome(currentBiome, stats);
					}
					break;
				case ConsoleKey.RightArrow or ConsoleKey.D:
					if (playerPosition.Left < Console.BufferWidth - 1)
						playerPosition.Left++;
					else
					{
						playerPosition.Left = 0;
						currentBiome.Left++;
						biome = ShowBiome(currentBiome, stats);
					}
					break;
				default:
					continue;
			}

			stats.Hunger--;
			stats.BlocksTravelled++;

			if (biome.Items.TryGetValue(playerPosition, out IItem? item))
			{
				item.Collected(stats);
				biome.Items.Remove(playerPosition);
			}

			DrawStats(currentBiome, stats);

			if (stats.Hunger <= 0)
			{
				deathReason = DeathReasons.Starved;
			}
			else if (stats.Hunger > 1000)
			{
				deathReason = DeathReasons.Diabetes;
			}
			else if (stats.Money > 5000)
			{
				deathReason = DeathReasons.Money;
			}
		}

		DrawDeathAnimation();
		if (ShowDeathScreen(stats, deathReason))
			Main(args);
		else
			Environment.Exit(0);
	}

	private static ConsoleColor GetRandomColor() => (ConsoleColor)Random.Shared.Next(15);

	private static void DrawDeathAnimation()
	{
		try
		{
			int topLeft = 0, topRight = 0, bottomLeft = Console.BufferHeight - 1, bottomRight = Console.BufferWidth - 1;
			int direction = 0;
			Position pos = new();
			long ticksPerChar = TimeSpan.FromSeconds(3).Ticks / (Console.BufferHeight * Console.BufferWidth);
			while (true)
			{
				var timestamp = Stopwatch.GetTimestamp();
				if (pos.Left >= Console.BufferWidth || pos.Top >= Console.BufferHeight)
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
		catch (ArgumentOutOfRangeException) { }
	}

	private static bool ShowDeathScreen(Stats stats, DeathReasons? deathReason)
	{
		string deathMessage = GetDeathMessage(deathReason),
		restartMessage = "Restart (y/n)?",
		blocksTravelledMessage = $"Felder bewegt: {stats.BlocksTravelled}",
		biomesDiscoveredMessage = $"Biome entdeckt: {stats.Biomes.Count}";
		Position center = new(Console.BufferWidth / 2, Console.BufferHeight / 2);
		Console
			.Position(center.Transform(-deathMessage.Length / 2, 0)).Write(deathMessage)
			.Position(center.Transform(-restartMessage.Length / 2, 1)).Write(restartMessage)
			.Position(center.Left - blocksTravelledMessage.Length / 2, Console.BufferHeight - 2).Write(blocksTravelledMessage)
			.Position(center.Left - biomesDiscoveredMessage.Length / 2, Console.BufferHeight - 1).Write(biomesDiscoveredMessage)
			.Position(center.Transform(0, 2));
		while (true)
		{
			switch (Console.ReadKey())
			{
				case ConsoleKey.Y:
					return true;
				case ConsoleKey.N:
					return false;
				default:
					continue;
			}
		}
	}

	static string GetDeathMessage(DeathReasons? reason)
	=> reason switch
	{
		DeathReasons.Money => "Du bist in deinem Geldsee ertrunken.",
		DeathReasons.Diabetes => "Du bist an Zucker gestorben.",
		DeathReasons.Starved => "Du bist verhungert.",
		_ => "Erstaunlich! Du bist dem Tode zu Opfer gefallen."
	};

	static private void DrawStats(Position biomePosition, Stats stats)
	{
		string[] statStrings = StatRegistry.Select(e => e(stats)).ToArray();
		var length = statStrings.Max(e => e.Length) + 2;

		int top = Console.BufferHeight - (2 + statStrings.Length);
		var oldColor = Console.ForegroundColor;

		if (length < stats.LastStatWidth)
		{
			Console.ForeColor(GetBiome(biomePosition, stats).Color);
			for (int x = length + 2; x < stats.LastStatWidth + 2; x++)
			{
				for (int y = top; y < Console.BufferHeight; y++)
				{
					Console.Position(x, y).Write('\u2588');
				}
			}
		}

		void EmptyLine(int y, char left, char middle, char right)
		=> Console.Position(0, y)
			.Write(left)
			.Write(string.Empty.PadRight(length, middle))
			.Write(right);

		Console.ForeColor(ConsoleColor.White);

		EmptyLine(top++, '╔', '═', '╗');

		foreach (string stat in statStrings)
			Console.Position(0, top++).Write($"║\u00A0{stat.PadRight(length - 2, '\u00A0')}\u00A0║");

		EmptyLine(top, '╚', '═', '╝');

		Console.ForeColor(oldColor);

		stats.LastStatWidth = length;
	}

	static Biome GetBiome(Position pos, Stats stats)
	{
		if (!stats.Biomes.ContainsKey(pos))
		{
			Dictionary<Position, IItem> Items = new();
			int count = Random.Shared.Next(3, 10);
			for (; count > 0; count--)
			{
				if (!Items.TryAdd(new Position(Random.Shared.Next(Console.BufferWidth - 1), Random.Shared.Next(Console.BufferHeight - 1)), ItemRegistry[Random.Shared.Next(ItemRegistry.Length)]()))
					count++;
			}
			stats.Biomes.Add(pos, new(GetRandomColor(), Items));
		}

		return stats.Biomes[pos];
	}

	static Biome ShowBiome(Position pos, Stats stats)
	{
		Biome biome = GetBiome(pos, stats);
		Console.Fill(biome.Color);
		foreach (var item in biome.Items)
		{
			Console.Position(item.Key).ForeColor(item.Value.Color).Write(item.Value.Character);
		}
		Console.ForegroundColor = biome.Color;
		return biome;
	}
}