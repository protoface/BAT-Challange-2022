using BATChallenge2022.Enums;
using BATChallenge2022.Helpers;
using BATChallenge2022.Interfaces;
using BATChallenge2022.Models;
using BATChallenge2022.Models.Player;
using System.Diagnostics;

internal class Program
{
	internal static FluentConsole Console = new();

	static Func<IItem>[] ItemRegistry = [() => new MoneyItem(), () => new HungerItem()];
	static Func<Stats, string>[] StatRegistry = [(stats) => $"Money: {stats.Money} $", (stats) => $"Hunger: {stats.Hunger}"];

	private static void Main(string[] args)
	{
		IPlayer player = args.Contains("--snake") ? new SnakePlayer() : new BasicPlayer();
		Stats stats = new();
		Position playerPosition = new(Console.Width / 2, Console.Height / 2), currentBiome = new();
		ConsoleColor playerColor = GetRandomColor();

		Biome biome = ShowBiome(currentBiome, stats);

		DrawStats(currentBiome, stats, biome);

		DeathReasons? deathReason = null;

		while (deathReason == null)
		{
			player.Move(stats, ref playerPosition, ref currentBiome, playerColor, ref biome, ref deathReason);

			HandleStatChanges(stats);

			TryEatItem(stats, playerPosition, biome);

			DrawStats(currentBiome, stats, biome);

			deathReason = stats switch
			{
				{ Hunger: <= 0 } => DeathReasons.Starved,
				{ Hunger: >= 1000 } => DeathReasons.Diabetes,
				{ Money: <= -200 } => DeathReasons.Mafia,
				{ Money: >= 5000 } => DeathReasons.Money,
				_ => deathReason
			};
		}

		Console.ForeColor(biome.Color);

		DrawDeathAnimation();
		if (ShowDeathScreen(stats, deathReason))
			Main(args);
		else
			Environment.Exit(0);
	}

	internal static bool TryEatItem(Stats stats, Position playerPosition, Biome biome)
	{
		if (biome.Items.TryGetValue(playerPosition, out IItem? item))
		{
			item.Collected(stats);
			biome.Items.Remove(playerPosition);
			return true;
		}
		return false;
	}

	private static void HandleStatChanges(Stats stats)
	{
		stats.Hunger--;

		if (Random.Shared.Next(61) == 60)
			stats.Money -= Random.Shared.Next(10, 100);

		stats.BlocksTravelled++;
	}

	private static ConsoleColor GetRandomColor() => (ConsoleColor)Random.Shared.Next(1, 15);

	private static void DrawDeathAnimation()
	{
		try
		{
			int topLeft = 1, topRight = 0, bottomLeft = Console.Height - 1, bottomRight = Console.Width - 1;
			int direction = 0;
			Position pos = new();
			long ticksPerChar = TimeSpan.FromSeconds(3).Ticks / (Console.Height * Console.Width);
			while (true)
			{
				var timestamp = Stopwatch.GetTimestamp();
				if (pos.Left >= Console.Width || pos.Top >= Console.Height)
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
		Position center = new(Console.Width / 2, Console.Height / 2);
		Console
			.Position(center.Transform(-deathMessage.Length / 2, 0)).Write(deathMessage)
			.Position(center.Transform(-restartMessage.Length / 2, 1)).Write(restartMessage)
			.Position(center.Left - blocksTravelledMessage.Length / 2, Console.Height - 2).Write(blocksTravelledMessage)
			.Position(center.Left - biomesDiscoveredMessage.Length / 2, Console.Height - 1).Write(biomesDiscoveredMessage)
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
		DeathReasons.Mafia => "Die Mafia hat dich wegen deinen Schulden heimgesucht.",
		DeathReasons.Diabetes => "Du bist an Zucker gestorben.",
		DeathReasons.Starved => "Du bist verhungert.",
		DeathReasons.Self => "Du bist an dir selbst gestorben",
		_ => "Erstaunlich! Du bist dem Tode zu Opfer gefallen."
	};

	static private void DrawStats(Position biomePosition, Stats stats, Biome biome)
	{
		string[] statStrings = StatRegistry.Select(e => e(stats)).ToArray();
		var length = statStrings.Max(e => e.Length) + 2;

		int top = Console.Height - (2 + statStrings.Length);

		if (length < stats.LastStatWidth)
		{
			Console.ForeColor(biome.Color);
			for (int x = length + 2; x < stats.LastStatWidth + 2; x++)
			{
				for (int y = top; y < Console.Height; y++)
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

		stats.LastStatWidth = length;
	}

	internal static Biome GetBiome(Position pos, Stats stats)
	{
		if (!stats.Biomes.ContainsKey(pos))
		{
			Dictionary<Position, IItem> Items = new();
			int count = Random.Shared.Next(3, 10);
			for (; count > 0; count--)
			{
				if (!Items.TryAdd(new Position(Random.Shared.Next(Console.Width - 1), Random.Shared.Next(Console.Height - 1)), ItemRegistry[Random.Shared.Next(ItemRegistry.Length)]()))
					count++;
			}
			stats.Biomes.Add(pos, new(GetRandomColor(), Items));
		}

		return stats.Biomes[pos];
	}

	internal static Biome ShowBiome(Position pos, Stats stats)
	{
		Biome biome = GetBiome(pos, stats);
		Console.Fill(biome.Color);
		foreach (var item in biome.Items)
		{
			Console.Position(item.Key).ForeColor(item.Value.Color).Write(item.Value.Character);
		}
		return biome;
	}
}