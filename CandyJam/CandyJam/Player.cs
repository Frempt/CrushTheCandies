﻿/*
	File:			Player.cs
	Version:		1.4
	Last altered:	February 2013.
	Authors:		James MacGilp.
	
	Extends:		SpriteAnimated.cs
	
	Description:	- Encapsulates the functionality of the player sprite object.
  
                    - Contains data on basic gameplay elements, such as lives, amount of enemies killed, and invulnerability (activated for a specified delay after damage is taken).
	
					- Shoot(int, int) creates a bullet aimed at the specified mouse position. The Gunpoint variable can be used to define where the bullets originate from the Player sprite.
					
					-  PlayerAnimationState is used to set which animation should be playing. The UpdateAnimation() function uses this with the AnimationLibrary static class to play the correct animation.
 */

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
        protected bool dead = false;

        public enum PlayerAnimationState { IDLE = 0, RUNNING = 1, SHOOTING = 2 , DYING = 3};
        protected PlayerAnimationState animationState = PlayerAnimationState.IDLE;

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
            SetAnimationState(Player.PlayerAnimationState.SHOOTING);
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

        new public void Jump()
        {
            base.Jump();
            SoundLibrary.Jump();
        }

        public int GetLives()
        {
            return lives;
        }

        public void LoseLife()
        {
            lives--;
            invulTimer = invulDelay;

            if (lives <= 0)
            {
                SoundLibrary.PlayerDie();
                SetAnimationState(PlayerAnimationState.DYING);
            }
            else
            {
                SoundLibrary.PlayerDamaged();
            }
        }

        public void AddLife()
        {
            if (lives < 5)
            {
                lives++;
            }
        }

        public bool IsInvulnerable()
        {
            return invulTimer > 0.0f;
        }

        public bool IsAlive()
        {
            return lives > 0;
        }

        public bool IsDead()
        {
            return dead;
        }

        public float GetAlpha()
        {
            if (IsInvulnerable())
            {
                return 0.5f;
            }
            return 1.0f;
        }

        public void SetAnimationState(PlayerAnimationState state)
        {
            animationState = state;
        }

        public PlayerAnimationState GetAnimationState()
        {
            return animationState;
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
            if (animationState == PlayerAnimationState.IDLE)
            {
                SetAnimationNumber(0);
                frameNumber = 0;
            }
            if (animationState == PlayerAnimationState.RUNNING)
            {
                AnimationLibrary.AnimationLooped(this, 5, 0);
            }
            if (animationState == PlayerAnimationState.SHOOTING)
            {
                bool animDone = AnimationLibrary.AnimationSingle(this, 3, 1);
                if(animDone)
                {
                    animationState = PlayerAnimationState.IDLE;
                }
            }
            if (animationState == PlayerAnimationState.DYING)
            {
                bool animDone = AnimationLibrary.AnimationSingleAndPause(this, 10, 2, 100);
                if (animDone)
                {
                    dead = true;
                }
            }
        }
    }
}
