namespace Asteroids
{
    class MoveForward : Behavior
    {
        internal float MovementSpeed { get; set; }

        public MoveForward(GameObject gameObject) : base(gameObject)
        {

        }

        internal override void OnFrame() => GameObject.Position += GameObject.Rotation * MovementSpeed;
    }
}
