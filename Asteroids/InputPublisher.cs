using System;

namespace Asteroids
{
    public class InputPublisher
    {
        internal event Action Forward;
        internal event Action ToLeft;
        internal event Action ToRight;
        internal event Action Fire;
        internal event Action Laser;

        public void ForwardPressed()
        {
            Forward?.Invoke();
        }

        public void ToLeftPressed()
        {
            ToLeft?.Invoke();
        }

        public void ToRightPressed()
        {
            ToRight?.Invoke();
        }

        public void FirePressed()
        {
            Fire?.Invoke();
        }

        public void LaserPressed()
        {
            Laser?.Invoke();
        }
    }
}
