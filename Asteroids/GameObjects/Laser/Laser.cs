using System;
using System.Linq;

namespace Asteroids
{
    class Laser : DirectionalGameObject
    {
        internal bool IsEmitting { get; set; }

        internal Laser() : base()
        {
            Game.Instance.Visualizers
                .Select(x => x.Factory.CreateLaser())
                .ForEach(p => Bind(p));

            IsEmitting = true;
        }

        internal override bool CollidesWith(GameObject another)
        {
            Vector2 v1 = another.Position - Position;
            Vector2 v2 = Rotation;
            float scalarProduct = v1.X * v2.X + v1.Y * v2.Y;
            if (scalarProduct < 0) return false;
            double angle = Math.Acos(scalarProduct / (v1.Length * v2.Length));
            float distanceToLaser = v1.Length * (float)Math.Sin(angle);
            return another.ColliderRadius > distanceToLaser;
        }

        protected void Bind(ILaser laser)
        {
            base.Bind(laser);
        }
    }
}
