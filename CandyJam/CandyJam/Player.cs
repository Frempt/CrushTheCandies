using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyJam
{
    class Player : SpriteAnimated
    {
        protected int lives = 5;
        protected Point gunPoint;
        protected float invulTimer = 0.0f;
        protected float invulDelay = 3000.0f;
        protected int enemiesKilled = 0;

        public enum AnimationState { IDLE = 0, RUNNING = 1, SHOOTING = 2 };
        protected AnimationState animState = AnimationState.IDLE;

        public Player(Texture2D tex, int numberOfFrames, int numberOfAnimations)
        {
            texture = tex;

            frameNumber = 0;
            animationNumber = 0;
            frameTotal = numberOfFrames - 1;

            rect = new Rectangle(0, 0, tex.Width / numberOfFrames, tex.Height / numberOfAnimations);

            velocity = new Vector2(0.0f, 0.0f);

            gunPoint = new Point(rect.Right, rect.Top + rect.Height / 3);

            effects = SpriteEffects.None;
        }

        public void Update()
        {
            if (effects == SpriteEffects.FlipHorizontally)
            {
                gunPoint = new Point(rect.Left, rect.Top + rect.Height / 3);
            }

            else
            {
                gunPoint = new Point(rect.Right, rect.Top + rect.Height / 3);
            }

            if (invulTimer > 0.0f)
            {
                invulTimer -= Main.DeltaTime.Milliseconds;

                if (invulTimer < 0.0f)
                {
                    invulTimer = 0.0f;
                }
            }
        }

        public Bullet Shoot(int mouseX, int mouseY)
        {
            SetAnimationState(Player.AnimationState.SHOOTING);
            ResetFrames();
            ResetTimer();
            SoundLibrary.PlayerShoot();

            Bullet bullet = new Bullet(TextureLibrary.bulletTexture, new Point(mouseX - rect.Width / 2, mouseY - rect.Width / 2), gunPoint);
            return bullet;
        }

        public void UpdateFacing(int mousePosX)
        {
            if (mousePosX <= rect.Center.X)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = SpriteEffects.None;
            }
        }

        public int GetLives()
        {
            return lives;
        }

        public void LoseLife()
        {
            lives--;
            invulTimer = invulDelay;
        }

        public void AddLife()
        {
            lives++;
        }

        public bool IsInvulnerable()
        {
            return invulTimer > 0.0f;
        }

        public float GetAlpha()
        {
            if (IsInvulnerable())
            {
                return 0.5f;
            }
            return 1.0f;
        }

        public void SetAnimationState(AnimationState state)
        {
            animState = state;
        }

        public AnimationState GetAnimationState()
        {
            return animState;
        }

        public Point GetGunPoint()
        {
            return gunPoint;
        }

        public void EnemyKilled()
        {
            enemiesKilled++;
        }

        public int GetEnemiesKilled()
        {
            return enemiesKilled;
        }

        public void ResetEnemiesKilled()
        {
            enemiesKilled = 0;
        }

        public void UpdateAnimation()
        {
            if (animState == AnimationState.IDLE)
            {
                SetAnimationNumber(0);
                frameNumber = 0;
            }
            if (animState == AnimationState.RUNNING)
            {
                SetAnimationNumber(0);
                AnimationLibrary.AnimationLooped(this, 10, 0);
            }
            if (animState == AnimationState.SHOOTING)
            {
                SetAnimationNumber(1);
                bool animDone = AnimationLibrary.AnimationSingle(this, 5, 1);
                if(animDone)
                {
                    animState = AnimationState.IDLE;
                }
            }
        }
    }
}
