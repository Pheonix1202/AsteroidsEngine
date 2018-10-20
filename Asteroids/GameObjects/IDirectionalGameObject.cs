namespace Asteroids
{
    public interface IDirectionalGameObject : IGameObject
    {
        void OnRotationChanged(object sender, RotationEventArgs e);
    }
}
