using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CandyJam
{
    class AnimationLibrary
    {
        private AnimationLibrary()
        {
        }

        public static void AnimationLooped(SpriteAnimated graphic, int frameDelay, int animationNumber)
        {
            graphic.SetAnimationNumber(animationNumber);

            // if it's time to change frame
            if (graphic.GetTimer() >= frameDelay)
            {
                //change frame
                graphic.NextFrame(true);

                // reset the timer
                graphic.ResetTimer();
            }

            //increment the counter
            graphic.IncrementTimer(1);
        }

        public static bool AnimationSingle(SpriteAnimated graphic, int frameDelay, int animationNumber)
        {
            graphic.SetAnimationNumber(animationNumber);

            // if it's time to change frame
            if (graphic.GetTimer() >= frameDelay)
            {
                // reset the timer
                graphic.ResetTimer();

                //change frame
                return graphic.NextFrame(false);
            }

            //increment the counter
            graphic.IncrementTimer(1);

            return false;
        }

        public static bool AnimationSingleAndPause(SpriteAnimated graphic, int frameDelay, int animationNumber, int pause)
        {
            graphic.SetAnimationNumber(animationNumber);

            // if this frame is the last
            if (graphic.IsOnLastFrame())
            {
                if (graphic.GetTimer() >= pause)
                {
                    // reset the timer
                    graphic.ResetTimer();

                    //animation complete
                    return true;
                }
            }

            // if it's time to change frame
            else if (graphic.GetTimer() >= frameDelay)
            {
                //change frame
                graphic.NextFrame(false);

                // reset the timer
                graphic.ResetTimer();
            }

            //increment the counter
            graphic.IncrementTimer(1);

            //animation incomplete
            return false;
        }
    }
}
