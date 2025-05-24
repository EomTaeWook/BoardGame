using System;

namespace Assets.Scripts.GameContents.Share
{

    public readonly struct Point
    {
        public static readonly Point Right = new(1, 0);
        public static readonly Point Left = new(-1, 0);
        public static readonly Point Up = new(0, 1);
        public static readonly Point Down = new(0, -1);

        public int X { get; }
        public int Y { get; }

        public Point(int x, int y) { X = x; Y = y; }

        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);

        public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Point a, Point b) => !(a == b);

        public override bool Equals(object obj)
        {
            return obj is Point point &&
                   X == point.X &&
                   Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
