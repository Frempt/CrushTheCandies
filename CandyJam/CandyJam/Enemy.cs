﻿/*
	File:			Enemy.cs
	Version:		1.2
	Last altered:	29/01/2014.
	Authors:		James MacGilp.
	
	Extends:		SpriteAnimated.cs
	
	Description:	- Encapsulates the functionality of the Enemy sprite object.
  
                    - Update() runs the basic AI routines for the Enemy. Default behaviour is to move from side to side until the Player is seen, at which point the enemy will move towards him.
	
					- SwapDirection() and GetNextPosition() are collision avoidance methods, and should be used to avoid enemies colliding with each other or leave the viewport's bounds.
					
					- EnemyAnimationState is used to set which animation should be playing. The UpdateAnimation() function uses this with the AnimationLibrary static class to play the correct animation.
  
                    - Further varieties of enemy could extend this and override the Update(Point, bool) function.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyJam
{
    class Enemy : SpriteAnimated
    {
        protected bool dead = false;
        protected float speed = 5.0f;

        public enum EnemyAnimationState{MOVING = 0, DYING = 1};
        protected EnemyAnimationState animationState = EnemyAnimationState.MOVING;

        public Enemy(Texture2D tex, int numberOfFrames, int numberOfAnimations)
        {
            texture = tex;

            frameNumber = 0;
            animationNumber = 0;
            frameTotal = numberOfFrames - 1;

            rect = new Rectangle(0, 0, tex.Width, tex.Height);

            velocity = new Vector2(0.0f, 0.0f);
        }

        public void Update(Point playerPosition, bool isPlayerGrounded)
        {
            if (IsGrounded() && isPlayerGrounded)
            {
                velocity.X = playerPosition.X - rect.Left;
                velocity.Normalize();
                velocity *= speed;
            }
        }

        public void SwapDirection()
        {
            velocity.X *= -1.0f;
        }

        public Rectangle GetNextPosition()
        {
            Rectangle next = new Rectangle(rect.X + (int)velocity.X, rect.Y + (int)velocity.Y, rect.Width, rect.Height);
            return next;
        }

        public void Die()
        {
            animationState = EnemyAnimationState.DYING;
        }

        public bool IsDead()
        {
            return dead;
        }

        public EnemyAnimationState GetAnimationState()
        {
            return animationState;
        }

        public void SetAnimationState(EnemyAnimationState state)
        {
            animationState = state;
        }

        public void UpdateAnimation()
        {
            if (animationState == EnemyAnimationState.MOVING)
            {
                AnimationLibrary.AnimationLooped(this, 5, 0);
            }

            if (animationState == EnemyAnimationState.DYING)
            {
                if(AnimationLibrary.AnimationSingleAndPause(this, 5, 1, 10))
                {
                    dead = true;
                }
            }
        }
    }
}
