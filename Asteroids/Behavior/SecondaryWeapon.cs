using System;

namespace Asteroids
{
    internal class SecondaryWeapon : Behavior, IDisposable
    {
        private const int maxCharges = 3;
        private const int laser_Cooldown = 1500;
        private const int laser_AccumulateTime = 6000;
        private const int laser_EmitTime = 250;

        internal event EventHandler<LaserEventArgs> ChargesCountChanged;

        internal bool Active { get => active; set => SetActive(value); }
        internal int Charges { get => chargeCount; set => SetCharges(value); }

        private int chargeCount;
        private bool active;
        private bool onCooldown;
        private bool accumulating;

        private LaserEventArgs laserEventArgs;
        private DelayedTaskManager dtm;
        private GameObject laser;

        public SecondaryWeapon(GameObject gameObject) : base(gameObject)
        {
            Active = true;
            dtm = GameObject.GetBehavior<DelayedTaskManager>();
            laserEventArgs = new LaserEventArgs();
        }

        internal override void OnFrame()
        {
            if (!accumulating && Charges < maxCharges)
            {
                accumulating = true;
                dtm.Invoke(laser_AccumulateTime, () => { Charges++; accumulating = false; });
            }                
        }

        private void EmitLaser()
        {
            if (Active && !onCooldown && Charges > 0)
            {
                onCooldown = true;
                laser = Game.Factory.CreateLaser(GameObject);
                dtm.Invoke(laser_Cooldown, () => onCooldown = false);
                dtm.Invoke(laser_EmitTime, () => laser.Destroy());
                Charges--;
                
            }
        }

        private void SetActive(bool value)
        {
            active = value;
            if (value) Game.InputPublisher.Laser += EmitLaser;
            else Game.InputPublisher.Laser -= EmitLaser;
        }

        private void SetCharges(int value)
        {
            chargeCount = value;
            laserEventArgs.Charges = value;
            ChargesCountChanged?.Invoke(this, laserEventArgs);
        }

        public void Dispose() => Game.InputPublisher.Laser -= EmitLaser;
    }
}
