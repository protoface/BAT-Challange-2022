using BATChallenge2022.Models;

namespace BATChallenge2022.Interfaces;

public interface IItem
{
	public char Character { get; set; }
	public ConsoleColor Color { get; set; }
	public void Collected(Stats stats);
}
