namespace Asteroids
{
    internal abstract class Behavior
    {
        internal GameObject GameObject { get; }
        internal Game Game { get; }

        public Behavior(GameObject gameObject)
        {
            GameObject = gameObject;
            GameObject.AddBehavior(this);
            Game = gameObject.Game;
        }

        internal abstract void OnFrame();
    }
}
