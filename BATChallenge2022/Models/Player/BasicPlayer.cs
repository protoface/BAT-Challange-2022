using BATChallenge2022.Enums;
using BATChallenge2022.Helpers;
using BATChallenge2022.Interfaces;

namespace BATChallenge2022.Models.Player;
internal class BasicPlayer : IPlayer
{
	static FluentConsole Console => Program.Console;
	public void Move(Stats stats, ref Position playerPosition, ref Position currentBiome, ConsoleColor playerColor, ref Biome biome, ref DeathReasons? deathReason)
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
					playerPosition.Top = Console.Height - 1;
					currentBiome.Top--;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				break;
			case ConsoleKey.DownArrow or ConsoleKey.S:
				if (playerPosition.Top < Console.Height - 1)
					playerPosition.Top++;
				else
				{
					playerPosition.Top = 0;
					currentBiome.Top++;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				break;
			case ConsoleKey.LeftArrow or ConsoleKey.A:
				if (playerPosition.Left > 0)
					playerPosition.Left--;
				else
				{
					playerPosition.Left = Console.Width - 1;
					currentBiome.Left--;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				break;
			case ConsoleKey.RightArrow or ConsoleKey.D:
				if (playerPosition.Left < Console.Width - 1)
					playerPosition.Left++;
				else
				{
					playerPosition.Left = 0;
					currentBiome.Left++;
					biome = Program.ShowBiome(currentBiome, stats);
				}
				break;
			default:
				return;
		}
	}
}
