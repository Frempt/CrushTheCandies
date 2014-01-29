using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace CandyJam
{
    class SoundLibrary
    {
        private static ContentManager content;
        private static SoundEffect[] playerShoot;
        private static SoundEffect[] roar;

        private SoundLibrary()
        {
        }

        public static void Setup(ContentManager contentManager)
        {
            content = contentManager;

            playerShoot = LoadSound("player_shoot", 2);
            roar = LoadSound("roar", 1);
        }

        public static SoundEffect[] LoadSound(string name, int length)
        {
            SoundEffect[] sounds = new SoundEffect[length];
            for (int i = 0; i < length; i++)
            {
                sounds[i] = content.Load<SoundEffect>(name + i);
            } 
            return sounds;
        }

        public static void PlayerShoot()
        {
            Random rng = new Random();
            playerShoot[rng.Next(playerShoot.Length)].Play();
        }

        public static void Roar()
        {
            roar[0].Play();
        }
    }
}
