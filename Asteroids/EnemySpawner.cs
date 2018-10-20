using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Asteroids
{
    internal class EnemySpawner
    {
        private enum Enemy { AsteroidBig, AsteroidMedium, AsteroidSmall, UFO }

        private const double bigAsteroid_Chance = 0.4;
        private const float asteroid_SpawnPeriod = 2000f;
        private const float ufo_SpawnPeriod = 5000f;
        private const float spawnPeriodDecrease_Coefficient = 0.9f;        
        private const float spawnDelta = 0.4f;
        private const int difficultyChangeInterval = 10000;
        private const int threadSpawnPeriod = 10;
        private const int maxThreadsPossible = 5;
        private const int timerDelay = 300;

        private float Asteroid_CurrentSpawnPeriod;
        private float UFO_CurrentSpawnPeriod;
        private int stepsToSpawnThread;
        private int threadsCount;
        private readonly object locker;
        private readonly float[] spawnSides;
        private List<GameOverToken> gameOverTokens;
        private List<Thread> asteroid_Threads;
        private List<Thread> ufo_Threads;
        private Stack<Enemy> nextWave;
        private Viewport viewport;
        private Random random;
        private Timer timer;

        internal EnemySpawner(Viewport viewport)
        {
            this.viewport = viewport;
            locker = new object();
            random = new Random();
            nextWave = new Stack<Enemy>();
            gameOverTokens = new List<GameOverToken>();
            spawnSides = new float[4]
                {
                    viewport.Height,                                // Left
                    viewport.Height + viewport.Width,               // Top
                    viewport.Height * 2f + viewport.Width,          // Right
                    viewport.Height * 2f + viewport.Width * 2f,     // Bottom
                };
            timer = new Timer { Interval = difficultyChangeInterval, AutoReset = true, Enabled = true };
            timer.Elapsed += new ElapsedEventHandler((o,e) => IncreaseDifficulty());
        }

        internal void StartSpawn()
        {
            Game.Instance.GameOverEvent += GameOver;
            Asteroid_CurrentSpawnPeriod = asteroid_SpawnPeriod;
            UFO_CurrentSpawnPeriod = ufo_SpawnPeriod;
            stepsToSpawnThread = 0;
            threadsCount = 0;

            asteroid_Threads = new List<Thread>();
            ufo_Threads = new List<Thread>();
            timer.Start();
            IncreaseDifficulty();
        }

        internal void GameOver()
        {
            lock (locker) { gameOverTokens.ForEach(x => x.GameOver = true); }           
            timer.Stop();                        
        }

        internal void OnFrame()
        {
            while (nextWave.Count != 0)
            {
                switch (nextWave.Pop())
                {
                    case Enemy.AsteroidBig:
                        new Asteroid(AsteroidSize.Big) { Position = GetSpawnPosition(), Rotation = Vector2.RandomRotation(random) };
                        break;
                    case Enemy.AsteroidMedium:
                        new Asteroid(AsteroidSize.Medium) { Position = GetSpawnPosition(), Rotation = Vector2.RandomRotation(random) };
                        break;
                    case Enemy.AsteroidSmall:
                        new Asteroid(AsteroidSize.Small) { Position = GetSpawnPosition(), Rotation = Vector2.RandomRotation(random) };
                        break;
                    case Enemy.UFO:
                        new UFO() { Position = GetSpawnPosition() };
                        break;
                }
            }
        }

        private void IncreaseDifficulty()
        {
            if (stepsToSpawnThread-- != 0)
            {
                Asteroid_CurrentSpawnPeriod *= spawnPeriodDecrease_Coefficient;
                UFO_CurrentSpawnPeriod *= spawnPeriodDecrease_Coefficient;
            }
            else
            {
                Thread asteroid_Thread = new Thread(SpawnAsteroid) { Name = string.Format("Asteroid spawner {0}", threadsCount), IsBackground = true };
                asteroid_Threads.Add(asteroid_Thread);
                asteroid_Thread.Start();

                Thread ufo_Thread = new Thread(SpawnUFO) { Name = string.Format("UFO spawner {0}", threadsCount), IsBackground = true };
                ufo_Threads.Add(ufo_Thread);
                ufo_Thread.Start();

                stepsToSpawnThread = threadSpawnPeriod;
                if (threadsCount++ == maxThreadsPossible) timer.Dispose();
            }
        }

        private void SpawnAsteroid()
        {
            int delay;
            double coeff;
            GameOverToken token = new GameOverToken();
            gameOverTokens.Add(token);

            while (!token.GameOver)
            {
                lock (locker)
                {
                    coeff = random.NextDouble() * spawnDelta + (1 - spawnDelta / 2);
                    delay = (int)(UFO_CurrentSpawnPeriod * coeff);
                    nextWave.Push(random.NextDouble() < 0.4 ? Enemy.AsteroidBig : Enemy.AsteroidMedium);
                }
                Thread.Sleep(delay);
            }

            gameOverTokens.Remove(token);
        }

        private void SpawnUFO()
        {
            int delay;
            double coeff;
            GameOverToken token = new GameOverToken();
            gameOverTokens.Add(token);

            while (!token.GameOver)
            {
                lock (locker)
                {
                    coeff = random.NextDouble() * spawnDelta + (1 - spawnDelta / 2);
                    delay = (int)(UFO_CurrentSpawnPeriod * coeff);
                    nextWave.Push(Enemy.UFO);
                }
                Thread.Sleep(delay);
            }

            gameOverTokens.Remove(token);
        }

        private Vector2 GetSpawnPosition()
        {
            float r = (float)random.NextDouble() * spawnSides[3];

            if (r < spawnSides[0]) return new Vector2(0f - Viewport.viewportEdgeToBorderDistance, r);
            if (r < spawnSides[1]) return new Vector2(r - spawnSides[0], viewport.Height + Viewport.viewportEdgeToBorderDistance);
            if (r < spawnSides[2]) return new Vector2(viewport.Width + Viewport.viewportEdgeToBorderDistance, viewport.Height - (r - spawnSides[1]));
            if (r < spawnSides[3]) return new Vector2(viewport.Width - (r - spawnSides[2]), 0f - Viewport.viewportEdgeToBorderDistance);

            throw new Exception("Out of viewport size");
        }

        private class GameOverToken
        {
            internal bool GameOver { get; set; }
        }
    }
}
