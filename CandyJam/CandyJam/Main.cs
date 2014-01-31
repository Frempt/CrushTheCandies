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
        public enum GameState{MENU = 0, INGAME = 1, GAMEOVER = 2};
        GameState gameState = GameState.MENU;

        public static TimeSpan DeltaTime;

        private int defaultGroundLevel;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private Sprite menuBackground;
        private Sprite background;
        private Player player;
        private SpritePhysics pickup;
        private List<Bullet> bullets;
        private List<Enemy> enemies;
        private List<Platform> platforms;

        private float waveDelayTimer = 0.0f;
        private float waveDelay = 10000.0f;
        private int wave = 1;
        private int enemyTarget = 10;
        private int initialSpawnCount = 2;

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

            defaultGroundLevel = GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 11;

            ResetLevel();
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

        protected void ResetLevel()
        {
            EnemySpawner.Setup();

            menuBackground = new Sprite(TextureLibrary.menuBackgroundTexture);
            menuBackground.SetRect(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            background = new Sprite(TextureLibrary.backgroundTexture);
            background.SetRect(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            player = new Player(TextureLibrary.playerTexture, 4, 3);
            player.MoveTo(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            bullets = new List<Bullet>();
            enemies = new List<Enemy>();
            platforms = new List<Platform>();

            EnemySpawner.WaveStart(enemies, GraphicsDevice.Viewport, wave, initialSpawnCount);

            //left platforms
            Platform platform = new Platform(TextureLibrary.platformTexture, 1);
            platform.MoveTo(GraphicsDevice.Viewport.Width / 10, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 4);
            platforms.Add(platform);

            Platform platform2 = new Platform(TextureLibrary.platformTexture, 1);
            platform2.MoveTo(GraphicsDevice.Viewport.Width / 10, GraphicsDevice.Viewport.Height / 4);
            platforms.Add(platform2);

            //left center platforms
            Platform platform3 = new Platform(TextureLibrary.platformTexture, 1);
            platform3.MoveTo(GraphicsDevice.Viewport.Width / 2 - GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 3);
            platforms.Add(platform3);

            Platform platform4 = new Platform(TextureLibrary.platformTexture, 1);
            platform4.MoveTo(GraphicsDevice.Viewport.Width / 2 - GraphicsDevice.Viewport.Width / 5, GraphicsDevice.Viewport.Height / 3);
            platforms.Add(platform4);

            //right platforms
            Platform platform5 = new Platform(TextureLibrary.platformTexture, 1);
            platform5.MoveTo((GraphicsDevice.Viewport.Width - GraphicsDevice.Viewport.Width / 10) - (platform5.GetRect().Width), GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 4);
            platforms.Add(platform5);

            Platform platform6 = new Platform(TextureLibrary.platformTexture, 1);
            platform6.MoveTo((GraphicsDevice.Viewport.Width - GraphicsDevice.Viewport.Width / 10) - (platform6.GetRect().Width), GraphicsDevice.Viewport.Height / 4);
            platforms.Add(platform6);

            //right center platforms
            Platform platform7 = new Platform(TextureLibrary.platformTexture, 1);
            platform7.MoveTo((GraphicsDevice.Viewport.Width / 2 + GraphicsDevice.Viewport.Width / 5) - (platform7.GetRect().Width), GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 3);
            platforms.Add(platform7);

            Platform platform8 = new Platform(TextureLibrary.platformTexture, 1);
            platform8.MoveTo((GraphicsDevice.Viewport.Width / 2 + GraphicsDevice.Viewport.Width / 5) - (platform8.GetRect().Width), GraphicsDevice.Viewport.Height / 3);
            platforms.Add(platform8);

            //center platform
            Platform platform9 = new Platform(TextureLibrary.platformTexture, 2);
            platform9.MoveTo((GraphicsDevice.Viewport.Width / 2) - (platform9.GetRect().Width / 2), GraphicsDevice.Viewport.Height / 2);
            platforms.Add(platform9);

            waveDelayTimer = 0.0f;
            wave = 1;
        }

        protected void UpdateWave()
        {
            //wave based logic
            if (player.GetEnemiesKilled() > (enemyTarget * wave))
            {
                if (wave >= 5)
                {
                    //win state reached
                }
                else
                {
                    wave++;
                    waveDelayTimer = waveDelay;
                    player.ResetEnemiesKilled();
                   
                    pickup = new SpritePhysics(TextureLibrary.pickupTexture);
                    pickup.MoveTo((GraphicsDevice.Viewport.Width / 2) - (pickup.GetRect().Width / 2), 0);
                }
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

                    enemies = EnemySpawner.WaveStart(enemies, GraphicsDevice.Viewport, wave, initialSpawnCount);
                }
            }
        }

        protected void Controls()
        {
            //controls
            KeyboardState key = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || key.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (gameState == GameState.MENU)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                {
                    gameState = GameState.INGAME;
                }
            }
            else if (gameState == GameState.INGAME)
            {
                if (player.IsAlive())
                {
                    //allows the player to jump
                    if ((key.IsKeyDown(Keys.Space) && previousKeys.IsKeyUp(Keys.Space)) || (key.IsKeyDown(Keys.W) && previousKeys.IsKeyUp(Keys.W)))
                    {
                        player.Jump();
                    }

                    if (key.IsKeyDown(Keys.S) && player.GetRect().Bottom != defaultGroundLevel - 1)
                    {
                        player.DropDown();
                    }

                    //set the animation state back to idle
                    if (player.GetAnimationState() == Player.PlayerAnimationState.RUNNING)
                    {
                        player.SetAnimationState(Player.PlayerAnimationState.IDLE);
                    }

                    //if the player isn't shooting, allows movement
                    if (key.IsKeyDown(Keys.A) && player.GetRect().Left > 10)
                    {
                        if (player.GetAnimationState() == Player.PlayerAnimationState.IDLE && player.IsGrounded()) player.SetAnimationState(Player.PlayerAnimationState.RUNNING);
                        player.MoveBy(-5, 0);
                    }

                    if (key.IsKeyDown(Keys.D) && player.GetRect().Right < GraphicsDevice.Viewport.Width - 10)
                    {
                        if (player.GetAnimationState() == Player.PlayerAnimationState.IDLE && player.IsGrounded()) player.SetAnimationState(Player.PlayerAnimationState.RUNNING);
                        player.MoveBy(5, 0);
                    }

                    //ensure the player faces the direction in which he aims
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        player.UpdateFacing(mouse.X);
                    }

                    //shoot if the player clicks
                    if (mouse.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed && player.GetAnimationState() != Player.PlayerAnimationState.SHOOTING)
                    {
                        bullets.Add(player.Shoot(mouse.X, mouse.Y));
                        bullets.Last<Bullet>().SetScreenDimensions(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                    }

                    //play the roar sound when left shift is pressed
                    if (key.IsKeyDown(Keys.LeftShift) && previousKeys.IsKeyUp(Keys.LeftShift))
                    {
                        SoundLibrary.Roar();
                    }
                }
            }

            //end of controls, set new previousMouseState
            previousMouseState = mouse;
            previousKeys = key;
        }

        protected override void Update(GameTime gameTime)
        {
            //set the statically accessable time last frame took to complete
            DeltaTime = gameTime.ElapsedGameTime;

            Controls();

            if (gameState == GameState.INGAME)
            {
                SoundLibrary.MusicStart();
                UpdateWave();

                List<Rectangle> geometry = new List<Rectangle>();

                Rectangle floorRect = new Rectangle(-GraphicsDevice.Viewport.Width, defaultGroundLevel, GraphicsDevice.Viewport.Width * 3, 5);
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
                if (player.IsAlive()) player.Update();
                player.UpdateAnimation();

                if (pickup != null)
                {
                    pickup.Physics(pickup.CalculateGroundLevel(geometry));
                    if (pickup.Collision(player.GetRect()))
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
                        SoundLibrary.AxeHit();
                        bullets.Remove(bullet);
                        bullet = null;
                    }
                }

                //updates the enemy objects
                for (int i = 0; i < enemies.Count; i++)
                {
                    Enemy enemy = enemies[i];

                    bool playerOnGround = (defaultGroundLevel - 1 == player.GetRect().Bottom);
                    enemy.Update(player.GetRect().Location, playerOnGround, player.IsInvulnerable(), player.IsAlive());
                    enemy.Physics(enemy.CalculateGroundLevel(geometry));
                    enemy.UpdateAnimation();

                    if (!enemy.IsDying())
                    {
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
                                if (enemies[j].Collision(nextPos) && !enemies[j].IsDying())
                                {
                                    enemy.SwapDirection();
                                    if (enemies[j].Collision(nextPos))
                                    {
                                        if (enemy.GetRect().Center.X < enemies[j].GetRect().Center.X)
                                        {
                                            enemy.MoveBy(-enemy.GetRect().Width / 2, 0);
                                        }
                                        else
                                        {
                                            enemy.MoveBy(enemy.GetRect().Width / 2, 0);
                                        }
                                    }
                                }
                            }
                        }

                        //checks if an enemy has been hit by a bullet
                        for (int j = 0; j < bullets.Count; j++)
                        {
                            if (!enemy.IsDying() && enemy.Collision(bullets[j].GetRect()))
                            {
                                bullets.Remove(bullets[j]);
                                player.EnemyKilled();
                                enemy.Die();
                            }
                        }

                        //check if the enemy has hit the player
                        if (enemy.Collision(player.GetRect()) && !player.IsInvulnerable())
                        {
                            player.LoseLife();
                            enemy.Die();
                        }
                    }
                    //if the enemy is dead, it is destroyed
                    if (enemy.IsDead())
                    {
                        enemies.Remove(enemy);
                        enemy = null;
                    }
                }

                if (player.IsDead())
                {
                    gameState = GameState.MENU;
                    ResetLevel();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (gameState == GameState.MENU)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(menuBackground.GetTexture(), menuBackground.GetRect(), Color.White);
                spriteBatch.End();
            }

            if (gameState == GameState.INGAME)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

                //draw background objects
                spriteBatch.Draw(background.GetTexture(), background.GetRect(), Color.White);

                //draw ingame objects
                foreach (Platform platform in platforms)
                {
                    spriteBatch.Draw(platform.GetTexture(), platform.GetRect(), new Rectangle(0, 0, platform.GetTexture().Width, platform.GetTexture().Height), Color.White * platform.GetAlpha());
                }

                spriteBatch.Draw(player.GetTexture(), player.GetRect(), player.GetSourceRect(), Color.White * player.GetAlpha(), 0.0f, Vector2.Zero, player.GetSpriteEffects(), 0.0f);

                foreach (Bullet bullet in bullets)
                {
                    spriteBatch.Draw(bullet.GetTexture(), bullet.GetRect(), null, Color.White, bullet.GetRotation(), Vector2.Zero, bullet.GetSpriteEffects(), 0.0f);
                }

                foreach (Enemy enemy in enemies)
                {
                    spriteBatch.Draw(enemy.GetTexture(), enemy.GetRect(), enemy.GetSourceRect(), Color.White, 0.0f, Vector2.Zero, enemy.GetSpriteEffects(), 0.0f);
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
                spriteBatch.DrawString(font, "Lives : " + player.GetLives(), new Vector2(10.0f, 10.0f), Color.Red, 0.0f, Vector2.Zero, (float)(GraphicsDevice.Viewport.Width / 600), SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Wave : " + wave, new Vector2(GraphicsDevice.Viewport.Width - 300.0f, 10.0f), Color.Red, 0.0f, Vector2.Zero, (float)(GraphicsDevice.Viewport.Width / 600), SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(font, "Enemies Remaining : " + ((enemyTarget * wave) - player.GetEnemiesKilled()), new Vector2(GraphicsDevice.Viewport.Width /2, 10.0f), Color.Red, 0.0f, Vector2.Zero, (float)(GraphicsDevice.Viewport.Width / 600), SpriteEffects.None, 0.0f);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}