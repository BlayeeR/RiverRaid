using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace River_Raid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Random enemySpawnChance = new Random();
        Plane player;
        int enemySpeed = 15;
        List<Plane> enemies = new List<Plane>();
        private TimeSpan lastEnemyTime;
        private KeyboardState oldState;
        private int oldDanger = 0, mapScale = 10, mapSpeed = 8;
        private SpriteFont font;
        private MapGenerator mapGenerator;
        private bool gameStarted = false, debugMode = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            mapGenerator = new MapGenerator(new Point(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), mapScale, mapSpeed);
            //margin = (int)(graphics.PreferredBackBufferWidth * 0.15);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Plane(this, new Vector2(0, 0), true);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("DefaultFont");
            LoadGame();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (state.IsKeyDown(Keys.Enter))
            {
                gameStarted = true;
            }
            if (gameStarted)
            {
                mapGenerator.Update(gameTime);
                if (state.IsKeyDown(Keys.Left))
                    //player.Position = new Vector2(player.Position.X - scale, GraphicsDevice.Viewport.Height - (player.Size.Y));
                    player.MoveLeft(mapScale);
                if (state.IsKeyDown(Keys.Right))
                    //player.Position = new Vector2(player.Position.X + scale, GraphicsDevice.Viewport.Height - (player.Size.Y));
                    player.MoveRight(mapScale);
                foreach (Plane enemy in enemies)
                    enemy.MoveForward(mapSpeed);
                
                

                //if (state.IsKeyDown(Keys.Up))
                //    player.MoveForward(scale);
                //if (state.IsKeyDown(Keys.Down))
                //    player.MoveBackward(scale);
                if (state.IsKeyDown(Keys.Space))
                {
                    if (gameTime.TotalGameTime.CompareTo(player.LastBulletTime) >= 0)
                    {
                        player.AddBullet();
                        player.LastBulletTime = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, 0, 0, 700));
                    }
                }
                for (int i = 0; i < player.Bullets.Count; i++)
                {
                    player.Bullets[i].Position = new Vector2(player.Bullets[i].Position.X, player.Bullets[i].Position.Y - player.Bullets[i].Speed);
                    if (enemies.Remove(enemies.Where(x => x.Intersects(player.Bullets[i].Rectangle)).FirstOrDefault()))
                        player.Bullets.Remove(player.Bullets[i]);
                    else if (player.Bullets[i].Position.Y + player.Bullets[i].Size.Y < 0)
                        player.Bullets.Remove(player.Bullets[i]);
                }
                if(mapGenerator.Intersects(player))
                    LoadGame();
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].Position.Y > GraphicsDevice.Viewport.Height)
                        enemies.Remove(enemies[i]);
                    else if (mapGenerator.Intersects(enemies[i]))
                        enemies.Remove(enemies[i]);
                }
                foreach (Plane enemy in enemies)
                {
                    int leftWall= mapGenerator.leftSide.Select(n => new { n, distance = Math.Abs(n.Top - enemy.Center.Y) }).OrderBy(p => p.distance).First().n.Right,
                        rightWall = mapGenerator.rightSide.Select(n => new { n, distance = Math.Abs(n.Top - enemy.Center.Y) }).OrderBy(p => p.distance).First().n.Left;
                    int danger = HelperClass.CalculateDanger(new Vector2(enemy.Center.X, enemy.Position.Y+enemy.Size.Y), player.Bullets, leftWall,rightWall);
                    int newDanger = HelperClass.CalculateDanger(new Vector2(enemy.Center.X + enemySpeed, enemy.Center.Y), player.Bullets, leftWall, rightWall);
                        if (danger < newDanger)
                        {
                            enemy.MoveLeft(enemySpeed);
                        }
                        else if (danger > newDanger)
                        {
                            enemy.MoveRight(enemySpeed);
                        }

                }
                
                if (enemies.Count < 4)
                {
                    if (gameTime.TotalGameTime.CompareTo(lastEnemyTime) >= 0)
                    {
                        enemies.Add(new Plane(this, new Vector2(enemySpawnChance.Next(mapGenerator.LastLeftRectangle.Right+60, mapGenerator.LastRightRectangle.Left-100), -60), false));//
                        lastEnemyTime = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, 0, 0, enemySpawnChance.Next(300, 1400)));
                    }
                }



            }
            oldState = state;
            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            mapGenerator.Draw(spriteBatch, debugMode);
            


            if (debugMode)
            {
                Texture2D tx = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                tx.SetData<Color>(new Color[] { Color.White });
                spriteBatch.Draw(tx, player.Rectangle, Color.Green);
                spriteBatch.Draw(tx, player.HorizontalHitbox, Color.White);
                spriteBatch.Draw(tx, player.VerticalHitbox, Color.White);
                foreach (Plane pl in enemies)
                {
                    spriteBatch.Draw(tx, pl.Rectangle, Color.Green);

                                spriteBatch.Draw(tx, pl.HorizontalHitbox, Color.White);
                    spriteBatch.Draw(tx, pl.VerticalHitbox, Color.White);
                }
            }
            foreach (Plane pl in enemies)
                spriteBatch.Draw(pl.Texture, pl.Rectangle, Color.White);
            spriteBatch.Draw(player.Texture, player.Rectangle, Color.White);

            foreach (Bullet bullet in player.Bullets)
                spriteBatch.Draw(bullet.Texture, bullet.Rectangle, Color.Yellow);
            //spriteBatch.Draw(HelperClass.GetTexture(spriteBatch), new Rectangle(new Point(0, 0), new Point(graphics.GraphicsDevice.Viewport.Width, 25)), Color.Red);
    
            int distance=0;
            foreach (Plane enemy in enemies)
            {
                foreach (Bullet bullet in player.Bullets)
                {
                    distance = (int)Math.Sqrt(Math.Pow(bullet.Position.X - enemy.Center.X, 2) + Math.Pow(bullet.Position.Y - enemy.Center.Y, 2));
                    if (distance /*> Math.Sqrt(Math.Pow(bullet.Position.X - enemy.Position.X, 2) + Math.Pow(bullet.Position.Y - enemy.Position.Y, 2))*/<300  && bullet.Position.Y > enemy.Center.Y)
                    {
                            HelperClass.DrawLine(spriteBatch, HelperClass.GetTexture(spriteBatch), new Vector2(enemy.Center.X, enemy.Position.Y+enemy.Size.Y), bullet.Position, new Color(244, distance, 66));
                        //distance = (int)Math.Sqrt(Math.Pow(bullet.Position.X - enemy.Position.X, 2) + Math.Pow(bullet.Position.Y - enemy.Position.Y, 2));
                    }
                    
                }
            }
            spriteBatch.End();
            // TODO: Add your drawing code here
            
            base.Draw(gameTime);
        }

        private void LoadGame()
        {
            gameStarted = false;
            mapGenerator = new MapGenerator(new Point(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), mapScale, mapSpeed);
            player.Bullets.Clear();
            enemies.Clear();
            // Convert the byte array to hexadecimal string
            //int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//graphics.GraphicsDevice.Viewport.Height;
            
            player.Position = new Vector2((GraphicsDevice.Viewport.Width / 2)-(player.Size.X/2), GraphicsDevice.Viewport.Height - (player.Size.Y));
        }
    }
}
