using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CandyJam
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        public static TimeSpan DeltaTime;

        private int defaultGroundLevel;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private Sprite background;
        private Player player;
        private Sprite pickup;
        private List<Bullet> bullets;
        private List<Enemy> enemies;
        private List<Platform> platforms;

        private float waveDelayTimer = 0.0f;
        private float waveDelay = 10000.0f;
        private int wave = 1;
        private int enemyTarget = 10;

        private MouseState previousMouseState;
        private KeyboardState previousKeys;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 1024;

            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            EnemySpawner.Setup();

            defaultGroundLevel = GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 8;

            background = new Sprite(TextureLibrary.backgroundTexture);
            player = new Player(TextureLibrary.playerTexture, 4, 2);
            bullets = new List<Bullet>();
            enemies = new List<Enemy>();
            platforms = new List<Platform>();

            Platform platform = new Platform(TextureLibrary.platformTexture, 1);
            platform.MoveTo(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 4);
            platforms.Add(platform);

            Platform platform2 = new Platform(TextureLibrary.platformTexture, 2);
            platform2.MoveTo(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 2);
            platforms.Add(platform2);

            Platform platform3 = new Platform(TextureLibrary.platformTexture, 3);
            platform3.MoveTo(GraphicsDevice.Viewport.Width / 6, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 4);
            platforms.Add(platform3);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SoundLibrary.Setup(Content);
            TextureLibrary.LoadTextures(Content);


            font = Content.Load<SpriteFont>("font");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //set the statically accessable time last frame took to complete
            DeltaTime = gameTime.ElapsedGameTime;

            //controls
            KeyboardState key = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || key.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            //allows the player to jump
            if ((key.IsKeyDown(Keys.Space) && previousKeys.IsKeyUp(Keys.Space)) || (key.IsKeyDown(Keys.W) && previousKeys.IsKeyUp(Keys.W)))
            {
                player.Jump();
            }

            if (key.IsKeyDown(Keys.S) && player.GetRect().Bottom != defaultGroundLevel -1)
            {
                player.DropDown();
            }

            //set the animation state back to idle
            if (player.GetAnimationState() == Player.AnimationState.RUNNING)
            {
                player.SetAnimationState(Player.AnimationState.IDLE);
            }

            //if the player isn't shooting, allows movement
            if (key.IsKeyDown(Keys.A) && player.GetAnimationState() != Player.AnimationState.SHOOTING && mouse.LeftButton != ButtonState.Pressed)
            {
                player.SetAnimationState(Player.AnimationState.RUNNING);
                player.MoveBy(-5, 0);
            }

            if (key.IsKeyDown(Keys.D) && player.GetAnimationState() != Player.AnimationState.SHOOTING && mouse.LeftButton != ButtonState.Pressed)
            {
                player.SetAnimationState(Player.AnimationState.RUNNING);
                player.MoveBy(5, 0);
            }

            //ensure the player faces the direction in which he aims
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                player.UpdateFacing(mouse.X);
            }

            //shoot if the player clicks
            if (mouse.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed && player.GetAnimationState() != Player.AnimationState.SHOOTING)
            {
                bullets.Add(player.Shoot(mouse.X, mouse.Y));
                bullets.Last<Bullet>().SetScreenDimensions(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }

            //play the roar sound when left shift is pressed
            if (key.IsKeyDown(Keys.LeftShift) && previousKeys.IsKeyUp(Keys.LeftShift))
            {
                SoundLibrary.Roar();
            }

            //end of controls, set new previousMouseState
            previousMouseState = mouse;
            previousKeys = key;

            List<Rectangle> geometry = new List<Rectangle>();

            Rectangle floorRect = new Rectangle(-GraphicsDevice.Viewport.Width, defaultGroundLevel, GraphicsDevice.Viewport.Width*2, 1);
            geometry.Add(floorRect);

            //calculates the player's ground level
            for (int i = 0; i < platforms.Count; i++)
            {
                Platform platform = platforms[i];
                platform.Update(new Point(player.GetRect().Center.X, player.GetRect().Bottom - 1));

                if (platform.IsSolid())
                {
                    geometry.Add(platform.GetRect());
                }
            }

            //updates the player object
            player.Physics(player.CalculateGroundLevel(geometry));
            player.Update();
            player.UpdateAnimation();

            if (pickup != null)
            {
                if(pickup.Collision(player.GetRect()))
                {
                    player.AddLife();
                    pickup = null;
                }
            }

            //updates the bullet objects
            for (int i = 0; i < bullets.Count; i++)
            {
                Bullet bullet = bullets[i];
                bullet.Update();

                //if the bullet is done, it is destroyed
                if (bullet.IsDone())
                {
                    bullets.Remove(bullet);
                    bullet = null;
                }
            }

            if (player.GetEnemiesKilled() > (enemyTarget * wave))
            {
                wave++;
                waveDelayTimer = waveDelay;
                player.ResetEnemiesKilled();
                pickup = new Sprite(TextureLibrary.pickupTexture);
                pickup.MoveTo((GraphicsDevice.Viewport.Width / 2) + (pickup.GetRect().Width / 2), (GraphicsDevice.Viewport.Height / 2) + (pickup.GetRect().Height / 2));
            }

            if (waveDelayTimer == 0.0f)
            {
                enemies = EnemySpawner.Update(enemies, GraphicsDevice.Viewport, wave);
            }
            else
            {
                waveDelayTimer -= Main.DeltaTime.Milliseconds;

                if (waveDelayTimer <= 0.0f)
                {
                    waveDelayTimer = 0.0f;
                }
            }

            //updates the enemy objects
            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];

                bool playerOnGround = (defaultGroundLevel == player.GetRect().Bottom);
                enemy.Update(player.GetRect().Location, playerOnGround);
                enemy.Physics(defaultGroundLevel);

                Rectangle nextPos = enemy.GetNextPosition();

                if (nextPos.Left <= 0 || nextPos.Right >= GraphicsDevice.Viewport.Width)
                {
                    enemy.SwapDirection();

                    if (enemy.GetNextPosition().Left <= 0)
                    {
                        enemy.MoveTo(1, enemy.GetRect().Y);
                    }
                    if (enemy.GetNextPosition().Right >= GraphicsDevice.Viewport.Width)
                    {
                        enemy.MoveTo((GraphicsDevice.Viewport.Width - enemy.GetRect().Width) - 1, enemy.GetRect().Y);
                    }
                }

                //checks if an enemy has been hit by another enemy
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (enemy != enemies[j])
                    {
                        if (enemies[j].Collision(nextPos))
                        {
                            enemy.SwapDirection();
                        }
                    }
                }

                //checks if an enemy has been hit by a bullet
                for (int j = 0; j < bullets.Count; j++)
                {
                    if(enemy.Collision(bullets[j].GetRect()))
                    {
                        bullets.Remove(bullets[j]);
                        player.EnemyKilled();
                        enemy.Die();
                    }
                }

                //check if the enemy has hit the player
                if(enemy.Collision(player.GetRect()) && !player.IsInvulnerable())
                {
                    player.LoseLife();
                    enemy.Die();
                }

                //if the enemy is dead, it is destroyed
                if (enemy.IsDead())
                {
                    enemies.Remove(enemy);
                    enemy = null;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

            //draw background objects
            spriteBatch.Draw(background.GetTexture(), background.GetRect(), Color.White);

            //draw ingame objects
            foreach(Platform platform in platforms)
            {
                spriteBatch.Draw(platform.GetTexture(), platform.GetRect(), new Rectangle(0, 0, platform.GetTexture().Width, platform.GetTexture().Height), Color.White * platform.GetAlpha());
            }

            spriteBatch.Draw(player.GetTexture(), player.GetRect(), player.GetSourceRect(), Color.White * player.GetAlpha(), 0.0f, Vector2.Zero, player.GetSpriteEffects(), 0.0f);

            foreach (Bullet bullet in bullets)
            {
                spriteBatch.Draw(bullet.GetTexture(), bullet.GetRect(), null, Color.White, bullet.GetRotation(), Vector2.Zero, SpriteEffects.None, 0.0f);
            }

            foreach (Enemy enemy in enemies)
            {
                spriteBatch.Draw(enemy.GetTexture(), enemy.GetRect(), Color.White);
            }

            if (pickup != null)
            {
                spriteBatch.Draw(pickup.GetTexture(), pickup.GetRect(), Color.White);
            }

            //if the player is aiming, draw laser sight
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Primitives.DrawLine(spriteBatch, player.GetGunPoint(), new Point(previousMouseState.X, previousMouseState.Y), 2.0f);
            }

            //draw foreground objects

            //draw UI objects
            spriteBatch.DrawString(font, "Lives : " + player.GetLives(), new Vector2(10.0f, 10.0f), Color.Red, 0.0f, Vector2.Zero, (float)(GraphicsDevice.Viewport.Width/600), SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, "Wave : " + wave, new Vector2(GraphicsDevice.Viewport.Width - 300.0f, 10.0f), Color.Red, 0.0f, Vector2.Zero, (float)(GraphicsDevice.Viewport.Width / 600), SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, "Enemies Remaining : " + ((enemyTarget*wave) - player.GetEnemiesKilled()), new Vector2(GraphicsDevice.Viewport.Width - 300.0f, 100.0f), Color.Red, 0.0f, Vector2.Zero, (float)(GraphicsDevice.Viewport.Width / 600), SpriteEffects.None, 0.0f);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}