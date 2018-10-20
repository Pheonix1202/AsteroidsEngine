using System;

namespace Asteroids
{
    internal struct Vector2
    {
        internal float X { get; set; }
        internal float Y { get; set; }
        internal float Length { get { return (float)Math.Sqrt(X * X + Y * Y); } }
        internal static Vector2 DefaultRotation { get => new Vector2 { X = 0f, Y = 1f }; }

        internal Vector2(float x, float y)
        {
            X = x; Y = y;
        }

        public Vector2 Normalize()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y);
            X /= length;
            Y /= length;
            return this;
        }

        public static Vector2 RandomRotation(Random random)
        {
            float angle = (float)(random.NextDouble() * Math.PI * 2.0);
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            return new Vector2(x, y);
        }

        public static Vector2 operator *(Vector2 a, float b) { return new Vector2 { X = a.X * b, Y = a.Y * b }; }
        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2 { X = a.X + b.X, Y = a.Y + b.Y }; }
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2 { X = a.X - b.X, Y = a.Y - b.Y }; }
    }
}
