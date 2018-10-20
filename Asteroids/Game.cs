using System;
using System.Collections.Generic;
using System.Linq;

namespace Asteroids
{
    public class Game : InputPublisher
    {
        public static Game Instance { get { if (instance == null) instance = new Game(); return instance; } }
        private static Game instance;

        public event Action GameOverEvent;
        public InputPublisher InputPublisher { get; private set; }
        public int Score { get { return score; } internal set { score = value; scoreEventArgs.Score = value; ScoreChanged?.Invoke(this, scoreEventArgs); } }
        
        internal event EventHandler<ScoreEventArgs> ScoreChanged;
        internal ScoreEventArgs scoreEventArgs;
        internal List<IVisualizer> Visualizers { get; private set; }
        internal List<GameObject> toRemove;
        internal List<Asteroid> asteroids;
        internal List<Missile> missiles;
        internal List<UFO> ufos;

        private Viewport viewport;
        private readonly EnemySpawner enemySpawner;
        private Player player;
        private int score;

        private Game()
        {
            InputPublisher = new InputPublisher();
            scoreEventArgs = new ScoreEventArgs();
            Visualizers = new List<IVisualizer>();
            asteroids = new List<Asteroid>();
            ufos = new List<UFO>();
            missiles = new List<Missile>();
            toRemove = new List<GameObject>();
            viewport = new Viewport();
            enemySpawner = new EnemySpawner(viewport);
        }

        public void StartGame()
        {            
            Score = 0;
            GameOverEvent += OnGameOver;
            player = new Player { Position = viewport.Center, Rotation = Vector2.DefaultRotation };
            enemySpawner.StartSpawn();
        }

        public void FinishGame()
        {
            GameOverEvent?.Invoke();
        }

        public void AddVisualizers(params IVisualizer[] visualizers)
        {
            visualizers.ForEach(x => Visualizers.Add(x));
            Visualizers.ForEach(x => ScoreChanged += x.OnScoreChanged);
        }

        public void OnFrame()
        {
            viewport.RelocateIfCrossedBorder(player);
            asteroids.ForEach(x => viewport.RelocateIfCrossedBorder(x));
            asteroids.ForEach(x => x.MoveForward());
            ufos.ForEach(x => x.PursuitPlayer(player));
            missiles.ForEach(x => x.MoveForward());
            missiles
                .Where(x => viewport.CrossedBorder(x))
                .ForEach(x => x.Destroy());

            bool playerCollidesWithAsteroid = asteroids.Where(x => x.CollidesWith(player)).Any(),
                 playerCollidesWithUfo = ufos.Where(x => x.CollidesWith(player)).Any();

            if (playerCollidesWithAsteroid || playerCollidesWithUfo) GameOverEvent();

            missiles.ForEach(x =>
            {
                UFO ufo = ufos.Where(y => x.CollidesWith(y)).FirstOrDefault();
                Asteroid asteroid = asteroids.Where(y => x.CollidesWith(y)).FirstOrDefault();
                if (asteroid != null) { x.Destroy(); asteroid.DestroyByMissile(); }
                else if (ufo != null) { x.Destroy(); ufo.DestroyByPlayer(); }
            });

            player.OnFrame();
            enemySpawner.OnFrame();
            toRemove.ForEach(Remove);
            toRemove.Clear();
        }

        private void OnGameOver()
        {
            asteroids.ForEach(x => x.Destroy());
            missiles.ForEach(x => x.Destroy());
            ufos.ForEach(x => x.Destroy());
            player.Destroy();

            toRemove.ForEach(Remove);
            toRemove.Clear();

            GameOverEvent
                .GetInvocationList()
                .Cast<Action>()
                .ForEach(x => GameOverEvent -= x);
        }

        private void Remove(GameObject gameObject)
        {
            if (gameObject is Missile) missiles.Remove(gameObject as Missile);
            else if (gameObject is UFO) ufos.Remove(gameObject as UFO);
            else if (gameObject is Asteroid) asteroids.Remove(gameObject as Asteroid);
        }
    }
}
