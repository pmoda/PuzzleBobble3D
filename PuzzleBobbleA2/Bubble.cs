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
    class Bubble
    {
        const float SIZE = 32.0f;
        const float FALL_RATE = 8;
        const float GRAVITY = 0.0035F;
        //const float InitialVelocity = 30F;
        const float SIZE3 = 1.0f;
        const float FRICTION = 0.3F;
        const float RAMP_SIZE = 12F;

        const float POP_1 = 120;        
        const float POP_2 = 200;        
        const float POP_3 = 400;

        public enum Colors { Red = 0, Green = 1, Blue = 2, Yellow = 3, Grey = 4, Purple = 5, Orange = 6 };
        

        public Texture2D texture{ get; set; }
        public Model model { get; set; }
        public Vector3 colorVector { get; set; }
        public Vector3 velocity3D;
        public Vector3 worldPosition { get; set; }
        public float xPosition { get; set; }

        public Vector2 positionPixels{ get; set; } 
        public Vector2 velocity{ get; set; } 
        public Vector2 gridPosition{ get; set; }
        public float angularVelocity;
        public float angle;
        public float heading;
        public Vector2 acceleration;

        public Colors color{ get; set; }
        private Rectangle sourceRectangle { get; set; }

        public bool popping { get; set; }
        public bool popped { get; set; }
        private float popTimer;
        float popScale;
        float popAlpha;

        public bool falling { get; set; }
        public bool fallen { get; set; }
        private int fallTimer;

        public Bubble(Colors color){
            this.color = color;
            this.positionPixels = new Vector2(235, 382);
            popTimer = 0;
            popped = false;
            popping = false;
            popScale = 1;
            popAlpha = 1;
            fallTimer = 0;
            fallen = false;
            falling = false;
            sourceRectangle = getSourceRectangle();
            colorVector = getColorVector();
            worldPosition = new Vector3(-7, -11, 8);
            angularVelocity = 0;
            angle = 0;
            acceleration = new Vector2(0, 0);
        }
        public Bubble(Colors color, int x, int y, Texture2D tex)
        {
            this.texture = tex;
            this.color = color;
            this.gridPosition = new Vector2(x, y);
            this.positionPixels = this.gridToPixels(0);
            popTimer = 0;
            popped = false;
            popping = false;
            popScale = 1;
            popAlpha = 1;
            fallTimer = 0;
            fallen = false;
            falling = false;
            sourceRectangle = getSourceRectangle();
            colorVector = getColorVector();
            worldPosition = new Vector3(gridPosition.X * 2 - RAMP_SIZE + 5 + (gridPosition.Y % 2), -RAMP_SIZE + 1,
                                                gridPosition.Y * 1.94f - RAMP_SIZE * 1.5f /*, 
                                                bubble.gridPosition.Y - RAMP_SIZE + 1 0.0f*/);
        }
        public Bubble()
        {
            positionPixels = new Vector2(235, 382);
            popTimer = 0;
            popped = false;
            popping = false;
            popScale = 1;
            popAlpha = 1;
            fallTimer = 0;
            fallen = false;
            falling = false;
            sourceRectangle = getSourceRectangle();
        }

        public void Move()
        {
            if (worldPosition.X <= -RAMP_SIZE + 5 || worldPosition.X >= RAMP_SIZE - 5)
            {
                this.velocity3D = new Vector3(-velocity3D.X, 0, velocity3D.Z);
            }
                       
            velocity3D += new Vector3(0, 0, GRAVITY);
            heading = (float)Math.Atan(velocity3D.X / velocity3D.Z);
            if (velocity3D.Z >= 0)
            {
                angularVelocity = -velocity3D.Length();
            }
            else
            {
                angularVelocity = velocity3D.Length();
            }

            //worldPosition += new Vector3(-0.3f * (float)Math.Sin(heading), 0,  -0.30f * (float)Math.Cos(heading));
            worldPosition += velocity3D;
            angle += angularVelocity;

            angularVelocity = velocity3D.Length();

        }

        public void Roll()
        {
            Vector3 tempV = new Vector3(28 * GRAVITY, 0, 7 * GRAVITY);
            angularVelocity = tempV.Length();
            angle -= angularVelocity;

            heading = 90 + (float)Math.Atan( 7 * GRAVITY / 28 * GRAVITY );
            worldPosition += tempV;
        }

        public void Draw(SpriteBatch spriteBatch, int ceiling)
        {
            spriteBatch.Draw(this.texture, this.gridToPixels(ceiling) + new Vector2(0, fallTimer * FALL_RATE), this.sourceRectangle, Color.White);   
        }

        public void DrawModel(Model model, Vector3 WorldPosition, Matrix view, Matrix projection, int ceiling)
        {
          // WorldPosition = new Vector3(WorldPosition.X, WorldPosition.Y, 0);
         //   WorldPosition = new Vector3(gridPosition.X, gridPosition.Y, 0);
        //    Vector3 WorldPosition = GraphicsDevice.Viewport.Unproject(new Vector3(positionPixels.X, positionPixels.Y, 0) , view, projection, world);
            Matrix world =
                Matrix.CreateScale(popScale) *
                Matrix.CreateRotationX(-angle) *
                Matrix.CreateRotationY(heading) *
                Matrix.CreateTranslation(this.worldPosition) *
                Matrix.CreateTranslation(0, 0, ceiling * 1.94f) *
                Matrix.CreateTranslation(new Vector3(0, 12, -12)) *
                //Matrix.CreateRotationY(heading) *
                Matrix.CreateRotationX(MathHelper.ToRadians(45)) *
                Matrix.CreateTranslation(new Vector3(0, -12, 12))  ;
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    // effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);//Light color (Moon)
                    effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    effect.DirectionalLight0.SpecularColor = colorVector;// Shinnyness/reflexive
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);//doesnt consider all reflexion values
                    //Approximates to save memory
                    effect.EmissiveColor = colorVector;//(SUN)

                    effect.Alpha = this.popAlpha;                    
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        public Vector2 pixelsToGrid()
        {
            float y = this.positionPixels.Y;
            float x = this.positionPixels.X;

            y = y / 28;
            float remainderY = y - (int)y;
            if (remainderY >= 0.5)
            {
                y = (int)y + 1;
            }
            if (((int)y) % 2 == 1)
            {
                x -= 15;
                if (x == 7)
                {
                    x = 6;
                    y++;
                }
            }

            x -= 200;
            x = x / 30;
            float remainderX = x - (int)x;
            if (remainderX >= 0.5)
            {
                x = (int)x + 1;
            }

            return new Vector2((int)x, (int)y);
        }
        
        public Vector3 worldToGrid()
        {
            float y = this.worldPosition.Y;
            float x = ( this.worldPosition.X + RAMP_SIZE - 5 ) / 2;
            float z = (this.worldPosition.Z + RAMP_SIZE * 1.5f ) / 1.94f;

            float remainderY = z - (int)z;
            if (remainderY >= 0.5)
            {
                z = (int)z + 1;
            }
            if (((int)z) % 2 == 1)
            {
                x = (this.worldPosition.X + RAMP_SIZE - 6) / 2;
                if (x >= 7)
                {
                    x = 6;
                    z++;
                }
            }

            float remainderX = x - (int)x;
            if (remainderX >= 0.5)
            {
                x = (int)x + 1;
            }
            if (x < 0)
            {
                x = 0;
            }
            return new Vector3((int)x, (int)y, (int)z);
        }
        public Vector2 gridToPixels(int ceiling)
        {
            int x = (int)this.gridPosition.X;
            int y = (int)this.gridPosition.Y;
            Vector2 position = new Vector2(200, 0);

            float yPosition = 28 * y;
            float xPosition = 30 * x;

            if (y % 2 == 1)
            {
                xPosition += 15;                
            }
            position.X += xPosition;
            position.Y += (yPosition + ceiling * 30);
            this.positionPixels = position;
            
            return position;
        }

        public Vector2 getCenter()
        {
            return new Vector2(this.positionPixels.X + (SIZE / 2), this.positionPixels.Y + (SIZE / 2));
        }
        public Rectangle getSourceRectangle(){

            Rectangle rect = new Rectangle(0, 0, 0, 0);
            
            if (color == Colors.Red)
                rect = new Rectangle(0, 0, 30, 30);
            if (color == Colors.Yellow)
                rect = new Rectangle(30, 0, 30, 30);
            if (color == Colors.Blue)
                rect = new Rectangle(60, 0, 30, 30);
            if (color == Colors.Purple)
                rect = new Rectangle(90, 0, 30, 30);
            if (color == Colors.Green)
                rect = new Rectangle(90, 30, 30, 30);
            if (color == Colors.Orange)
                rect = new Rectangle(60, 60, 30, 30);
            if (color == Colors.Grey)
                rect = new Rectangle(0, 90, 30, 30);

            return rect;
        }

        public Vector3 getColorVector()
        {
            Vector3 v = new Vector3(0,0,0);

            if (color == Colors.Red)
                v = new Vector3(1, 0, 0);
            if (color == Colors.Yellow)
                v = new Vector3(1, 1, 0);
            if (color == Colors.Blue)
                v = new Vector3(0, 0, 1);
            if (color == Colors.Purple)
                v = new Vector3(0.5f, 0, 1);
            if (color == Colors.Green)
                v = new Vector3(0, 1, 0);
            if (color == Colors.Orange)
                v = new Vector3(0.6f, 0.25f, 0.05f);
            if (color == Colors.Grey)
                v = new Vector3(0.3f, 0.3f, 0.3f);

            return v;
        }

        public void pop(float timeElapsed)
        {
            popTimer += timeElapsed;

            //sourceRectangle = new Rectangle(10, 318, 30, 30);

            //if (popTimer > POP_1)
            //{
            //    this.positionPixels += new Vector2(-0.5f, -0.5f);
            //    sourceRectangle = new Rectangle(50, 312, 40, 40);
            //}
            //if (popTimer > POP_2)
            //{
            //    this.positionPixels += new Vector2(-0.5f, -0.5f);
            //    sourceRectangle = new Rectangle(95, 305, 50, 50);
            //}
           // if (popTimer > POP_3)
             //   popped = true;
            if (popAlpha <= 0)
                popped = true;

            popAlpha -= 0.05f;
            popScale += 0.1f;
        }

        public void fall()
        {
            if (this.worldPosition.Z > 25)
            {
                fallen = true;
                falling = false;                
            }
            Move();
        }
          
    }
}
