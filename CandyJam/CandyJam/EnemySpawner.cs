using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyJam
{
    class EnemySpawner
    {
        protected static float enemySpawnTimer;
        protected static float enemySpawnDelay;
        protected static int maxEnemies;

        private EnemySpawner()
        {
        }

        public static void Setup()
        {
            enemySpawnTimer = 0.0f;
            enemySpawnDelay = 5000.0f;
            maxEnemies = 6;
        }

        public static List<Enemy> Update(List<Enemy> enemies, Viewport screen, int wave)
        {
            if (enemySpawnTimer >= (enemySpawnDelay / wave) && enemies.Count < maxEnemies)
            {
                enemySpawnTimer = 0.0f;
                Enemy enemy = new Enemy(TextureLibrary.enemyTexture, 4, 2);
                Random rng = new Random();
                int xPos = 0;
                Vector2 velocity = new Vector2(5.0f, 0.0f);
                if (rng.Next(1) == 1)
                {
                    xPos = screen.Width - enemy.GetRect().Width * 2;
                    velocity *= -1;
                }

                enemy.MoveTo(xPos, screen.Height / 2);
                enemy.SetVelocity(velocity);
                enemies.Add(enemy);
            }
            else if (enemySpawnTimer < enemySpawnDelay / wave)
            {
                enemySpawnTimer += Main.DeltaTime.Milliseconds;
            }
            return enemies;
        }
    }
}
