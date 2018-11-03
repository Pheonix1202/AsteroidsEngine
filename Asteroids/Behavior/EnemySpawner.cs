using System;
using System.Linq;

namespace Asteroids
{
    internal class EnemySpawner : Behavior
    {
        private const double bigAsteroid_Chance = 0.4;
        private const float asteroid_SpawnPeriod = 2000f;
        private const float      ufo_SpawnPeriod = 5000f;
        private const float spawnPeriodDecrease_Coefficient = 0.9f;
        private const int difficultyChangeInterval = 10000;

        internal bool Active { get; set; } = true;

        private readonly float[] spawnSides;
        private GameObject player;
        private Random random;
        private DelayedTaskManager dtm;

        private float asteroid_CurrentSpawnPeriod = asteroid_SpawnPeriod;
        private float ufo_CurrentSpawnPeriod = ufo_SpawnPeriod;

        public EnemySpawner(GameObject gameObject) : base(gameObject)
        {
            random = new Random();
            spawnSides = new float[4]
            {
                Game.Viewport.Height,                                   // Left
                Game.Viewport.Height + Game.Viewport.Width,             // Top
                Game.Viewport.Height * 2f + Game.Viewport.Width,        // Right
                Game.Viewport.Height * 2f + Game.Viewport.Width * 2f,   // Bottom
            };
            player = Game.Find("Player").FirstOrDefault();
            dtm = GameObject.GetBehavior<DelayedTaskManager>();
            if (dtm == null) throw new NullReferenceException("DelayedTaskManager is missing");
            dtm.Invoke(difficultyChangeInterval, IncreaseDifficulty);
            SpawnAsteroid();
            SpawnUFO();
        }

        internal override void OnFrame()
        {

        }

        private void IncreaseDifficulty()
        {
            ufo_CurrentSpawnPeriod *= spawnPeriodDecrease_Coefficient;
            asteroid_CurrentSpawnPeriod *= spawnPeriodDecrease_Coefficient;
            if (Active) dtm.Invoke(difficultyChangeInterval, IncreaseDifficulty);
        }

        private void SpawnUFO()
        {
            if (GameObject.Active)
            {
                Game.Factory.CreateUFO(GetSpawnPosition(), player);
                dtm.Invoke((long)ufo_CurrentSpawnPeriod, SpawnUFO);
            }           
        }

        private void SpawnAsteroid()
        {
            if (GameObject.Active)
            {
                AsteroidSize size = bigAsteroid_Chance >= random.NextDouble() ? AsteroidSize.Big : AsteroidSize.Medium;
                Game.Factory.CreateAsteroid(size, GetSpawnPosition(), Vector2.RandomRotation(random));
                dtm.Invoke((long)asteroid_CurrentSpawnPeriod, SpawnAsteroid);
            }
        }

        private Vector2 GetSpawnPosition()
        {
            float r = (float)random.NextDouble() * spawnSides[3];

            if (r < spawnSides[0]) return new Vector2(0f - Viewport.viewportEdgeToBorderDistance, r);
            if (r < spawnSides[1]) return new Vector2(r - spawnSides[0], Game.Viewport.Height + Viewport.viewportEdgeToBorderDistance);
            if (r < spawnSides[2]) return new Vector2(Game.Viewport.Width + Viewport.viewportEdgeToBorderDistance, Game.Viewport.Height - (r - spawnSides[1]));
            if (r < spawnSides[3]) return new Vector2(Game.Viewport.Width - (r - spawnSides[2]), 0f - Viewport.viewportEdgeToBorderDistance);

            throw new Exception("Out of viewport size");
        }
    }
}
