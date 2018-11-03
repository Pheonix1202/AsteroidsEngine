using System;
using System.Linq;

namespace Asteroids
{
    internal class Laser : Behavior
    {
        public Laser(GameObject gameObject) : base(gameObject)
        {

        }

        internal override void OnFrame()
        {
            Game.GameObjects
                .Where(LaysOnRay)
                .Select(x => x.GetBehavior<Enemy>())
                .Where(x => x != null)
                .ForEach(x => x.DestroyBy(GameObject));           
        }

        private bool LaysOnRay(GameObject another)
        {
            Vector2 v1 = another.Position - GameObject.Position;
            Vector2 v2 = GameObject.Rotation;
            float scalarProduct = v1.X * v2.X + v1.Y * v2.Y;
            if (scalarProduct < 0) return false;
            double angle = Math.Acos(scalarProduct / (v1.Length * v2.Length));
            float distanceToLaser = v1.Length * (float)Math.Sin(angle);
            Collider anotherCollider = another.GetBehavior<Collider>();
            return  anotherCollider == null ? false : anotherCollider.ColliderRadius > distanceToLaser;
        }
    }
}
