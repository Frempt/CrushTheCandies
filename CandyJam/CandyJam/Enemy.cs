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
            MoveBy((int)velocity.X, (int)velocity.Y);

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
            dead = true;
        }

        public bool IsDead()
        {
            return dead;
        }
    }
}
