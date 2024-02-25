using BATChallenge2022.Interfaces;

namespace BATChallenge2022.Models;

public class MoneyItem : IItem
{
	public char Character { get; set; } = '$';
	public ConsoleColor Color { get; set; } = ConsoleColor.Yellow;
	//public Position Position { get; set; } = new(Random.Shared.Next(Console.BufferWidth - 1), Random.Shared.Next(Console.BufferHeight - 1));

	public int Money { get; set; } = Random.Shared.Next(50, 500);

	public void Collected(Stats stats) => stats.Money += Money;
}

public class HungerItem : IItem
{
	public char Character { get; set; } = '¶';
	public ConsoleColor Color { get; set; } = ConsoleColor.Yellow;
	//public Position Position { get; set; } = new(Random.Shared.Next(Console.BufferWidth - 1), Random.Shared.Next(Console.BufferHeight - 1));

	public int Hunger { get; set; } = Random.Shared.Next(75, 150);

	public void Collected(Stats stats) => stats.Hunger += Hunger;
}
