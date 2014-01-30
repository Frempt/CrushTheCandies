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
            enemySpawnDelay = 1000.0f;
            maxEnemies = 6;
        }

        public static List<Enemy> Update(List<Enemy> enemies, Viewport screen, int wave)
        {
            if (enemySpawnTimer >= (enemySpawnDelay / wave) && enemies.Count < maxEnemies)
            {
                enemySpawnTimer = 0.0f;
                Enemy enemy = new Enemy(TextureLibrary.enemyTexture, 4, 2);
                Random rng = new Random();
                int xPos = rng.Next(0, screen.Width);

                Vector2 velocity = new Vector2(5.0f, 0.0f);
                if (xPos >= screen.Width/2)
                {
                    velocity *= -1;
                }

                enemy.MoveTo(xPos, 0);
                enemy.SetVelocity(velocity);
                enemies.Add(enemy);
            }
            else if (enemySpawnTimer < enemySpawnDelay / wave)
            {
                enemySpawnTimer += Main.DeltaTime.Milliseconds;
            }
            return enemies;
        }

        public static List<Enemy> WaveStart(List<Enemy> enemies, Viewport screen, int wave, int spawnSize)
        {
            for (int i = 0; i < (wave * spawnSize); i++)
            {
                Random rng = new Random();
                int xPos = rng.Next(0, screen.Width);

                Vector2 velocity = new Vector2(5.0f, 0.0f);
                if (xPos >= screen.Width / 2)
                {
                    velocity *= -1;
                }

                Enemy enemy = new Enemy(TextureLibrary.enemyTexture, 4, 2);
                enemy.MoveTo(xPos, 0);

                enemy.SetVelocity(velocity);
                enemies.Add(enemy);
            }

            return enemies;
        }
    }
}
