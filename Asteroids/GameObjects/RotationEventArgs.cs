using System;

namespace Asteroids
{
    public class RotationEventArgs : EventArgs
    {
        public float X { get; internal set; }
        public float Y { get; internal set; }
        public float RotationAngle { get; internal set; }
    }
}
