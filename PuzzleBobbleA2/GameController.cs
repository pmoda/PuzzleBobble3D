using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleBobbleA2
{
    class GameController
    {
        public GraphicsDeviceManager Graphics;

        public const int BOARD_WIDTH = 240;
        public const int BOARD_HEIGHT = 408;
        public const int GRID_WIDTH = 8;
        public const int GRID_HEIGHT = 13;

        public const float BALL_SIZE = 1F;
        public const float RAMP_SIZE = 12F;

        Random randomSeed = new Random();

        public enum gridState { Empty, Popped, Occupied, Falling, X}
        public List<Bubble> fallingBubbles;
        public List<Bubble> poppingBubbles;

        public Bubble[,] grid = new Bubble[GRID_WIDTH, GRID_HEIGHT];
        public int level;
        public Texture2D levelTile { get; set; }

        public Model ballModel;

        public Texture2D ballSprites;
        public Texture2D sprites { get; set; }
        List<Texture2D> gameBoardTiles = new List<Texture2D>();

        public float arrowAngle { get; set; }
        public int ceiling = 0;

        public GameController(int level, GraphicsDeviceManager graph)
        {
            Graphics = graph;
            this.level = level;
            arrowAngle = 0;
            ceiling = 0;
            initializeLevel();
            poppingBubbles = new List<Bubble>();
            fallingBubbles = new List<Bubble>();
            //stats = new Stats();
        }

        public void initializeLevel()
        {
            if (this.level == 1)
            {                
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < (GRID_WIDTH - j%2); i++)
                    {
                        Bubble b1 = new Bubble();
                        if (j > 1)
                        {
                            if (i < 8)
                                b1 = new Bubble(Bubble.Colors.Green, i, j, ballSprites);
                            if (i < 6)
                                b1 = new Bubble(Bubble.Colors.Blue, i, j, ballSprites);
                            if (i < 4)
                                b1 = new Bubble(Bubble.Colors.Yellow, i, j, ballSprites);
                            if (i < 2)
                                b1 = new Bubble(Bubble.Colors.Red, i, j, ballSprites);
                        }
                        else
                        {
                            if (i < 8)
                                b1 = new Bubble(Bubble.Colors.Yellow, i, j, ballSprites);
                            if (i < 6)
                                b1 = new Bubble(Bubble.Colors.Red, i, j, ballSprites);
                            if (i < 4)
                                b1 = new Bubble(Bubble.Colors.Green, i, j, ballSprites);
                            if (i < 2)
                                b1 = new Bubble(Bubble.Colors.Blue, i, j, ballSprites);
                        }
                        addBubble(b1);
                    }
                }
            }
            if (this.level == 2)
            {
                for (int j = 0; j < 6; j++)
                {
                    for (int i = 1; i < (GRID_WIDTH -1); i++)
                    {
                        Bubble b1 = new Bubble();
                        if (j > 3)
                        {
                            if (i < 8)
                                b1 = new Bubble(Bubble.Colors.Red, i, j, ballSprites);
                            if (i < 6)
                                b1 = new Bubble(Bubble.Colors.Grey, i, j, ballSprites);
                            if (i < 4)
                                b1 = new Bubble(Bubble.Colors.Green, i, j, ballSprites);
                            if (i < 2)
                                b1 = new Bubble(Bubble.Colors.Blue, i, j, ballSprites);
                        }else if (j > 1)
                        {
                            if (i < 8)
                                b1 = new Bubble(Bubble.Colors.Red, i, j, ballSprites);
                            if (i < 6)
                                b1 = new Bubble(Bubble.Colors.Orange, i, j, ballSprites);
                            if (i < 4)
                                b1 = new Bubble(Bubble.Colors.Yellow, i, j, ballSprites);
                            if (i < 2)
                                b1 = new Bubble(Bubble.Colors.Blue, i, j, ballSprites);
                        }
                        else
                        {
                            if (i < 8)
                                b1 = new Bubble(Bubble.Colors.Blue, i, j, ballSprites);
                            if (i < 6)
                                b1 = new Bubble(Bubble.Colors.Purple, i, j, ballSprites);
                            if (i < 4)
                                b1 = new Bubble(Bubble.Colors.Green, i, j, ballSprites);
                            if (i < 2)
                                b1 = new Bubble(Bubble.Colors.Orange, i, j, ballSprites);
                        }
                        addBubble(b1);
                    }
                }
            }
        }

        public void setLevelTile(Texture2D levelTile){
            this.levelTile = levelTile;            
        }
        public void DrawTiles(SpriteBatch spriteBatch)
        {
            int xCount = 0;
            int yCount = 0;
            while (xCount <     BOARD_WIDTH)
            {
                while (yCount <= BOARD_HEIGHT)
                {
                    spriteBatch.Draw(levelTile, new Vector2(xCount + 200,yCount), Color.White);
                    yCount += levelTile.Height;
                }
                yCount = 0;
                xCount += levelTile.Width;
            }
        }

        public void addBubble(Bubble bubble)
        {
            bubble.velocity3D = new Vector3(0,0,0);
            bubble.worldPosition -= new Vector3(0, 0, ceiling * 1.94f);
            Vector2 gridCoordinates = new Vector2(bubble.worldToGrid().X, bubble.worldToGrid().Z);
            if ((int)gridCoordinates.Y % 2 == 1 && (int)gridCoordinates.X == GRID_WIDTH - 1)
            {
                if (grid[(int)gridCoordinates.X - 1, (int)gridCoordinates.Y] == null)
                    gridCoordinates.X -= 1;
                else if (grid[(int)gridCoordinates.X, (int)gridCoordinates.Y + 1] == null)
                    gridCoordinates.Y += 1;

            }
            bubble.heading = 0;
            bubble.angularVelocity = 0;
            bubble.gridPosition = gridCoordinates;
            bubble.positionPixels = bubble.gridToPixels(ceiling) + new Vector2(0, ceiling * 30);
            grid[(int)gridCoordinates.X, (int)gridCoordinates.Y] = bubble;
            bubble.worldPosition = new Vector3(bubble.gridPosition.X * 2 - RAMP_SIZE + 5 + ( bubble.gridPosition.Y % 2), -RAMP_SIZE + 1,
                                                bubble.gridPosition.Y * 1.94f - RAMP_SIZE * 1.5f /*, 
                                                bubble.gridPosition.Y - RAMP_SIZE + 1 0.0f*/);

            /*
             * TO DO:
             * 
             * FIGURE OUT WHY BUBBLE FLOAT WHEN CEILING MOVES DOWN
             * 
             */

            Stats.recordBubble(bubble);
        }
        public int generateColor()
        {
            int[] bubbles = new int[7];
            int count = 0;
            foreach (Bubble b in grid)
            {
                if (b != null)
                {
                    bubbles[(int)b.color]++;
                    count++;
                }
            }
            Stats.bubbleCount = count;
            Stats.bubbles = bubbles;

            if (Stats.bubbleCount == 0)
            {
                int randomNumFake = randomSeed.Next(0, 7);
               //Stats.bubbleCount = 1;
                return randomNumFake;
            }
            List<int> results = new List<int>(Stats.NUM_COLORS);

            for (int i = 0; i < Stats.NUM_COLORS; i++)
            {
                if (Stats.bubbleCount < 0)
                {
                    int randomNumFake = randomSeed.Next(0, 7);
                    Stats.bubbleCount = 1;
                    return randomNumFake;
                }
                float probability = 100 * Stats.bubbles[i] / Stats.bubbleCount;
                int randomNum = randomSeed.Next(0, (int)probability);
                results.Add(randomNum);
            }
            return results.IndexOf(results.Max());

        }

        public bool collides(Bubble shot)
        {
            foreach (Bubble b in grid)
            {
                if (b != null) 
                {
                    if (!b.popping && !b.falling)
                    {
                        float distance = Math.Abs(Vector3.Distance(shot.worldPosition,
                                    b.worldPosition + new Vector3(0, 0, 2 * ceiling)));
                        if (distance <= 2)
                            return true;
                    }
                }
            }
            if(shot.worldPosition.Z <= -RAMP_SIZE*1.5 + ceiling*1.94)
                return true;
            else
                return false;
        }

        public void MoveArrowLeft()
        {
            if(arrowAngle > -0.75)
            arrowAngle -= 0.02f;
        }
        public void MoveArrowRight()
        {
            if (arrowAngle < 0.75)
            arrowAngle += 0.02f;
        }
        public Vector2 getArrowVector()
        {
            return new Vector2((float)Math.Sin(arrowAngle), -(float)Math.Cos(arrowAngle));
        }

        public void displayGrid(SpriteBatch spriteBatch)
        {     
            foreach (Bubble bPop in poppingBubbles)
            {
                if (bPop != null)
                {
                    if (bPop.popped)
                    {
                        poppingBubbles = new List<Bubble>();
                        break;
                    }
                    else
                    {
                        bPop.Draw(spriteBatch, ceiling);
                    }
                }
            }
            foreach (Bubble bFall in fallingBubbles)
            {
                if (bFall != null)
                {
                    if (bFall.fallen)
                    {
                        fallingBubbles = new List<Bubble>();
                        break;
                    }
                    else
                    {
                        bFall.fall();
                        bFall.Draw(spriteBatch, ceiling);
                    }
                }
            }
            foreach (Bubble b in grid)
            {
                if (b != null)
                {    
                    b.Draw(spriteBatch, ceiling);
                }
            }
        }
        public void displayGrid3D(Model model, Matrix view, Matrix projection)
        {
            foreach (Bubble bPop in poppingBubbles)
            {
                if (bPop != null)
                {
                    if (bPop.popped)
                    {
                        poppingBubbles = new List<Bubble>();
                        break;
                    }
                    else
                    {
                        Vector3 WorldPosition = Vector3.Zero;
                        bPop.DrawModel(model, WorldPosition, view, projection, ceiling);
                    }
                }
            }
            foreach (Bubble bFall in fallingBubbles)
            {
                if (bFall != null)
                {
                    if (bFall.fallen)
                    {
                        fallingBubbles = new List<Bubble>();
                        break;
                    }
                    else
                    {
                        Vector3 WorldPosition = Vector3.Zero;
                        bFall.fall();
                        bFall.DrawModel(model, WorldPosition, view, projection, ceiling);
                    }
                }
            }
            foreach (Bubble b in grid)
            {
                if (b != null)
                {
                    b.angularVelocity += 0.01f;
                    Vector3 WorldPosition = Vector3.Zero;
                    //World position always goes to z of lookat so I cant see anything
                    b.DrawModel(model, WorldPosition, view, projection, ceiling);
                }
            }
        }

        public void Pop(GameTime gameTime)
        {
            float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (poppingBubbles.Count != 0)
            { 
                foreach (Bubble b in poppingBubbles)
                {
                        b.pop(timer);                
                }
            }
        }

        public bool popBubbles(Bubble addedBubble)
        {
            int[,] visitedGrid = new int[GRID_WIDTH, GRID_HEIGHT];
            List<Bubble> bubbles = new List<Bubble>();
            bubbles = adjacentBubbles(addedBubble, visitedGrid);
            if (bubbles.Count > 2)
            {
                foreach (Bubble b in bubbles)
                {
                    b.popping = true;
                    grid[(int)b.gridPosition.X, (int)b.gridPosition.Y].texture = sprites;
                    poppingBubbles.Add(grid[(int)b.gridPosition.X, (int)b.gridPosition.Y]);
                    Stats.removeBubble(grid[(int)b.gridPosition.X, (int)b.gridPosition.Y]);

                    grid[(int)b.gridPosition.X, (int)b.gridPosition.Y] = null;
                }
                return true;
            }
            return false;
        }

        public bool connectToTop(Bubble b, int[,] visited)
        {
            bool right  = false, left = false, topR = false, topL = false;

            if (b != null && visited[(int)b.gridPosition.X, (int)b.gridPosition.Y] != 1)
            {
                visited[(int)b.gridPosition.X, (int)b.gridPosition.Y] = 1;
                if (b.gridPosition.Y == 0)
                {
                    return true;
                }
                if (b.gridPosition.Y % 2 == 1)
                {
                    //Check TOP LEFT recursively
                    topL = connectToTop(grid[(int)b.gridPosition.X, (int)b.gridPosition.Y - 1], visited);
                    if (b.gridPosition.X + 1 < GRID_WIDTH)
                    {
                        //Check TOP RIGHT
                        topR = connectToTop(grid[(int)b.gridPosition.X + 1, (int)b.gridPosition.Y - 1], visited);
                        //Check RIGHT
                        right = connectToTop(grid[(int)b.gridPosition.X + 1, (int)b.gridPosition.Y], visited);
                    }
                    if (b.gridPosition.X - 1 >= 0)
                    {
                        //Check LEFT
                        left = connectToTop(grid[(int)b.gridPosition.X - 1, (int)b.gridPosition.Y], visited);
                    }
                }
                else if(b.gridPosition.Y % 2 == 0)
                {
                    //Check TOP RIGHT recursively
                    topR = connectToTop(grid[(int)b.gridPosition.X, (int)b.gridPosition.Y - 1], visited);
                    if (b.gridPosition.X - 1 >= 0)
                    {
                        //Check TOP LEFT
                        topL = connectToTop(grid[(int)b.gridPosition.X - 1, (int)b.gridPosition.Y - 1], visited);
                        //Check LEFT
                        left = connectToTop(grid[(int)b.gridPosition.X - 1, (int)b.gridPosition.Y], visited);
                    }
                    if (b.gridPosition.X + 1 < GRID_WIDTH)
                    {
                        //Check RIGHT
                        right = connectToTop(grid[(int)b.gridPosition.X + 1, (int)b.gridPosition.Y], visited);
                    }
                }

                return topL || topR || right || left;
            }
            else return false;
        }
        public void fallBubbles()
        {
            foreach (Bubble b in grid)
            {
                if (b != null) 
                {
                    int[,] visited = new int[GRID_WIDTH, GRID_HEIGHT];
                    if (!connectToTop(b, visited))
                    {
                        b.falling = true;
                        fallingBubbles.Add(b);
                        grid[(int)b.gridPosition.X, (int)b.gridPosition.Y] = null;
                    }
                }
            }
            if (fallingBubbles.Count != 0)
            {
                Stats.cascadeBubbles(fallingBubbles);
            }
        }

        private List<Bubble> adjacentBubbles(Bubble bubble, int[,] visitedGrid)
        {
            int x = (int)bubble.gridPosition.X;
            int y = (int)bubble.gridPosition.Y;
            List<Bubble> bubbles = new List<Bubble>();

            if (visitedGrid[x, y] == 1)
                return bubbles;
            
            visitedGrid[x, y] = 1;
            bubbles.Add(bubble);

            if (x > 0)
            {
                //check left
                if (grid[x - 1, y] != null)
                {
                    if (grid[x - 1, y].color == bubble.color)
                    {
                        bubbles.AddRange(adjacentBubbles(grid[x - 1, y], visitedGrid));
                    }
                }
                if (y > 0)
                {
                    //check top left
                    if (grid[x - ((y+1) % 2), y - 1] != null)
                    {
                        if (grid[x - ((y+1) % 2), y - 1].color == bubble.color)
                        {
                            bubbles.AddRange(adjacentBubbles(grid[x - ((y+1) % 2), y - 1], visitedGrid));
                        }
                    }
                }
                if (y < GRID_HEIGHT - 1)
                {
                    if (grid[x - ((y+1) % 2), y + 1] != null)
                    {
                        if (grid[x - ((y+1) % 2), y + 1].color == bubble.color)
                        {
                            bubbles.AddRange(adjacentBubbles(grid[x - ((y+1) % 2), y + 1], visitedGrid));
                        }
                    }
                }
            }

            if (x == 0 && (y % 2 == 1))
            {
                if (y > 0)
                {
                    //check top left
                    if (grid[x, y - 1] != null)
                    {
                        if (grid[x, y - 1].color == bubble.color)
                        {
                            bubbles.AddRange(adjacentBubbles(grid[x, y - 1], visitedGrid));
                        }
                    }
                }
                if (y < GRID_HEIGHT - 1)
                {
                    if (grid[x, y + 1] != null)
                    {
                        if (grid[x, y + 1].color == bubble.color)
                        {
                            bubbles.AddRange(adjacentBubbles(grid[x , y + 1], visitedGrid));
                        }
                    }
                }
            }   

            if (x < GRID_WIDTH-1)
            {
                //check right
                if (grid[x + 1, y] != null)
                {
                    if (grid[x + 1, y].color == bubble.color)
                    {
                        bubbles.AddRange(adjacentBubbles(grid[x + 1, y], visitedGrid));
                    }
                }
                if (bubble.gridPosition.Y > 0)
                {
                    //check top right
                    if (grid[x + y%2, y - 1] != null)
                    {
                        if (grid[x + y % 2, y - 1].color == bubble.color)
                        {
                            bubbles.AddRange(adjacentBubbles(grid[x + y%2, y - 1], visitedGrid));
                        }
                    }
                }
                if (bubble.gridPosition.Y < GRID_HEIGHT - 1)
                {
                    //bottom right
                    if (grid[x + y%2, y + 1] != null)
                    {
                        if (grid[x + y%2, y + 1].color == bubble.color)
                        {
                            bubbles.AddRange(adjacentBubbles(grid[x + y%2, y + 1], visitedGrid));
                        }
                    }
                }
            }

            return bubbles;
        }

        public bool Win()
        {
            bool winner = false;
            foreach (Bubble b in grid)
            {
                if (b != null)
                {
                    winner = true;
                }
            }
            return winner;            
        }

        public bool GameOver()
        {
            int lowestPoint = GRID_HEIGHT - 1 - ceiling;
            bool looser = false;
            for (int i = 0; i < GRID_WIDTH; i++)
            {
                if (grid[i, lowestPoint] != null)
                    looser = true;
            }
            if (ceiling >= 12) { looser = true; }
            return looser;
        }

        public void DrawArrow(Model model, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix arrowWorld =
                    Matrix.CreateRotationZ(-arrowAngle) *
                    Matrix.CreateScale(1, 1, 2) *
                    Matrix.CreateTranslation(0, 0, 4)*
                    Matrix.CreateRotationY(-arrowAngle - MathHelper.ToRadians(180)) *                        
                    Matrix.CreateTranslation(0, 0, -4)*
                    Matrix.CreateRotationX(MathHelper.ToRadians(45)) *
                    Matrix.CreateTranslation(0, -13, 15);

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                     effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 0);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, -1, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 0);// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.1f);//doesnt consider all reflexion values
                    //effect.EmissiveColor = new Vector3(1, 1, 0);//(SUN)
                    effect.Alpha = 0.5f;
                    effect.World = arrowWorld;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }

}
