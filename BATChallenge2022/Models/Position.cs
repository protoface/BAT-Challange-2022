public struct Position(int left, int top)
{
	public int Left { get; set; } = left;
	public int Top { get; set; } = top;
	public Position Transform(int left, int top) => new(Left + left, Top + top);


	public override bool Equals(object? other) => other is Position position && this == position;
	public override int GetHashCode() => Left.GetHashCode() + Top.GetHashCode();


	public static bool operator ==(Position left, Position right) => left.Left == right.Left && left.Top == right.Top;
	public static bool operator !=(Position left, Position right) => !(left == right);
	public static Position operator -(Position left, Position right) => new(left.Left - right.Left, left.Top - right.Top);
	public static Position operator +(Position left, Position right) => new(left.Left + right.Left, left.Top + right.Top);

	public static explicit operator Position((int Left, int Top) v) => new(v.Left, v.Top);
}