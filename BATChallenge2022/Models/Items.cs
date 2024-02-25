using BATChallenge2022.Interfaces;

namespace BATChallenge2022.Models;

public class MoneyItem : IItem
{
	public char Character { get; set; } = '$';
	public ConsoleColor Color { get; set; } = ConsoleColor.Green;

	public void Collected(Stats stats) => stats.Money += Money;

	int Money = Random.Shared.Next(50, 500);
}

public class HungerItem : IItem
{
	public char Character { get; set; } = '¶';
	public ConsoleColor Color { get; set; } = ConsoleColor.Yellow;

	public void Collected(Stats stats) => stats.Hunger += Hunger;

	int Hunger = Random.Shared.Next(75, 150);
}
