public struct Position(int left, int top) : IEquatable<Position>
{
	public int Left { get; set; } = left;
	public int Top { get; set; } = top;

	public bool Equals(Position other) => Left == other.Left && Top == other.Top;

	public Position Transform(int left, int top) => new(Left + left, Top + top);

	public static explicit operator Position((int Left, int Top) v) => new(v.Left, v.Top);
}