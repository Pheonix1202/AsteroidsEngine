using System.Linq;

namespace Asteroids
{
    internal class UFO : GameObject
    {
        internal UFO() : base()
        {
            Game.Instance.ufos.Add(this);
            Game.Instance.Visualizers
                .Select(x => x.Factory.CreateUFO())
                .ForEach(p => Bind(p));

            Score = 15;
            MovementSpeed = 2f;
            ColliderRadius = 24f;
        }

        internal void PursuitPlayer(Player player)
        {
            Vector2 direction = (player.Position - Position).Normalize();
            Position += direction * MovementSpeed;
        }

        internal void DestroyByPlayer()
        {
            Game.Instance.Score += Score;
            Destroy();
        }

        protected void Bind(IUFO ufo)
        {
            base.Bind(ufo);
        }
    }
}
