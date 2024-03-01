using BATChallenge2022.Enums;
using BATChallenge2022.Models;

namespace BATChallenge2022.Interfaces;
internal interface IPlayer
{
	public void Move(Stats stats, ref Position playerPosition, ref Position currentBiome, ConsoleColor playerColor, ref Biome biome, ref DeathReasons? deathReason);
}
