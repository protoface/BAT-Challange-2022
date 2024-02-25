namespace BATChallenge2022.Models;
public class Stats
{
	public int BlocksTravelled { get; set; }
	public Dictionary<Position, Biome> Biomes { get; set; } = [];

	public int Money { get; set; } = 0;

	public int Hunger { get; set; } = 200;

	public int LastStatWidth { get; set; }
}
