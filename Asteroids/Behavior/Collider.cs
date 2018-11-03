using System;
using System.Linq;

namespace Asteroids
{
    internal class Collider : Behavior, IDisposable
    {
        internal event Action<GameObject> Collision;

        internal float ColliderRadius { get; set; }

        public Collider(GameObject gameObject) : base(gameObject)
        {

        }

        public Collider(GameObject gameObject, Action<GameObject> onCollisionCallback) : base(gameObject)
        {
            Collision += onCollisionCallback;
        }

        internal override void OnFrame()
        {
            GameObject.Game.GameObjects
                .Select(x => x.GetBehavior<Collider>())
                .Where(x => x != null && x != this && Colliding(x))
                .ForEach(x => Collision?.Invoke(x.GameObject));  
        }

        private bool Colliding(Collider another)
        {
            Vector2 distance = another.GameObject.Position - this.GameObject.Position;
            return Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y) < another.ColliderRadius + this.ColliderRadius;
        }

        public void Dispose()
        {
            Collision?
                .GetInvocationList()
                .Cast<Action<GameObject>>()
                .ForEach(x => Collision -= x);
        }
    }
}
