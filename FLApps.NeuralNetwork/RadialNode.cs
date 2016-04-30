using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLApps.NeuralNetwork
{
    public class RadialNode
    {
        public double Value { get; set; }
        public RadialNode Addon { get; set; }
        public RadialNode(double value)
        {
            this.Value = value;
            this.Addon = null;
        }
    }
}
