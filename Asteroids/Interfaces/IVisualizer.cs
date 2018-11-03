namespace Asteroids
{
    public interface IVisualizer
    {
        IAsteroidsFactory Factory { get; }
        void OnScoreChanged(object sender, ScoreEventArgs e);
        void OnLaserCountChanged(object sender, LaserEventArgs e);
    }
}
