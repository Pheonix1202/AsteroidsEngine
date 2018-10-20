using System;
using System.Linq;

namespace Asteroids
{
    internal abstract class GameObject
    {
        internal event EventHandler Destroyed;
        internal event EventHandler<PositionEventArgs> PositionChanged;

        internal int Score { get; set; }
        internal float MovementSpeed { get; set; }
        internal float ColliderRadius { get; set; }
        
        internal Vector2 Position
        {
            get => position; set
            {
                position = value;
                positionEventArgs.X = value.X;
                positionEventArgs.Y = value.Y;
                PositionChanged?.Invoke(this, positionEventArgs);
            }
        }
        protected PositionEventArgs positionEventArgs;
        protected bool gameOver;
        private Vector2 position;

        internal GameObject()
        {
            Game.Instance.GameOverEvent += () => gameOver = true;
            positionEventArgs = new PositionEventArgs();
        } 

        internal virtual void Destroy()
        {           
            PositionChanged?
                .GetInvocationList()
                .Cast<EventHandler<PositionEventArgs>>()
                .ForEach(x => PositionChanged -= x);
            
            Game.Instance.toRemove.Add(this);
            Destroyed?.Invoke(this, EventArgs.Empty);
        }

        internal virtual bool CollidesWith(GameObject another)
        {
            Vector2 distance = another.Position - this.Position;
            return Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y) < another.ColliderRadius + this.ColliderRadius;
        }

        protected void Bind(IGameObject gameObject)
        {
            Destroyed += gameObject.OnDestroy;
            PositionChanged += gameObject.OnPositionChanged;
        }
    }
}
