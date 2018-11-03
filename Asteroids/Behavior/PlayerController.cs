using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    internal enum RotateDirection { Clockwise = -1, CounterClockwise = 1 }

    internal class PlayerController : Behavior, IDisposable
    {
        internal bool Active { get => active; set => SetActive(value); }
        internal float AngularSpeed { get; set; }
        internal float MovementSpeed { get; set; }

        private bool active;

        public PlayerController(GameObject gameObject) : base(gameObject)
        {
            Active = true;
        }

        internal override void OnFrame()
        {
            
        }

        private void MoveForward() => GameObject.Position += GameObject.Rotation * MovementSpeed;

        private void Rotate(RotateDirection direction)
        {
            double radians = Math.Atan2(GameObject.Rotation.Y, GameObject.Rotation.X);
            float rotationAngle = AngularSpeed * (direction == RotateDirection.Clockwise ? -1 : 1);
            radians += rotationAngle * Math.PI / 180.0;
            GameObject.Rotation = new Vector2 { X = (float)Math.Cos(radians), Y = (float)Math.Sin(radians) };
        }

        private void SetActive(bool value)
        {
            active = value;
            if (value)
            {
                Game.InputPublisher.Forward += MoveForward;
                Game.InputPublisher.ToLeft += () => Rotate(RotateDirection.CounterClockwise);
                Game.InputPublisher.ToRight += () => Rotate(RotateDirection.Clockwise);
            }
            else
            {
                Game.InputPublisher.Forward -= MoveForward;
                Game.InputPublisher.ToLeft -= () => Rotate(RotateDirection.CounterClockwise);
                Game.InputPublisher.ToRight -= () => Rotate(RotateDirection.Clockwise);
            }
        }

        public void Dispose()
        {
            Game.InputPublisher.Forward -= MoveForward;
            Game.InputPublisher.ToLeft -= () => Rotate(RotateDirection.CounterClockwise);
            Game.InputPublisher.ToRight -= () => Rotate(RotateDirection.Clockwise);
        }
    }


}
