using System;
using System.Collections.Generic;
using System.Linq;

namespace Asteroids
{
    public class Game : InputPublisher
    {
        public event Action GameOverEvent;
        internal event EventHandler<ScoreEventArgs> ScoreChanged;

        public InputPublisher InputPublisher { get; }
        public int Score { get => score; internal set => SetScore(value); }
        internal List<IVisualizer> Visualizers { get; }
        internal HashSet<GameObject> GameObjects { get; }
        internal GameObjectFactory Factory { get; }
        internal Viewport Viewport { get; }

        internal ScoreEventArgs scoreEventArgs;
        private GameObject player;
        private GameObject enemySpawner;
        private List<GameObject> removeList;
        private List<GameObject> addList;
        private int score;

        public Game()
        {
            GameObjects = new HashSet<GameObject>();
            InputPublisher = new InputPublisher();
            scoreEventArgs = new ScoreEventArgs();
            Visualizers = new List<IVisualizer>();
            Factory = new GameObjectFactory(this);
            Viewport = new Viewport();
            removeList = new List<GameObject>(25);
            addList = new List<GameObject>(25);
        }

        public void StartGame()
        {            
            Score = 0;
            GameOverEvent += OnGameOver;
            player = Factory.CreatePlayer(Viewport.Center, Vector2.DefaultRotation);
            enemySpawner = Factory.CreateEnemySpawner();            
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
            addList.ForEach(x => GameObjects.Add(x));
            addList.Clear();

            GameObjects.ForEach(x => x.OnFrame());

            removeList.ForEach(x => GameObjects.Remove(x));
            removeList.Clear();
        }

        internal void Add(GameObject gameObject) => addList.Add(gameObject);
        internal void Remove(GameObject gameObject) => removeList.Add(gameObject);
        internal IEnumerable<GameObject> Find(string tag) => GameObjects.Where(x => x.Tag == tag).Concat(addList.Where(y => y.Tag == tag));

        private void OnGameOver()
        {
            GameObjects.ForEach(x => x.Destroy());
            GameOverEvent
                .GetInvocationList()
                .Cast<Action>()
                .ForEach(x => GameOverEvent -= x);
        }

        private void SetScore(int value)
        {
            score = value;
            scoreEventArgs.Score = value;
            ScoreChanged?.Invoke(this, scoreEventArgs);
        }
    }
}
