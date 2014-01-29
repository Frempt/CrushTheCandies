/*
	File:			SpritePhysics.cs
	Version:		1.0
	Last altered:	29/01/2014.
	Authors:		James MacGilp.

	Description:	- Used to animate graphics via spritesheets. 
	
					- There are functions for both looped and single animations, both of which take a graphic to be animated, 
					  a rate at which to animate, and which animation to be played.
					
					- The function for a single animation returns a boolean, which is true if the animation is complete, 
					  and false if it is not.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyJam
{
    class SpritePhysics : Sprite
    {
        protected bool grounded = false;
        protected float jumpVelocity = 20.0f;

        public SpritePhysics()
        {
        }

        public SpritePhysics(Texture2D tex)
        {
            texture = tex;

            rect = new Rectangle(0, 0, tex.Width, tex.Height);

            velocity = new Vector2(0.0f, 0.0f);
        }

        public void Physics(int groundLevel)
        {
            this.MoveBy((int)velocity.X, (int)velocity.Y);

            if (rect.Bottom < groundLevel)
            {
                velocity.Y += 1.0f;
            }
            else
            {
                velocity.Y = 0.0f;
                MoveTo(rect.Left, groundLevel - rect.Height - 1);
                grounded = true;
            }
        }

        public int CalculateGroundLevel(List<Rectangle> geometry)
        {
            int groundLevel = 0;
            int distance = 4999;

            for (int i = 0; i < geometry.Count; i++)
            {
                Rectangle geoRect = geometry[i];

                int newDistance = 5000;

                if (rect.Center.X > geoRect.Left && rect.Center.X < geoRect.Right && rect.Bottom < geoRect.Top)
                {
                    newDistance = geoRect.Y - rect.Bottom;
                }

                if (newDistance < distance)
                {
                    distance = newDistance;
                    groundLevel = geoRect.Y;
                }
            }

            return groundLevel;
        }

        public void Jump()
        {
            if (grounded)
            {
                velocity.Y = -jumpVelocity;
                grounded = false;
            }
        }

        public void DropDown()
        {
            if (grounded)
            {
                MoveBy(0, 10);
                grounded = false;
            }
        }

        public bool IsGrounded()
        {
            return grounded;
        }
    }
}
