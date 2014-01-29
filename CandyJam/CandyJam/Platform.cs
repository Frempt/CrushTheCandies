using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyJam
{
    class Platform : Sprite
    {
        protected float maxLife = 5000.0f;
        protected float life;
        protected float timer = 0.0f;
        protected float delay = 4000.0f;
        protected bool isSolid = true;
        protected int segments = 1;

        public Platform(Texture2D tex, int widthInSegments)
        {
            texture = tex;

            segments = widthInSegments;

            rect = new Rectangle(0, 0, tex.Width * segments, tex.Height);

            velocity = new Vector2(0.0f, 0.0f);

            life = maxLife;
        }

        public void Update(Point playerPosition)
        {
            if (IsBelow(playerPosition) < 5)
            {
                if (isSolid)
                {
                    if (life > 0.0f)
                    {
                        life -= Main.DeltaTime.Milliseconds;
                    }
                    else
                    {
                        isSolid = false;
                    }
                }
            }
            else if(life < maxLife)
            {
                life += Main.DeltaTime.Milliseconds;

                if (life > maxLife) life = maxLife;
            }

            if (!isSolid && timer < delay)
            {
                timer += Main.DeltaTime.Milliseconds;
                if (timer >= delay)
                {
                    isSolid = true;
                    timer = 0.0f;
                    life = maxLife;
                }
            }
        }

        public bool IsSolid()
        {
            return isSolid;
        }

        public float GetAlpha()
        {
            return life / maxLife;
        }

        public int GetSegments()
        {
            return segments;
        }

        //returns distance between player and platform's y positions
        public int IsBelow(Point playerPosition)
        {
            if(playerPosition.X > rect.Left && playerPosition.X < rect.Right && playerPosition.Y < rect.Top)
            {
                return rect.Y - playerPosition.Y;
            }
            return 5000;
        }
    }
}
