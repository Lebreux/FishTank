using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLApps.NeuralNetwork
{
    public class SpikingNode
    {
        private static double TriggerThreshold = .9d;
        private static double ActionPotentialRecover = .05d;
        private static double PotentialDecay = .01d;
        private static double ActionPotential = .25d;

        private double _actionPotentialMult;
        public double Potential { get; private set; }

        public Dictionary<SpikingNode, double> Links { get; set; }

        public SpikingNode(double potential)
        {
            this._actionPotentialMult = 1d;
            this.Potential = potential;
            this.Links = new Dictionary<SpikingNode, double>();
        }

        public void ReceiveSpike(double action)
        {
            this.Potential = System.Math.Min(System.Math.Max(0d, this.Potential + this._actionPotentialMult * action), 1d);
        }

        public void SendSpike()
        {
            this.Potential = 0d;
            this._actionPotentialMult = 0d;
            foreach (var link in this.Links)
                link.Key.ReceiveSpike(link.Value * SpikingNode.ActionPotential);
        }

        public void Tick()
        {
            this._actionPotentialMult = System.Math.Min(1d, this._actionPotentialMult + SpikingNode.ActionPotentialRecover);
            this.Potential = System.Math.Max(0d, this.Potential - SpikingNode.PotentialDecay);
            if (this.Potential > SpikingNode.TriggerThreshold)
                this.SendSpike();
        }
    }
}
