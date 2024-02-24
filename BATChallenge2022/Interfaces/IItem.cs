namespace BATChallenge2022.Interfaces;

public interface IItem
{
	public char Character { get; set; }
	public ConsoleColor Color { get; set; }
	public Position Position { get; set; }
}
