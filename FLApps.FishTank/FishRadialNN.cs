using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLApps.NeuralNetwork;
using System.Drawing;

namespace FLApps.FishTank
{
    internal class FishRadialNN : Fish
    {
        private static int MaxRotation = 5;
        private static int MaxSpeed = 2;
        private static int MaxProximity = 250;

        private RadialNode _lightR;
        private RadialNode _lightG;
        private RadialNode _lightB;
        private RadialNode _eyeR;
        private RadialNode _eyeG;
        private RadialNode _eyeB;
        private RadialNode _speed;
        private RadialNode _rotation;
        private RadialNode _proximity;

        private RadialNetwork _nn;

        public FishRadialNN(int x, int y)
            : base(x, y)
        {
            this._nn = new RadialNetwork(25);
            this._lightR = this._nn.AddOutput();
            this._lightG = this._nn.AddOutput();
            this._lightB = this._nn.AddOutput();
            this._eyeR = this._nn.AddInput();
            this._eyeG = this._nn.AddInput();
            this._eyeB = this._nn.AddInput();
            this._speed = this._nn.AddOutput();
            this._rotation = this._nn.AddOutput();
            this._proximity = this._nn.AddInput();

            this.X = x;
            this.Y = y;
        }

        public override void Tick()
        {
            this._nn.Tick();

            double rotationDelta = this._rotation.Value * 2d - 1;
            this.Angle += ((int)((double)FishRadialNN.MaxRotation * rotationDelta) + 360) % 360;

            double xSpeed = this._speed.Value * Math.Cos(this.Angle * Math.PI / 180d) * (double)FishRadialNN.MaxSpeed;
            double ySpeed = this._speed.Value * Math.Sin(this.Angle * Math.PI / 180d) * (double)FishRadialNN.MaxSpeed;

            this.XVelocity = xSpeed;
            this.YVelocity = ySpeed;

            this.X += this.XVelocity;
            this.Y += this.YVelocity;
        }

        public override Color GetLightColor()
        {
            byte r = (byte)(this._lightR.Value * (double)byte.MaxValue);
            byte g = (byte)(this._lightG.Value * (double)byte.MaxValue);
            byte b = (byte)(this._lightB.Value * (double)byte.MaxValue);
            return Color.FromArgb(r, g, b);
        }

        public override void SetEye(Color c)
        {
            this._eyeR.Value = (double)c.R / (double)byte.MaxValue;
            this._eyeG.Value = (double)c.G / (double)byte.MaxValue;
            this._eyeB.Value = (double)c.B / (double)byte.MaxValue;
        }

        public override void SetProximity(int proximity)
        {
            int proxy = Math.Max(0, FishRadialNN.MaxProximity - proximity);
            this._proximity.Value = (double)proxy / FishRadialNN.MaxProximity;
        }

        public override void SetFood(int food)
        {
            throw new NotImplementedException();
        }

        public override void SetSmell(int smell)
        {
            throw new NotImplementedException();
        }

        public override string GetGeneticCode()
        {
            throw new NotImplementedException();
        }

        public override void SetGeneticCode(string geneticCode)
        {
            throw new NotImplementedException();
        }

        public override SFML.Graphics.Drawable[] RenderBrain(SFML.Graphics.FloatRect view)
        {
            throw new NotImplementedException();
        }
    }
}
