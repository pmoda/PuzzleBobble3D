using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace PuzzleBobbleA2
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int SPRITE_TIMER = 100;
        const int SPRITE_SIZE = 32;
        public const int DIFFICULTY_RATING = 7;

        GraphicsDeviceManager graphics;
        GraphicsDevice graphics3D;

        SpriteBatch spriteBatch;

        //Declaring Variables
        GameController controller;

        private Model model;
        private Matrix Identity = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 30), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Vector3 position;
       
        //Sprite Texture
        Texture2D sprite;
        Texture2D pokeBalls;
        Texture2D arrow;
        Texture2D line;
        Texture2D ceiling;
        Texture2D gameOver;
        Texture2D mainMenu;
        Texture2D levelTile;
        Texture2D levelTile2;
        Texture2D green;
        int greenCurrentFrame;
        float greenTimer;
        Texture2D blue;
        int blueCurrentFrame;
        float blueTimer;

        protected SpriteFont font;

        Bubble loadedBall;
        //Bubble loadedBall = new Bubble();
        Bubble movingBall;
        //Bubble sackBall = new Bubble();
        Bubble sackBall;

        //A Timer variable
        float timer = 0f;
        //The interval (100 milliseconds)
        float interval = 0f;
        const float AUTO_FIRE = 6000;

        //A rectangle to store which 'frame' is currently being shown
        Rectangle sourceRect;
        //The centre of the current 'frame'

        Texture2D backgroundTexture;
        Song song;

        enum GameState
        {
            MainMenu,
            Options,
            Playing1,
            Playing2,
            Credits,
            GameOver,
        }
        GameState currentGameState = GameState.MainMenu;
        private Vector3 WorldPosition;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
       //     graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            controller = new GameController(1, graphics);
        }

        protected override void Initialize()
        {
            base.Initialize();

            Stats.reset();
            
            controller.setLevelTile(levelTile);
            if (currentGameState == GameState.Playing2)
                controller.setLevelTile(levelTile2);
            controller.sprites = sprite;
            controller.ballSprites = pokeBalls;
            controller.initializeLevel();

            sackBall = new Bubble((PuzzleBobbleA2.Bubble.Colors)controller.generateColor());
            sackBall.texture = pokeBalls;

            loadedBall = new Bubble((PuzzleBobbleA2.Bubble.Colors)controller.generateColor());
            loadedBall.texture = pokeBalls;
            loadedBall.positionPixels = new Vector2(306, 360);
            
            interval = 0f;
            timer = 0f;
            greenCurrentFrame  = 0;
            greenTimer = 0;
            blueCurrentFrame = 0;
            blueTimer = 0;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("untitled");

            mainMenu = Content.Load<Texture2D>("bubbobwallpaper");
            gameOver = Content.Load<Texture2D>("GameOver");
            font = Content.Load<SpriteFont>("Score"); 

            //Load the sprite texture (noticeable Gohan, a great character)
            backgroundTexture = Content.Load<Texture2D>("Background");
            song = Content.Load<Song>("10 The Ecstasy of Gold");
//            MediaPlayer.Play(song);
//            MediaPlayer.IsRepeating = true;

            sprite = Content.Load<Texture2D>("allSprites");
            green = Content.Load<Texture2D>("greenMan");
            blue = Content.Load<Texture2D>("blueMan");
            sourceRect = new Rectangle(0, 105, 150, 50);

            pokeBalls = Content.Load<Texture2D>("ballSprites");
            arrow = Content.Load<Texture2D>("arrow");
            line = Content.Load<Texture2D>("DeathLine");
            ceiling = Content.Load<Texture2D>("ceiling");
            levelTile = Content.Load<Texture2D>("gameBG");
            levelTile2 = Content.Load<Texture2D>("gameBG1");
        }
        
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();    

            if (currentGameState == GameState.Playing1 || currentGameState == GameState.Playing2)
            {
                //Increase the timer by the number of milliseconds since update was last called
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                interval += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    blueTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    controller.MoveArrowRight();
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    blueTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    controller.MoveArrowLeft();
                }
                if (keyboardState.IsKeyDown(Keys.Up) || interval >= AUTO_FIRE)
                {
                    movingBall = loadedBall;
                    if (loadedBall.velocity.Y == 0)
                    {
                        Stats.ballsFired++;
                        movingBall.velocity = 10 * controller.getArrowVector();
                    }
                    interval = 0;
                }

                if (movingBall != null)
                {
                    greenTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (greenTimer >= SPRITE_TIMER - 25)
                    {
                        greenCurrentFrame = (greenCurrentFrame+1);
                        if (greenCurrentFrame >= 4)
                            greenCurrentFrame = 3;
                        greenTimer = 0;
                    }
                    movingBall.Move();
                    if (controller.collides(movingBall))
                    {
                        greenTimer = 0;
                        greenCurrentFrame = 0;

                        movingBall.texture = pokeBalls;
                        controller.addBubble(movingBall);
                        controller.popBubbles(movingBall);
                        controller.fallBubbles();                         

                        loadedBall = sackBall;
                        loadedBall.positionPixels = new Vector2(306, 360);
                        loadedBall.worldPosition = new Vector3(0, -10, 0);

                        sackBall = new Bubble((PuzzleBobbleA2.Bubble.Colors)controller.generateColor());
                        movingBall = null;
                    }
                }
                //Check to see if we shoudl drop down one level
                if (Stats.ballsFired >= (controller.ceiling + 1) * (Stats.bubbleCount / 2 + (DIFFICULTY_RATING - controller.level)))
                {
                    controller.ceiling++;
                }

                //Check the timer is more than the chosen interval
                controller.Pop(gameTime);
                if (controller.GameOver())
                {
                    currentGameState = GameState.GameOver;
                    //this.Exit();
                }
                if (blueTimer >= SPRITE_TIMER)
                {
                    blueTimer = 0;
                    blueCurrentFrame = (blueCurrentFrame + 1) % 6;
                }

            }
            if (currentGameState == GameState.Playing2)
            {
                if (Stats.bubbleCount == 0 && controller.fallingBubbles.Count == 0 && controller.poppingBubbles.Count == 0)
                {
                    currentGameState = GameState.Credits;
                    //controller = new GameController(1);
                    //Initialize();
                }
                return;
            }
            if (currentGameState == GameState.Playing1)
            {
                if (Stats.bubbleCount == 0 && controller.fallingBubbles.Count == 0 && controller.poppingBubbles.Count == 0)
                {
                    currentGameState = GameState.Playing2;
                    controller = new GameController(2, graphics);
                    int oldscore = Stats.score;
                    Initialize();
                    Stats.score = oldscore;
                }
                return;
            }
            if (currentGameState == GameState.MainMenu)
            {
                KeyboardState keyboardState1 = Keyboard.GetState();
                if (keyboardState1.IsKeyDown(Keys.Space))
                {
                    currentGameState = GameState.Playing1;
                }
                return;
            }
            if (currentGameState == GameState.GameOver)
            {
                KeyboardState keyboardState1 = Keyboard.GetState();
                if (keyboardState1.IsKeyDown(Keys.Space))
                {
                    currentGameState = GameState.Playing1;
                    controller = new GameController(1, graphics);
                    Initialize();
                }
                return;
            }
            if (currentGameState == GameState.Credits)
            {
                KeyboardState keyboardState1 = Keyboard.GetState();
                if (keyboardState1.IsKeyDown(Keys.Space))
                {
                    currentGameState = GameState.MainMenu;
                    controller = new GameController(1, graphics);
                    Initialize();
                }
                return;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            if (currentGameState == GameState.MainMenu)
            {
                spriteBatch.Draw(mainMenu, new Vector2(0.0f, 0.0f), Color.White);
                spriteBatch.DrawString(font, "PRESS SPACE TO START", new Vector2(graphics.PreferredBackBufferWidth / 2 - 150, 25.0f), Color.Pink);
                spriteBatch.End();
                base.Draw(gameTime);
                return;
            }
            if (currentGameState == GameState.GameOver)
            {
                spriteBatch.Draw(gameOver, new Vector2(0.0f, 0.0f), Color.White);
                spriteBatch.DrawString(font, "PRESS SPACE TO TRY AGAIN", new Vector2(graphics.PreferredBackBufferWidth/2 - 192, 100.0f), Color.Blue);

                spriteBatch.End();
                base.Draw(gameTime);
                return;
            }
            if (currentGameState == GameState.Credits)
            {
                graphics.GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(font, "CONGRATULATIONS!!! Your final score is " + Stats.score, new Vector2(50, 100.0f), Color.White);

                spriteBatch.End();
                base.Draw(gameTime);
                return;
            }

            //DRAW ACTUAL GAME 

            spriteBatch.Draw(backgroundTexture, new Vector2(0.0f, 0.0f), Color.White);
    
            //Draw the background first
            controller.DrawTiles(spriteBatch);
            spriteBatch.Draw(ceiling, new Vector2(200, 0), new Rectangle(0, 0, 240, (controller.ceiling * 30)), Color.White);

            spriteBatch.Draw(line, new Vector2(200, 354), Color.White);
            spriteBatch.Draw(sprite, new Vector2(186, 358), sourceRect, Color.White, 0.0f, new Vector2(0, 0), 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(arrow, new Vector2(321, 378), null, Color.White, controller.arrowAngle, new Vector2(arrow.Width / 2, arrow.Height/*40*/), 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "SCORE: " + Stats.score.ToString(), new Vector2(0, graphics.PreferredBackBufferHeight -22), Color.Black);

            spriteBatch.Draw(green, new Vector2(260, 375), new Rectangle(32 * greenCurrentFrame,0 , SPRITE_SIZE, SPRITE_SIZE), Color.White, 0.0f, new Vector2(0, 0), 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(blue, new Vector2(355, 375), new Rectangle(35 * blueCurrentFrame, 0, SPRITE_SIZE, SPRITE_SIZE), Color.White, 0.0f, new Vector2(0,0), 1.25f, SpriteEffects.None, 0f);

            controller.displayGrid(spriteBatch);
         //   controller.testDisplay(spriteBatch, pokeBalls);

            spriteBatch.Draw(pokeBalls, loadedBall.positionPixels, loadedBall.getSourceRectangle(), Color.White);
            spriteBatch.Draw(pokeBalls, sackBall.positionPixels, sackBall.getSourceRectangle(), Color.White);


            if (movingBall != null)
            {
                spriteBatch.Draw(pokeBalls, movingBall.positionPixels, movingBall.getSourceRectangle(), Color.White);
            }
            

            spriteBatch.End();

            Vector3 WorldPositionSack = GraphicsDevice.Viewport.Unproject(new Vector3(sackBall.positionPixels.X, sackBall.positionPixels.Y, 100f), projection, view, Identity);
            Vector3 WorldPositionLoad = GraphicsDevice.Viewport.Unproject(new Vector3(loadedBall.positionPixels.X, loadedBall.positionPixels.Y, 0f), projection, view, Identity);

           loadedBall.DrawModel(model, WorldPositionLoad, view, projection);
            sackBall.DrawModel(model, WorldPositionSack, view, projection);
            controller.displayGrid3D(model, world, view, projection);

            base.Draw(gameTime);
        }
    }
}
