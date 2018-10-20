using System;

namespace Asteroids
{
    public class PositionEventArgs : EventArgs
    {
        public float X { get; internal set; }
        public float Y { get; internal set; }
    }
}
