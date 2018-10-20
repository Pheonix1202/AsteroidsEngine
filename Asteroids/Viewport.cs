namespace Asteroids
{
    internal class Viewport
    {
        internal const float viewportEdgeToBorderDistance = 20f;

        internal float Width { get; set; }
        internal float Height { get; set; }
        internal Vector2 Center { get { return new Vector2(Width / 2, Height / 2); } }

        internal Viewport()
        {
            Width = 1600;
            Height = 900;
        }

        internal void RelocateIfCrossedBorder(GameObject obj)
        {
            Vector2 position = obj.Position;
            if (position.X < -viewportEdgeToBorderDistance) position.X = Width;
            else if (position.X > Width + viewportEdgeToBorderDistance) position.X = 0f;

            if (position.Y < -viewportEdgeToBorderDistance) position.Y = Height;
            else if (position.Y > Height + viewportEdgeToBorderDistance) position.Y = 0f;
            obj.Position = position;
        }

        internal bool CrossedBorder(GameObject obj)
        {
            return  obj.Position.X < -viewportEdgeToBorderDistance
                    ||
                    obj.Position.X > Width + viewportEdgeToBorderDistance
                    ||
                    obj.Position.Y < -viewportEdgeToBorderDistance
                    ||
                    obj.Position.Y > Height + viewportEdgeToBorderDistance;
        }

    }
}
