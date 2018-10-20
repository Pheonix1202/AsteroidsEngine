using System.Linq;

namespace Asteroids
{
    class Missile : DirectionalGameObject
    {
        internal Missile() : base()
        {
            Game.Instance.missiles.Add(this);
            Game.Instance.Visualizers
                .Select(x => x.Factory.CreateMissile())
                .ForEach(p => Bind(p));

            MovementSpeed = 12f;
            ColliderRadius = 5f;
        }

        protected void Bind(IMissile missile)
        {
            base.Bind(missile);
        }
    }
}
