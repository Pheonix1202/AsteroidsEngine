using System;

namespace Asteroids
{
    public interface IGameObject
    {
        void OnDestroy(object sender, EventArgs e);
        void OnPositionChanged(object sender, PositionEventArgs e);
    }
}
