using System;

namespace Asteroids
{
    internal enum EnemyClass { Undefined, UFO, BigAsteroid, MediumAsteroid, SmallAsteroid }

    internal class Enemy : Behavior
    {
        internal int Bounty { get; set; }
        internal EnemyClass EnemyClass { get; set; } = EnemyClass.Undefined;

        private readonly Random random;

        public Enemy(GameObject gameObject) : base(gameObject)
        {
            random = new Random();
        }

        internal override void OnFrame()
        {
            
        }

        internal void DestroyBy(GameObject destroyer)
        {
            switch (destroyer.Tag)
            {
                case "Missile":
                    Game.Score += Bounty;
                    if (EnemyClass == EnemyClass.BigAsteroid)
                    {
                        Game.Factory.CreateAsteroid(AsteroidSize.Medium, GameObject.Position, Vector2.RandomRotation(random));
                        Game.Factory.CreateAsteroid(AsteroidSize.Medium, GameObject.Position, Vector2.RandomRotation(random));
                    }
                    else if (EnemyClass == EnemyClass.MediumAsteroid)
                    {
                        Game.Factory.CreateAsteroid(AsteroidSize.Small, GameObject.Position, Vector2.RandomRotation(random));
                        Game.Factory.CreateAsteroid(AsteroidSize.Small, GameObject.Position, Vector2.RandomRotation(random)); 
                        Game.Factory.CreateAsteroid(AsteroidSize.Small, GameObject.Position, Vector2.RandomRotation(random));
                    }
                    GameObject.Destroy();
                    destroyer.Destroy();
                    break;
                case "Laser":
                    Game.Score += Bounty;
                    GameObject.Destroy();
                    destroyer.Destroy();
                    break;
                default: throw new Exception("Unhandled source of damage");
            }
        }
    }
}
