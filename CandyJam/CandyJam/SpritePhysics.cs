/*
	File:			SpritePhysics.cs
	Version:		1.1
	Last altered:	29/01/2014.
	Authors:		James MacGilp.
    Extends:        Sprite.cs 
 

	Description:	- Used to apply basic physics to a sprite object. 
	
					- Physics(int) and CalculateGroundLevel(List<Rectangle>) should be used in conjunction, passing a list of collidable geometry as a list of rectangles to return a groundlevel.
                    - It's important to remember to include the base (floor) groundlevel as a rectangle.
					
					- The functions Jump() and DropDown() are used to initiate a jump, or for the sprite to drop from the current groundlevel.
                    - It's important to NOT call DropDown() when the sprite is on the base (floor) groundlevel.
  
                    - This class should be extended by more specific classes.
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
                MoveTo(rect.Left, (groundLevel - rect.Height) - 1);
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
