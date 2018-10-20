using System;

namespace Asteroids
{
    public class LaserEventArgs : EventArgs
    {      
        public int Charges { get; internal set; }
    }
}
