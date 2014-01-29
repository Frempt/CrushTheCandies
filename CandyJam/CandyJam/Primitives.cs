using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyJam
{
    class Primitives
    {
        private Primitives()
        {
        }

        public static void DrawLine(SpriteBatch spriteBatch, Point origin, Point target, float thickness)
        {
            Vector2 originVec = new Vector2((float)origin.X, (float)origin.Y);
            Vector2 targetVec = new Vector2((float)target.X, (float)target.Y);
            Vector2 direction = targetVec - originVec;

            float length = Vector2.Distance(originVec, targetVec);

            Rectangle rect = new Rectangle(origin.X, origin.Y, (int)length, (int) thickness);

            float angle = (float)Math.Atan2(direction.Y, direction.X);

            spriteBatch.Draw(TextureLibrary.laserTexture, rect, null, Color.White, angle, Vector2.Zero, SpriteEffects.None, 0.0f);
        }
    }
}
