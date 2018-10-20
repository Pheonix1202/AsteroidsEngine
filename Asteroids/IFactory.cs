namespace Asteroids
{
    public interface IAsteroidsFactory
    {
        IPlayer CreatePlayer();
        IMissile CreateMissile();
        ILaser CreateLaser();
        IUFO CreateUFO();
        IAsteroid CreateAsteroid(AsteroidSize size);
    }
}
