namespace Asteroids
{
    public interface IPlayer : IDirectionalGameObject
    {
        void OnLaserChargeCountChanged(object sender, LaserEventArgs e);
    }
}
