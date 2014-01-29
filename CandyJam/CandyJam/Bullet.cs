using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyJam
{
    class Bullet : Sprite
    {
        protected int screenWidth;
        protected int screenHeight;
        protected float rotation = 0.0f;
        protected float spinSpeed = 0.005f;

        protected bool done;

        public Bullet(Texture2D tex, Point target, Point origin)
        {
            texture = tex;

            rect = new Rectangle(origin.X, origin.Y, tex.Width, tex.Height);

            velocity = new Vector2((float)target.X - origin.X, (float)target.Y - origin.Y);
            velocity.Normalize();
            velocity *= 50.0f;

            done = false;
        }

        public void Update()
        {
            MoveBy((int)velocity.X, (int)velocity.Y);

            if (velocity.X < 0.0f)
            {
                rotation += (Main.DeltaTime.Milliseconds * -spinSpeed);
            }
            else
            {
                rotation += (Main.DeltaTime.Milliseconds * spinSpeed);
            }

            if (rect.X >= screenWidth || rect.Y >= screenHeight)
            {
                done = true;
            }
        }

        public void SetScreenDimensions(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
        }

        public bool IsDone()
        {
            return done;
        }

        public float GetRotation()
        {
            return rotation;
        }
    }
}
