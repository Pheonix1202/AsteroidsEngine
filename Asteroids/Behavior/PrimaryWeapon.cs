using System;

namespace Asteroids
{
    internal class PrimaryWeapon : Behavior, IDisposable
    {
        internal bool Active { get => active; set => SetActive(value); }
        internal float AttackSpeed { get => attackSpeed; set => SetAttackSpeed(value); }

        private bool active;
        private bool onCooldown;
        private int cooldown;
        private float attackSpeed;

        private DelayedTaskManager dtm;

        public PrimaryWeapon(GameObject gameObject) : base(gameObject)
        {
            Active = true;
            dtm = GameObject.GetBehavior<DelayedTaskManager>();
        }

        internal override void OnFrame()
        {
            
        }

        private void Fire()
        {

            if (Active && !onCooldown)
            {
                onCooldown = true;
                Game.Factory.CreateMissile(GameObject.Position, GameObject.Rotation);
                dtm.Invoke(cooldown, () => onCooldown = false);
            }           
        }

        private void SetActive(bool value)
        {
            active = value;
            if (value) Game.InputPublisher.Fire += Fire;
            else Game.InputPublisher.Fire -= Fire;           
        }

        private void SetAttackSpeed(float value)
        {
            attackSpeed = value;
            cooldown = (int)(1000f / value);
        }

        public void Dispose() => Game.InputPublisher.Fire -= Fire;

    }
}
