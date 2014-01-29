/*
	File:			TextureLibrary.cs
	Version:		1.0
	Last altered:	28/01/2014.
	Authors:		James MacGilp.

	Description:	- Used to load in textures which are statically accessable. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CandyJam
{
    class TextureLibrary
    {
        public static Texture2D playerTexture;
        public static Texture2D bulletTexture;
        public static Texture2D laserTexture;
        public static Texture2D platformTexture;
        public static Texture2D enemyTexture;
        public static Texture2D backgroundTexture;
        public static Texture2D pickupTexture;

        private TextureLibrary()
        {
        }

        public static void LoadTextures(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>("player");
            bulletTexture = content.Load<Texture2D>("bullet");
            laserTexture = content.Load<Texture2D>("laser");
            platformTexture = content.Load<Texture2D>("platform");
            enemyTexture = content.Load<Texture2D>("enemy");
            backgroundTexture = content.Load<Texture2D>("background");
            pickupTexture = content.Load<Texture2D>("pickup");
        }
    }
}
