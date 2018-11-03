namespace Asteroids
{
    public interface IAsteroidsFactory
    {
        IGameObject CreatePlayer();
        IGameObject CreateMissile();
        IGameObject CreateLaser();
        IGameObject CreateUFO();
        IGameObject CreateAsteroid(AsteroidSize size);
    }
}
