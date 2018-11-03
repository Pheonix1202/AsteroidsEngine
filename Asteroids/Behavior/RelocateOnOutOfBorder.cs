namespace Asteroids
{
    internal class RelocateOnOutOfBorder : Behavior
    {
        public RelocateOnOutOfBorder(GameObject gameObject) : base(gameObject)
        {

        }

        internal override void OnFrame() => Game.Viewport.RelocateIfCrossedBorder(GameObject);
    }
}
