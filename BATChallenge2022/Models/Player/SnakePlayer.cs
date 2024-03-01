using BATChallenge2022.Enums;
using BATChallenge2022.Helpers;
using BATChallenge2022.Interfaces;

namespace BATChallenge2022.Models.Player;
internal class SnakePlayer : IPlayer
{
	static FluentConsole Console => Program.Console;
	Queue<(Position player, Position biome)> playerSegments = new();
	Position direction = new();
	public SnakePlayer()
	{
		playerSegments.Enqueue((new(Console.Width / 2, Console.Height / 2), new()));
	}

	// TODO: Optimize
	public void Move(Stats stats, ref Position playerPosition, ref Position currentBiome, ConsoleColor playerColor, ref Biome biome, ref DeathReasons? deathReason)
	{
		Console.ForeColor(playerColor);
		foreach (var segment in playerSegments)
		{
			if (segment.biome == currentBiome)
				Console.Position(segment.player).Write('@');
		}
		Console.ForeColor(biome.Color);

		ConsoleKey input = Console.ReadKey();

		switch (input)
		{
			case ConsoleKey.UpArrow or ConsoleKey.W:
				if (direction.Top == 1)
					return;
				if (playerPosition.Top > 0)
					playerPosition.Top--;
				else
				{
					playerPosition.Top = Console.Height - 1;
					currentBiome.Top--;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				direction = new(0, -1);
				break;
			case ConsoleKey.DownArrow or ConsoleKey.S:
				if (direction.Top == -1)
					return;
				if (playerPosition.Top < Console.Height - 1)
					playerPosition.Top++;
				else
				{
					playerPosition.Top = 0;
					currentBiome.Top++;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				direction = new(0, 1);
				break;
			case ConsoleKey.LeftArrow or ConsoleKey.A:
				if (direction.Left == 1)
					return;
				if (playerPosition.Left > 0)
					playerPosition.Left--;
				else
				{
					playerPosition.Left = Console.Width - 1;
					currentBiome.Left--;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				direction = new(-1, 0);
				break;
			case ConsoleKey.RightArrow or ConsoleKey.D:
				if (direction.Left == -1)
					return;
				if (playerPosition.Left < Console.Width - 1)
					playerPosition.Left++;
				else
				{
					playerPosition.Left = 0;
					currentBiome.Left++;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				direction = new(1, 0);
				break;
			default:
				return;
		}
		playerSegments.Enqueue((playerPosition, currentBiome));

		if (CheckCollisions())
		{
			deathReason = DeathReasons.Self;
			return;
		}

		if (!Program.TryEatItem(stats, playerPosition, biome))
			Console.Position(playerSegments.Dequeue().player).Write('\u2588');
	}

	private bool CheckCollisions()
	{
		var head = playerSegments.Last();
		return playerSegments.SkipLast(1).Any(e => e == head);
	}
}
