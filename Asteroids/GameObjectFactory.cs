using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    // todo check all float values!!!! bounty radius etc
    internal class GameObjectFactory
    {
        private Game Game { get; }

        internal GameObjectFactory(Game game)
        {
            Game = game;
        }

        internal GameObject CreateAsteroid(AsteroidSize size, Vector2 position, Vector2 rotation)
        {
            EnemyClass @class = EnemyClass.Undefined;
            float movementSpeed = 0f;
            float colliderRadius = 0f;
            int bounty = 0;
            
            switch (size)
            {
                case AsteroidSize.Big:
                    @class = EnemyClass.BigAsteroid;
                    movementSpeed = 1f;
                    colliderRadius = 40f;
                    bounty = 10;
                    break;
                case AsteroidSize.Medium:
                    @class = EnemyClass.MediumAsteroid;
                    movementSpeed = 2f;
                    colliderRadius = 25f;
                    bounty = 8;
                    break;
                case AsteroidSize.Small:
                    @class = EnemyClass.SmallAsteroid;
                    movementSpeed = 2.7f;
                    colliderRadius = 15f;
                    bounty = 6;
                    break;
            }           
            
            GameObject asteroid = new GameObject(Game) { Tag = "Enemy", Position = position, Rotation = rotation };
                new RelocateOnOutOfBorder(asteroid);
                new MoveForward(asteroid) { MovementSpeed = movementSpeed };
                new Collider(asteroid) { ColliderRadius = colliderRadius };
                new Enemy(asteroid) { EnemyClass = @class, Bounty = bounty };

            Game.Visualizers
                .Select(x => x.Factory.CreateAsteroid(size))
                .ForEach(x => asteroid.Bind(x));

            return asteroid;
        }

        internal GameObject CreateUFO(Vector2 position, GameObject pursued)
        {
            GameObject ufo = new GameObject(Game) { Position = position, Tag = "Enemy" };
            new Pursuit(ufo, pursued) { MovementSpeed = 2.5f };
            new Collider(ufo) { ColliderRadius = 24f };
            new Enemy(ufo) { EnemyClass = EnemyClass.UFO, Bounty = 15 };

            Game.Visualizers
                .Select(x => x.Factory.CreateUFO())
                .ForEach(x => ufo.Bind(x));

            return ufo;
        }

        internal GameObject CreatePlayer(Vector2 position, Vector2 rotation)
        {
            Action<GameObject> destroyOnCollisionWithEnemy = go => { if (go.Tag == "Enemy") Game.FinishGame(); };
            GameObject player = new GameObject(Game) { Tag = "Player", Position = Game.Viewport.Center, Rotation = Vector2.DefaultRotation };
                new DelayedTaskManager(player);
                new PlayerController(player) { AngularSpeed = 4.5f, MovementSpeed = 4.5f };
                new Collider(player, destroyOnCollisionWithEnemy) { ColliderRadius = 19f };
                new RelocateOnOutOfBorder(player);
                new PrimaryWeapon(player) { AttackSpeed = 4f };
                var laser = new SecondaryWeapon(player);

            Game.Visualizers
                .Select(x => x.Factory.CreatePlayer())
                .ForEach(x => player.Bind(x));

            Game.Visualizers.ForEach(x => laser.ChargesCountChanged += x.OnLaserCountChanged);
            laser.Charges = 1;

            return player;               
        }

        internal GameObject CreateEnemySpawner()
        {
            GameObject enemySpawner = new GameObject(Game) { Tag = "Enemy spawner" };
                new DelayedTaskManager(enemySpawner);
                new EnemySpawner(enemySpawner);

            return enemySpawner;
        }

        internal GameObject CreateMissile(Vector2 position, Vector2 rotation)
        {
            GameObject missile = new GameObject(Game) { Position = position, Rotation = rotation, Tag = "Missile" };
            Action<GameObject> onCollision = hit => { if (hit.Tag == "Enemy") hit.GetBehavior<Enemy>().DestroyBy(missile); };
                new Collider(missile, onCollision) { ColliderRadius = 5f };
                new MoveForward(missile) { MovementSpeed = 12f };
                new DestroyOnOutOfBorder(missile);

            Game.Visualizers
                .Select(x => x.Factory.CreateMissile())
                .ForEach(x => missile.Bind(x));

            missile.Rotation = rotation;
            return missile;
        }

        internal GameObject CreateLaser(GameObject owner)
        {
            GameObject laser = new GameObject(Game) { Tag = "Laser" };
            new Laser(laser);
            new BelongsTo(laser) { Parent = owner };

            Game.Visualizers
                .Select(x => x.Factory.CreateLaser())
                .ForEach(x => laser.Bind(x));

            return laser;
        }
    }
}
