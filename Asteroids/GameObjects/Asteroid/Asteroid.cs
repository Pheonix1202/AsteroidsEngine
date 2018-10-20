using System;
using System.Linq;

namespace Asteroids
{
    internal class Asteroid : DirectionalGameObject
    {
        internal const int mediumShardsCount = 2;
        internal const int smallShardsCount = 3;

        internal AsteroidSize Size { get; private set; }

        private readonly Random random;

        internal Asteroid(AsteroidSize size) : base()
        {
            Size = size;
            random = new Random();
            Game.Instance.asteroids.Add(this);
            Game.Instance.Visualizers
                .Select(x => x.Factory.CreateAsteroid(Size))
                .ForEach(p => Bind(p));

            switch (Size)
            {
                case AsteroidSize.Big:
                    Score = 10;
                    ColliderRadius = 40f;
                    MovementSpeed = 1f;
                    break;
                case AsteroidSize.Medium:
                    Score = 8;
                    ColliderRadius = 26f;
                    MovementSpeed = 1.8f;
                    break;
                case AsteroidSize.Small:
                    Score = 6;
                    ColliderRadius = 15f;
                    MovementSpeed = 2.7f;
                    break;
            }            
        }

        internal void DestroyByMissile()
        {
            int i = 0;
            if (Size == AsteroidSize.Big)
                for (; i < mediumShardsCount; i++)
                    new Asteroid(AsteroidSize.Medium) { Position = Position, Rotation = Vector2.RandomRotation(random) };
            else if (Size == AsteroidSize.Medium)
                for (; i < smallShardsCount; i++)
                    new Asteroid(AsteroidSize.Small) { Position = Position, Rotation = Vector2.RandomRotation(random) };

            DestroyByPlayer();
        }

        internal void DestroyByPlayer()
        {
            Game.Instance.Score += Score;
            base.Destroy();
        }

        protected void Bind(IAsteroid asteroid)
        {
            base.Bind(asteroid);
        }
    }
}
