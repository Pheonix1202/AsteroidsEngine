using System;
using System.Linq;

namespace Asteroids
{
    internal abstract class DirectionalGameObject : GameObject
    {
        internal event EventHandler<RotationEventArgs> RotationChanged;

        internal Vector2 Rotation
        {
            get => rotation; set
            {
                rotation = value;
                RotationEventArgs.X = value.X;
                RotationEventArgs.Y = value.Y;
                RotationChanged?.Invoke(this, RotationEventArgs);
            }
        }
        protected internal RotationEventArgs RotationEventArgs { protected set; get; }
        private Vector2 rotation;
        
        internal DirectionalGameObject() : base()
        {
            RotationEventArgs = new RotationEventArgs();
        }

        internal virtual void MoveForward()
        {
            Position += rotation * MovementSpeed;
        }

        internal override void Destroy()
        {
            RotationChanged?
                .GetInvocationList()
                .Cast<EventHandler<RotationEventArgs>>()
                .ForEach(x => RotationChanged -= x);

            base.Destroy();
        }

        protected void Bind(IDirectionalGameObject gameObject)
        {
            base.Bind(gameObject);
            RotationChanged += gameObject.OnRotationChanged;
        }
    }
}
