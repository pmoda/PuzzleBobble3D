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

        public const float BALL_SIZE = 1F;
        public const float RAMP_SIZE = 12F;
        public const float SIDE = 8f;
        const float INITIAL_VELOCITY = 0.61F;
        public float angle = 0;

        GraphicsDeviceManager graphics;
        GraphicsDevice graphics3D;

        SpriteBatch spriteBatch;

        //Declaring Variables
        GameController controller;

        private Model model;
        private Model ramp;
        private Model smallRamp;
        private Model arrow3D;
        private Model scoreText;
        private Model monkey;
        private Model cylinder;
        private Model forrest;

        private Matrix view = Matrix.CreateLookAt(new Vector3(0, -5, 25), new Vector3(0, 2f, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), 800f / 600f, 0.1f, 100f);
        DepthStencilState depthState;

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

        //Testing
        float viewDist = 50;
        float pane = 0;

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
            loadedBall.worldPosition = new Vector3(0, 1 - RAMP_SIZE, RAMP_SIZE - 2);
            
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

            depthState = new DepthStencilState();
            depthState.DepthBufferEnable = true; /* Enable the depth buffer */
            depthState.DepthBufferWriteEnable = true; /* When drawing to the screen, write to the depth buffer */

            GraphicsDevice.DepthStencilState = depthState;

            model = Content.Load<Model>("sphereTex");
            ramp = Content.Load<Model>("board");
            smallRamp = Content.Load<Model>("board1");
            arrow3D = Content.Load<Model>("arrow3D");
            scoreText = Content.Load<Model>("score3D");
            cylinder = Content.Load<Model>("line");
            monkey = Content.Load<Model>("monkey1");
            forrest = Content.Load<Model>("forrest");

            mainMenu = Content.Load<Texture2D>("bubbobwallpaper");
            gameOver = Content.Load<Texture2D>("GameOver");
            font = Content.Load<SpriteFont>("Score"); 

            //Load the sprite texture (noticeable Gohan, a great character)
            backgroundTexture = Content.Load<Texture2D>("Background");
            song = Content.Load<Song>("10 The Ecstasy of Gold");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

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
                if (keyboardState.IsKeyDown(Keys.Up) || interval >= AUTO_FIRE )
                {
                    movingBall = loadedBall;
                    if (loadedBall.velocity3D.Length() == 0)
                    {
                        Stats.ballsFired++;
                        movingBall.heading = (float)Math.Atan(controller.getArrowVector().X / controller.getArrowVector().Y);
                        movingBall.velocity3D = new Vector3(-INITIAL_VELOCITY * (float)Math.Sin(movingBall.heading), 0, -INITIAL_VELOCITY * (float)Math.Cos(movingBall.heading));
                    }
                    interval = 0;
                }                
                if (keyboardState.IsKeyDown(Keys.W))
                {
                   // view = Matrix.CreateLookAt(new Vector3(0, viewDist++, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                    angle = 0.005f;
                    view = view * Matrix.CreateRotationX(angle);
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                   // view = Matrix.CreateLookAt(new Vector3(0, viewDist--, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                    angle = -0.005f;
                    view = view * Matrix.CreateRotationX(angle);
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    view = Matrix.CreateLookAt(new Vector3(pane--, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    view = Matrix.CreateLookAt(new Vector3(pane++, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
                } 
                if (keyboardState.IsKeyDown(Keys.D1))
                {
                    view = Matrix.CreateLookAt(new Vector3(0, -11, 16), new Vector3(0, -10f, 15), Vector3.UnitY);
                } 
                if (keyboardState.IsKeyDown(Keys.D3))
                {
                    view = Matrix.CreateLookAt(new Vector3(0, -5, 25), new Vector3(0, 2f, 0), Vector3.UnitY);        
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
                    sackBall.Roll();
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
                        loadedBall.worldPosition = new Vector3(0, 1-RAMP_SIZE, RAMP_SIZE - 2);

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

            int count = 0;
            //CHECK FOR BUBBLE COUNT INCONSISTENCY
            foreach (Bubble b in controller.grid)
            {
                if (b != null)
                    count++;
            }
            if (Stats.bubbleCount != count)
            {
                Stats.bubbleCount = count;
            }

            if (currentGameState == GameState.Playing2)
            {
                if (count == 0 && controller.fallingBubbles.Count == 0 && controller.poppingBubbles.Count == 0)
                {
                    currentGameState = GameState.Credits;
                    //controller = new GameController(1);
                    //Initialize();
                }
                return;
            }
            if (currentGameState == GameState.Playing1)
            {


                if (count == 0 && controller.fallingBubbles.Count == 0 && controller.poppingBubbles.Count == 0)
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

            DrawRamp(ramp, view, projection);
             controller.DrawArrow(arrow3D, view, projection);
            Vector3 WorldPositionSack = GraphicsDevice.Viewport.Unproject(new Vector3(sackBall.positionPixels.X, sackBall.positionPixels.Y, 1), projection, view, Matrix.Identity);
            Vector3 WorldPositionLoad = GraphicsDevice.Viewport.Unproject(new Vector3(loadedBall.positionPixels.X, loadedBall.positionPixels.Y, 0), projection, view, Matrix.Identity);
            Vector3 WorldPositiontest = GraphicsDevice.Viewport.Unproject(new Vector3(0, 0, 0), projection, view, Matrix.Identity);
            Vector3 WorldPositiontestx = GraphicsDevice.Viewport.Unproject(new Vector3(600, 0, 0), projection, view, Matrix.Identity);
            Vector3 WorldPositiontesty = GraphicsDevice.Viewport.Unproject(new Vector3(0, 600, 0), projection, view, Matrix.Identity);

            loadedBall.DrawModel(model, WorldPositionLoad, view, projection, 0);
            sackBall.DrawModel(model, WorldPositionSack, view, projection, 0);
            controller.displayGrid3D(model, view, projection);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            if (currentGameState == GameState.MainMenu)
            {
                spriteBatch.Draw(mainMenu, new Vector2(0.0f, 0.0f), null,  Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "PRESS SPACE TO START", new Vector2(graphics.PreferredBackBufferWidth / 2 - 150, 25.0f), Color.Pink);
                spriteBatch.End();
                base.Draw(gameTime);
                return;
            }
            if (currentGameState == GameState.GameOver)
            {
                spriteBatch.Draw(gameOver, new Vector2(0.0f, 0.0f), null,  Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "PRESS SPACE TO TRY AGAIN", new Vector2(graphics.PreferredBackBufferWidth / 2 - 192, 100.0f), Color.Blue);

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
             spriteBatch.DrawString(font, "SCORE: " + Stats.score.ToString(), new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawRamp(Model model, Matrix view, Matrix projection)
        {
            //MAIN RAMP

            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix rampWorld = Matrix.CreateScale(RAMP_SIZE, RAMP_SIZE, RAMP_SIZE);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    // effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 1);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.4f, 0.2f, 0.0f);//doesnt consider all reflexion values
                    //Approximates to save memory
                  //  effect.EmissiveColor = new Vector3(0.4f, 0.2f, 0.0f);//(SUN)

                    effect.World = rampWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            //RIGHT WALL
            foreach (ModelMesh mesh in forrest.Meshes)
            {
                Matrix rampWorld = 
                    Matrix.CreateScale(RAMP_SIZE * 2, RAMP_SIZE *2.2f, RAMP_SIZE*2) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(90)) *
                    Matrix.CreateTranslation(RAMP_SIZE - 4, RAMP_SIZE, 0);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.8f, 0.0f);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.2f, 0.8f, 0.0f);//doesnt consider all reflexion values
                    //Approximates to save memory
                    //effect.EmissiveColor = new Vector3(1, 1, 1);//(SUN)

                    effect.World = rampWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            //LEFT WALL
            foreach (ModelMesh mesh in forrest.Meshes)
            {
                Matrix rampWorld =
                    Matrix.CreateScale(RAMP_SIZE * 2, RAMP_SIZE * 2.2f, RAMP_SIZE * 2) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(-90)) *
                    Matrix.CreateTranslation(-RAMP_SIZE + 4, RAMP_SIZE, 0);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                     effect.DirectionalLight0.DiffuseColor = new Vector3(0.2f, 0.8f, 0.0f);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.2f, 0.8f, 0.0f);//doesnt consider all reflexion values
                    //Approximates to save memory
                    //effect.EmissiveColor = new Vector3(1, 1, 1);//(SUN)

                    effect.World = rampWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            //BACK WALL
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix rampWorld =
                    Matrix.CreateScale(RAMP_SIZE, RAMP_SIZE * 4, RAMP_SIZE) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(180)) *
                    Matrix.CreateTranslation(0, 0, -RAMP_SIZE*2 + 2.9f  + controller.ceiling*((float)Math.Sqrt((Math.Pow(1.94f, 2f)/2))));

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                   effect.DirectionalLight0.DiffuseColor = new Vector3(0.4f, 0.2f, 0.2f);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.4f, 0.2f, 0.0f);//doesnt consider all reflexion values
                    //Approximates to save memory
                    //effect.EmissiveColor = new Vector3(0.4f, 0.2f, 0.0f);//(SUN)

                    effect.World = rampWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        
            //SCORE 
            foreach (ModelMesh mesh in scoreText.Meshes)
            {
                Matrix rampWorld =
                    Matrix.CreateScale(2,2,2) *
                   // Matrix.CreateRotationY(MathHelper.ToRadians(180)) *
                    Matrix.CreateTranslation(-RAMP_SIZE/2 , RAMP_SIZE + 2 , -RAMP_SIZE + controller.ceiling * 2);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    // effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);//doesnt consider all reflexion values
                    //Approximates to save memory
                    //effect.EmissiveColor = new Vector3(1, 1, 1);//(SUN)

                    effect.World = rampWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            //MONKEY1
            foreach (ModelMesh mesh in monkey.Meshes)
            {
                Matrix rampWorld =
                    Matrix.CreateScale(4, 4, 4) *
                    // Matrix.CreateRotationY(MathHelper.ToRadians(180)) *
                    Matrix.CreateTranslation(0, RAMP_SIZE + 4, -RAMP_SIZE + 5.5f + controller.ceiling * 2);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    // effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.5f , 0.38f , 0.18f);//doesnt consider all reflexion values
                    effect.EmissiveColor = new Vector3(0.4f, 0.4f, 0.1f);//(SUN)

                    effect.World = rampWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            //LINE
            foreach (ModelMesh mesh in cylinder.Meshes)
            {
                Matrix rampWorld =
                    Matrix.CreateScale(12, 0.1f, 0.1f) *
                    // Matrix.CreateRotationY(MathHelper.ToRadians(180)) *
                    Matrix.CreateTranslation(0, -6f, 8.5f);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    // effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 0.1f, 0.1f);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.8f, 0.08f, 0.08f);//doesnt consider all reflexion values
                    //Approximates to save memory
                    //effect.EmissiveColor = new Vector3(1, 1, 1);//(SUN)
                    effect.Alpha = 1.0f;
                    effect.World = rampWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            //LOADING RAMP
            foreach (ModelMesh mesh in smallRamp.Meshes)
            {
                Matrix ramp2World =
                    Matrix.CreateScale(1f, 1f, 4f) *
                     Matrix.CreateRotationZ(MathHelper.ToRadians(-45)) *
                     Matrix.CreateRotationY(MathHelper.ToRadians(90)) *
                    Matrix.CreateTranslation(-6, -9.2f, 10.5f);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    // effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, -1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.8f, 0.8f, 1.0f);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.8f, 0.8f, 1.0f);//doesnt consider all reflexion values
                    //Approximates to save memory
                    effect.EmissiveColor = new Vector3(1, 1, 1);//(SUN)
                    effect.Alpha = 0.15f;
                    effect.World = ramp2World;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
            
            DrawMonkey(monkey, view, projection);
        }

        public void DrawMonkey(Model model, Matrix view, Matrix projection){
            //MONKEY2
            for (int i = 0; i <= 40; i += 4)
            {
                foreach (ModelMesh mesh in monkey.Meshes)
                {
                    Matrix rampWorld =
                        Matrix.CreateScale(1, 1, 1) *
                         Matrix.CreateRotationY(MathHelper.ToRadians(-90)) *
                        Matrix.CreateTranslation(SIDE, RAMP_SIZE + SIDE / 2 - i + 4, -RAMP_SIZE + 2 + i);
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                         effect.DirectionalLight0.DiffuseColor = new Vector3(0.4f, 0.2f, 0.05f);//Light color (Moon)
                        effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                        effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.0f, 0.1f);// Shinnyness/reflexive
                        effect.AmbientLightColor = new Vector3(0.1f, 0.0f, 0.08f);//doesnt consider all reflexion values
                        //effect.EmissiveColor = new Vector3(1, 1, 1);//(SUN)

                        effect.World = rampWorld;
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
                //MONKEY3
                foreach (ModelMesh mesh in monkey.Meshes)
                {
                    Matrix rampWorld =
                        Matrix.CreateScale(1, 1, 1) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(90)) *
                        Matrix.CreateTranslation(-SIDE, RAMP_SIZE + SIDE / 2 - i + 4, -RAMP_SIZE + 2 + i);
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.DirectionalLight0.DiffuseColor = new Vector3(0.4f, 0.2f, 0.05f);//Light color (Moon)
                        effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                        effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.0f, 0.1f);// Shinnyness/reflexive
                        effect.AmbientLightColor = new Vector3(0.1f, 0.0f, 0.08f);//doesnt consider all reflexion values
                        //effect.EmissiveColor = new Vector3(1, 1, 1);//(SUN)

                        effect.World = rampWorld;
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
            }
        }
       
    }
}
