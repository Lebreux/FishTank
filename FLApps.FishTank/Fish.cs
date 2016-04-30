using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FLApps.FishTank
{
    abstract class Fish
    {
        public static int FishSize = 15;
        public static int FishThickness = 5;
        public static int FishBellySize = 10;
        public static int FishMaxFood = 1000000;
        public static double ReproductionFoodPercent = .75;
        public static double ReproductionFoodUsage = .50;

        internal double X { get; set; }
        internal double Y { get; set; }
        internal double Angle { get; set; }
        internal double XVelocity { get; set; }
        internal double YVelocity { get; set; }
        internal int Food { get; set; }

        public Fish(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public abstract void Tick();
        public abstract Color GetLightColor();
        public abstract void SetEye(Color c);
        public abstract void SetProximity(int proximity);
        public abstract void SetFood(int food);
        public abstract void SetSmell(int smell);

        public SFML.Graphics.Drawable[] Render()
        {
            Color c = this.GetLightColor();
            SFML.Graphics.Color sfml_c = new SFML.Graphics.Color(c.R, c.G, c.B);

            // Body
            SFML.Graphics.CircleShape body = new SFML.Graphics.CircleShape();
            body.Radius = Fish.FishSize;
            body.Origin = new SFML.Window.Vector2f(body.Radius, body.Radius);
            body.Position = new SFML.Window.Vector2f((float)this.X, (float)this.Y);
            body.OutlineColor = sfml_c;
            body.OutlineThickness = Fish.FishThickness;
            body.FillColor = SFML.Graphics.Color.Transparent;

            // Head
            SFML.Graphics.RectangleShape head = new SFML.Graphics.RectangleShape();
            head.Size = new SFML.Window.Vector2f(Fish.FishSize + Fish.FishThickness * 2f, Fish.FishThickness);
            head.Origin = new SFML.Window.Vector2f(Fish.FishThickness, Fish.FishThickness / 2f);
            head.Position = new SFML.Window.Vector2f((float)this.X, (float)this.Y);
            head.Rotation = (float)this.Angle;
            head.FillColor = sfml_c;

            // Belly
            SFML.Graphics.CircleShape belly = new SFML.Graphics.CircleShape();
            belly.Radius = Fish.FishBellySize;
            belly.Origin = new SFML.Window.Vector2f(Fish.FishBellySize, Fish.FishBellySize);
            belly.Position = new SFML.Window.Vector2f((float)this.X, (float)this.Y);
            belly.FillColor = new SFML.Graphics.Color(
                (byte)((double)byte.MaxValue * (double)(Fish.FishMaxFood - this.Food) / (double)Fish.FishMaxFood), 
                (byte)((double)this.Food / (double)Fish.FishMaxFood), 
                0);

            // Fin
            SFML.Graphics.CircleShape fin = new SFML.Graphics.CircleShape(Fish.FishSize * 1.25f, 3);
            fin.Origin = new SFML.Window.Vector2f(fin.Radius, 0f);
            fin.Rotation = 90f + (float)this.Angle;
            fin.Position = new SFML.Window.Vector2f((float)this.X, (float)this.Y);
            fin.FillColor = sfml_c;

            return new SFML.Graphics.Drawable[] { body, belly, head, fin };
        }

        public abstract SFML.Graphics.Drawable[] RenderBrain(SFML.Graphics.FloatRect view);

        public abstract string GetGeneticCode();
        public abstract void SetGeneticCode(string geneticCode);
    }
}
