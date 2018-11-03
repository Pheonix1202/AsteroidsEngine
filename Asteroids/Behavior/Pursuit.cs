namespace Asteroids
{
    internal class Pursuit : Behavior
    {
        internal float MovementSpeed { get; set; }
        protected GameObject Pursued { get; }
        private Vector2 direction;

        public Pursuit(GameObject gameObject, GameObject pursued) : base(gameObject)
        {
            Pursued = pursued;
        }

        internal override void OnFrame()
        {
            direction = (Pursued.Position - GameObject.Position).Normalize();
            GameObject.Position += direction * MovementSpeed;
        }
    }
}
