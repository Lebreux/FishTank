using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLApps.NeuralNetwork
{
    public class RadialNetwork
    {
        private static Random _rand = new Random();
        private List<RadialNode> _nodes;
        private List<RadialNodeLink> _nodeLinks;

        public RadialNetwork(int nbNodes)
        {
            this._nodes = new List<RadialNode>();
            for (int i = 0; i < nbNodes; i++)
                this._nodes.Add(new RadialNode(_rand.NextDouble()));
            this._nodeLinks = new List<RadialNodeLink>();
            foreach (var nodeA in this._nodes)
                foreach (var nodeB in this._nodes)
                    this._nodeLinks.Add(new RadialNodeLink(nodeA, nodeB, _rand.NextDouble()));
        }

        public RadialNode AddInput()
        {
            RadialNode node = this.GetNextAddon();
            RadialNode inputNode = new RadialNode(0d);
            if (node != null)
            {
                node.Addon = inputNode;
                this._nodeLinks.Insert(0, new RadialNodeLink(node.Addon, node, _rand.NextDouble()));
            }
            else
                throw new Exception("Couldn't find free node.");
            return inputNode;
        }

        public RadialNode AddOutput()
        {
            RadialNode node = this.GetNextAddon();
            RadialNode outputNode = new RadialNode(0d);
            if (node != null)
            {
                node.Addon = outputNode;
                this._nodeLinks.Add(new RadialNodeLink(node, node.Addon, 1d));
                this._nodes.Add(outputNode);
            }
            else
                throw new Exception("Couldn't find free node.");
            return outputNode;
        }

        private RadialNode GetNextAddon()
        {
            foreach (var node in this._nodes)
                if (node.Addon == null)
                    return node;
            return null;
        }

        public void Tick()
        {
            foreach (var node in this._nodes)
                node.Value = 0d;
            foreach (var nodeLink in this._nodeLinks)
                nodeLink.NodeB.Value += nodeLink.NodeA.Value * nodeLink.Weight;
            foreach (var node in this._nodes)
                node.Value = RadialNetwork.Sigmoid(node.Value);
        }

        public string PrintOut()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this._nodes.Count; i++)
            {
                sb.AppendFormat("{0}: {1}", i, this._nodes[i].Value);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static double Sigmoid(double value)
        {
            double outValue = 1d / (1d + System.Math.Pow(System.Math.E, -value));
            //Console.WriteLine("{0}: {1:00%}", value, outValue);
            return outValue;
        }

        private class RadialNodeLink
        {
            public RadialNode NodeA { get; private set; }
            public RadialNode NodeB { get; private set; }
            public double Weight { get; set; }
            public RadialNodeLink(RadialNode nodeA, RadialNode nodeB, double weight)
            {
                this.NodeA = nodeA;
                this.NodeB = nodeB;
                this.Weight = weight;
            }
        }
    }
}
