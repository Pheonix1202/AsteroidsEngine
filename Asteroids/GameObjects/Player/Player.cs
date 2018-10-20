using System;
using System.Linq;
using System.Threading;


namespace Asteroids
{
    internal enum RotateDirection { Clockwise = -1, CounterClockwise = 1 }

    internal class Player : DirectionalGameObject
    {
        internal const int laser_ChargeUpTime = 7000;
        internal const int laser_Cooldown = 1500;
        internal const int laser_DurationTime = 200;
        internal const int laser_MaxCharges = 3;

        internal event EventHandler<LaserEventArgs> LaserEvent;
        internal LaserEventArgs laserEventArgs;

        internal float AngularSpeed { get; set; }   // Degrees per second
        internal float AttackSpeed { get; set; }    // Missiles per second 
        internal int LaserCooldown { get; set; }
        internal int LaserCharges { get => laser_ChargeCount; set
            {
                laser_ChargeCount = value;
                laserEventArgs.Charges = value;
                laser_ChargeCountChanged = true;
            }
        }

        private bool missile_OnCooldown;
        private bool laser_OnCooldown;
        private bool laser_ChargeCountChanged;
        private int laser_ChargeCount;

        private Thread missile_ReloadThread;
        private Thread laser_ReloadThread;
        private Thread laser_ChargeUpThread;
        private Thread laser_EmitThread;
        private Laser laser;

        internal Player() : base()
        {
            SubscribeInputs();
            laserEventArgs = new LaserEventArgs { Charges = LaserCharges };

            Game.Instance.Visualizers
                .Select(x => x.Factory.CreatePlayer())
                .ForEach(p => Bind(p));

            ColliderRadius = 17f;
            MovementSpeed = 4.5f;            
            AngularSpeed = 4.5f;
            AttackSpeed = 4f;
            LaserCharges = 1;            

            missile_ReloadThread = new Thread(ReloadMissile) { Name = "Players weapon reload thread", IsBackground = true };
            missile_ReloadThread.Start();

            laser_ChargeUpThread = new Thread(ChargeUpLaser) { Name = "Players laser charge up thread", IsBackground = true };
            laser_ChargeUpThread.Start();

            laser_ReloadThread = new Thread(ReloadLaser) { Name = "Players laser reload thread", IsBackground = true };
            laser_ReloadThread.Start();
        }

        internal void OnFrame()
        {
            if (laser != null && !laser.IsEmitting)
            {
                laser.Destroy();
                laser = null;
            }
            else if (laser != null)
            {
                Game.Instance.asteroids
                    .Where(x => laser.CollidesWith(x))
                    .ForEach(x => x.DestroyByPlayer());
                Game.Instance.ufos
                    .Where(x => laser.CollidesWith(x))
                    .ForEach(x => x.DestroyByPlayer());
            }
            if (laser_ChargeCountChanged)
            {
                laser_ChargeCountChanged = false;
                LaserEvent?.Invoke(this, laserEventArgs);
            }
        }

        internal void RotateToLeft()
        {
            Rotate(RotateDirection.CounterClockwise);
        }

        internal void RotateToRight()
        {
            Rotate(RotateDirection.Clockwise);
        }

        internal override void MoveForward()
        {
            base.MoveForward();
            if (laser != null) laser.Position = Position;
        }

        internal void Fire()
        {
            if (!missile_OnCooldown)
            {
                new Missile() { Position = Position, Rotation = Rotation };
                missile_OnCooldown = true;
            }                
        }

        internal void Laser()
        {
            if (!laser_OnCooldown && LaserCharges > 0)
            {
                laser = new Laser() { Position = Position, Rotation = Rotation };
                LaserCharges--;
                laser_OnCooldown = true;
                laser_EmitThread = new Thread(EmitLaser) { Name = "Players laser emit thread", IsBackground = true };
                laser_EmitThread.Start();
            }
        }

        internal override void Destroy()
        {
            UnsubscribeInputs();

            LaserEvent?
                .GetInvocationList()
                .Cast<EventHandler<LaserEventArgs>>()
                .ForEach(x => LaserEvent -= x);

            base.Destroy();
        }

        protected void Bind(IPlayer player)
        {
            base.Bind(player);
            LaserEvent += player.OnLaserChargeCountChanged;
        }

        private void SubscribeInputs()
        {
            InputPublisher ip = Game.Instance.InputPublisher;
            ip.Fire += Fire;
            ip.Laser += Laser;
            ip.Forward += MoveForward;
            ip.ToLeft += RotateToLeft;
            ip.ToRight += RotateToRight;
        }

        private void UnsubscribeInputs()
        {
            InputPublisher ip = Game.Instance.InputPublisher;
            ip.Fire -= Fire;
            ip.Laser -= Laser;
            ip.Forward -= MoveForward;
            ip.ToLeft -= RotateToLeft;
            ip.ToRight -= RotateToRight;
        }

        private void Rotate(RotateDirection direction)
        {
            double radians = Math.Atan2(Rotation.Y, Rotation.X);
            float rotationAngle = AngularSpeed * (direction == RotateDirection.Clockwise ? -1 : 1);
            radians += rotationAngle * Math.PI / 180.0;
            RotationEventArgs.RotationAngle = rotationAngle;
            Rotation = new Vector2 { X = (float)Math.Cos(radians), Y = (float)Math.Sin(radians) };
            if (laser != null)
            {
                laser.RotationEventArgs.RotationAngle = RotationEventArgs.RotationAngle;
                laser.Rotation = Rotation;
            }
        }

        private void ReloadMissile()
        {
            int cooldown = (int)(1000f / AttackSpeed);
            while (!gameOver)            
                if (missile_OnCooldown)
                {                    
                    Thread.Sleep(cooldown);
                    missile_OnCooldown = false;
                }            
        }

        private void ReloadLaser()
        {
            while (!gameOver)            
                if (laser_OnCooldown)
                {
                    Thread.Sleep(laser_Cooldown);
                    laser_OnCooldown = false;
                }            
        }

        private void ChargeUpLaser()
        { 
            while (!gameOver)            
                if (LaserCharges < laser_MaxCharges)
                {
                    Thread.Sleep(laser_ChargeUpTime);
                    LaserCharges++;
                }            
        }

        private void EmitLaser()
        {
            Thread.Sleep(laser_DurationTime);
            laser.IsEmitting = false;
        }
    }
}
