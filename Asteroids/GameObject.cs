using System;
using System.Linq;
using System.Collections.Generic;

namespace Asteroids
{
    internal class GameObject
    {
        internal event EventHandler<PositionEventArgs> PositionChanged;
        internal event EventHandler<RotationEventArgs> RotationChanged;
        internal event EventHandler Destroyed;

        internal Vector2 Position { get => position; set => SetPosition(value); }
        internal Vector2 Rotation { get => rotation; set => SetRotation(value); }
        internal bool Active { get; set; }
        internal string Tag { get; set; }
        internal Game Game { get; }

        protected PositionEventArgs positionEventArgs;
        protected RotationEventArgs rotationEventArgs;
        private Vector2 position;
        private Vector2 rotation;

        private HashSet<Behavior> Behaviors { get; }

        public GameObject(Game game)
        {
            positionEventArgs = new PositionEventArgs();
            rotationEventArgs = new RotationEventArgs();
            Behaviors = new HashSet<Behavior>();
            Active = true;
            Game = game;
            game.Add(this);
        }

        internal void OnFrame()
        {
            if (Active)
                Behaviors.ForEach(x => x.OnFrame());
        }

        internal virtual void Destroy()
        {
            Active = false;

            PositionChanged?
                .GetInvocationList()
                .Cast<EventHandler<PositionEventArgs>>()
                .ForEach(x => PositionChanged -= x);

            RotationChanged?
                .GetInvocationList()
                .Cast<EventHandler<RotationEventArgs>>()
                .ForEach(x => RotationChanged -= x);
           
            Behaviors
                .OfType<IDisposable>()
                .ForEach(x => x.Dispose());

            Destroyed?.Invoke(this, EventArgs.Empty);
            Game.Remove(this);

            Destroyed?
                .GetInvocationList()
                .Cast<EventHandler>()
                .ForEach(x => Destroyed -= x);
        }

        internal void AddBehavior(Behavior behavior) 
        {
            if (!Behaviors.Contains(behavior)) Behaviors.Add(behavior);
        }

        internal void RemoveBehavior(Behavior behavior)
        {
            if (!Behaviors.Contains(behavior)) Behaviors.Remove(behavior);
        }

        internal T GetBehavior<T>() where T : Behavior => Behaviors.OfType<T>().FirstOrDefault();

        internal void Bind(IGameObject gameObject)
        {
            Destroyed += gameObject.OnDestroy;
            PositionChanged += gameObject.OnPositionChanged;
            RotationChanged += gameObject.OnRotationChanged;
        }

        private void SetPosition(Vector2 value)
        {
            position = value;
            positionEventArgs.X = value.X;
            positionEventArgs.Y = value.Y;
            PositionChanged?.Invoke(this, positionEventArgs);
        }

        private void SetRotation(Vector2 value)
        {
            rotation = value;
            rotationEventArgs.X = value.X;
            rotationEventArgs.Y = value.Y;
            RotationChanged?.Invoke(this, rotationEventArgs);            
        }
    }


}
