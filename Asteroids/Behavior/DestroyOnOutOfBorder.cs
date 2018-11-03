namespace Asteroids
{
    class DestroyOnOutOfBorder : Behavior
    {      
        public DestroyOnOutOfBorder(GameObject gameObject) : base(gameObject)
        {

        }

        internal override void OnFrame()
        {
            if (Game.Viewport.CrossedBorder(GameObject)) GameObject.Destroy();
        }
    }
}
