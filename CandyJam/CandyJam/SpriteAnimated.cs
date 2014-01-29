/*
	File:			SpriteAnimated.cs
	Version:		1.1
	Last altered:	29/01/2014.
	Authors:		James MacGilp.
	Extends: 		SpritePhysics.cs

	Description:	- Extends a sprite object to allow animation via spritesheet. 
	
					- Adds several integers to control the rate of animation, number of frames, number of animations, etc.
					
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
    class SpriteAnimated : SpritePhysics
    {
        protected int frameNumber;
        protected int frameTotal;
        protected int animationNumber;

        protected int timer;

        public SpriteAnimated()
        {
        }

        public SpriteAnimated(Texture2D tex, int numberOfFrames, int numberOfAnimations)
        {
            texture = tex;

            frameNumber = 0;
            animationNumber = 0;
            frameTotal = numberOfFrames - 1;
            //if (frameTotal < 1) frameTotal = 1;
            rect = new Rectangle(0, 0, tex.Width/numberOfFrames, tex.Height/numberOfAnimations);

            velocity = new Vector2(0.0f, 0.0f);

            effects = SpriteEffects.None;
        }

        public Rectangle GetSourceRect()
        {
            int left = frameNumber * rect.Width;
            int top = animationNumber * rect.Height;
            Rectangle sourceRect = new Rectangle(left, top, rect.Width, rect.Height);
            return sourceRect;
        }

        public void ResetFrames()
        {
            frameNumber = 0;
        }

        public void SetAnimationNumber(int number)
        {
            animationNumber = number;
        }

        public bool IsOnLastFrame()
        {
            return frameNumber == frameTotal;
        }

        public int GetTimer()
        {
            return timer;
        }

        public void IncrementTimer(int value)
        {
            timer += value;
        }

        public void ResetTimer()
        {
            timer = 0;
        }

        public bool NextFrame(bool loop)
        {
            if (frameNumber < frameTotal)
            {
                // move to the next frame
                frameNumber++;

                return false;
            }
            // if the frame is the final frame of the animation and is looped
            else if (loop)
            {
                // move back to the start of the animation
                frameNumber = 0;

                return false;
            }

            // if the frame is the final frame of the animation and is not looped
            else
            {
                return true;
            }
        }
    }
}
