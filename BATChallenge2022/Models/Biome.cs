using BATChallenge2022.Interfaces;

namespace BATChallenge2022.Models;
public record Biome(ConsoleColor Color, Dictionary<Position, IItem> Items);
