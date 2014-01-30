/*
	File:			SoundLibrary.cs
	Version:		1.1
	Last altered:	28/01/2014.
	Authors:		James MacGilp.

	Description:	- Used to statically play sound effects and music. 
	
					- All SoundEffects are stored in arrays, which are filled by calling LoadSound(string, int) and passing the filename (without numeration) and the amount of effects to be loaded.
                    - It's important to ensure there are that many sounds in the content folder under that filename (default format for naming is "filename1").
					
					- The functions for playing a sound choose one at random from the array. For instance, calling the static function PlayerShoot() will choose a sound from the playerShoot array and play it.
 */

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
        private static SoundEffect music;
        private static SoundEffectInstance musicInstance;
        private static SoundEffect[] playerShoot;
        private static SoundEffect[] roar;

        private SoundLibrary()
        {
        }

        public static void Setup(ContentManager contentManager)
        {
            content = contentManager;

            music = contentManager.Load<SoundEffect>("music");
            musicInstance = music.CreateInstance();

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

        public static void MusicStart()
        {
            if (musicInstance.State != SoundState.Playing)
            {
                musicInstance.Play();
            }
        }

        public static void MusicStop()
        {
            if (musicInstance.State == SoundState.Playing)
            {
                musicInstance.Stop();
            }
        }
    }
}
