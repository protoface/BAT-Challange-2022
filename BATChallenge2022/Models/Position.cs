public struct Position(int left, int top) : IEquatable<Position>
{
	public int Left { get; set; } = left;
	public int Top { get; set; } = top;
	public int X => Left;
	public int Y => Top;

	public bool Equals(Position other) => X == other.X && Y == other.Y;

	public static explicit operator Position((int Left, int Top) v) => new(v.Left, v.Top);
}