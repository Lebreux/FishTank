using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLApps.NeuralNetwork;
using System.Drawing;

namespace FLApps.FishTank
{
    internal class FishSpikingNN : Fish
    {
        private static int MaxRotation = 5;
        private static int MaxSpeed = 2;
        private static int MaxProximity = 250;
        private static int MaxSmell = 250;

        private SpikingNode _lightR;
        private SpikingNode _lightG;
        private SpikingNode _lightB;
        private SpikingNode _eyeR;
        private SpikingNode _eyeG;
        private SpikingNode _eyeB;
        private SpikingNode _speed;
        private SpikingNode _rotationR;
        private SpikingNode _rotationL;
        private SpikingNode _proximity;
        private SpikingNode _smellFood;
        private SpikingNode _hungriness;

        private SpikingNetwork _nn;

        public FishSpikingNN(int x, int y)
            : base(x, y)
        {
            this._nn = new SpikingNetwork(15, 15);
            this._lightR = this._nn.AddOutput();
            this._lightG = this._nn.AddOutput();
            this._lightB = this._nn.AddOutput();
            this._eyeR = this._nn.AddInput();
            this._eyeG = this._nn.AddInput();
            this._eyeB = this._nn.AddInput();
            this._speed = this._nn.AddOutput();
            this._rotationR = this._nn.AddOutput();
            this._rotationL = this._nn.AddOutput();
            this._proximity = this._nn.AddInput();
            this._smellFood = this._nn.AddInput();
            this._hungriness = this._nn.AddInput();
        }

        public override void Tick()
        {
            this._nn.Tick();

            double rotationDelta = this._rotationR.Potential - this._rotationL.Potential;
            this.Angle += ((int)((double)FishSpikingNN.MaxRotation * rotationDelta) + 360) % 360;

            double xSpeed = this._speed.Potential * Math.Cos(this.Angle * Math.PI / 180d) * (double)FishSpikingNN.MaxSpeed;
            double ySpeed = this._speed.Potential * Math.Sin(this.Angle * Math.PI / 180d) * (double)FishSpikingNN.MaxSpeed;

            this.XVelocity = xSpeed;
            this.YVelocity = ySpeed;

            this.X += this.XVelocity;
            this.Y += this.YVelocity;
        }

        public override Color GetLightColor()
        {
            byte r = (byte)(this._lightR.Potential * (double)byte.MaxValue);
            byte g = (byte)(this._lightG.Potential * (double)byte.MaxValue);
            byte b = (byte)(this._lightB.Potential * (double)byte.MaxValue);
            return Color.FromArgb(r, g, b);
        }

        public override void SetEye(Color c)
        {
            this._eyeR.ReceiveSpike((double)c.R / (double)byte.MaxValue);
            this._eyeG.ReceiveSpike((double)c.G / (double)byte.MaxValue);
            this._eyeB.ReceiveSpike((double)c.B / (double)byte.MaxValue);
        }

        public override void SetProximity(int proximity)
        {
            int proxy = Math.Max(0, FishSpikingNN.MaxProximity - proximity);
            this._proximity.ReceiveSpike((double)proxy / FishSpikingNN.MaxProximity);
        }

        public override void SetFood(int food)
        {
            this.Food = Math.Min(Math.Max(0, food), 1000);
            this._hungriness.ReceiveSpike((double)byte.MaxValue - food);
        }

        public override void SetSmell(int smell)
        {
            int smellValue = Math.Max(0, FishSpikingNN.MaxSmell - smell);
            this._smellFood.ReceiveSpike((double)smellValue / FishSpikingNN.MaxSmell);
        }

        public override string GetGeneticCode()
        {
            return this._nn.GetGeneticCode();
        }

        public override void SetGeneticCode(string geneticCode)
        {
            this._nn.SetGeneticCode(geneticCode);
        }

        public override SFML.Graphics.Drawable[] RenderBrain(SFML.Graphics.FloatRect view)
        {
            throw new NotImplementedException();
        }
    }
}
