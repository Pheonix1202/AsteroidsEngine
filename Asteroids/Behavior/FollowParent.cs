using System;

namespace Asteroids
{
    internal class BelongsTo : Behavior
    {
        internal GameObject Parent { get; set; }
        internal Vector2 LocalPosition { get; set; }
        internal Vector2 LocalRotation { get; set; }

        public BelongsTo(GameObject gameObject) : base(gameObject)
        {

        }

        internal override void OnFrame()
        {
            if (Parent != null)
            {
                GameObject.Position = Parent.Position + LocalPosition;
                double localAngle = Math.Atan2(LocalRotation.Y, LocalRotation.X);
                double parentAngle = Math.Atan2(Parent.Rotation.Y, Parent.Rotation.X);
                double angle = parentAngle + localAngle;
                GameObject.Rotation = new Vector2 { X = (float)Math.Cos(angle), Y = (float)Math.Sin(angle) };
            }
        }
    }
}
