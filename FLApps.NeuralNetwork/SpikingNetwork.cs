using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLApps.NeuralNetwork
{
    public class SpikingNetwork
    {
        private static Random _rand = new Random();
        private static double GenerateWeight() { return _rand.NextDouble() * 2d - 1d; }

        private static char LinkSeparator = ',';
        private static char NodeSeparator = ';';
        private static char LayerSeparator = '>';

        private List<SpikingNode> _inputLayer;
        private List<List<SpikingNode>> _layers;
        private List<SpikingNode> _outputLayer;

        public SpikingNetwork(params int[] layersNbNodes)
        {
            this._inputLayer = new List<SpikingNode>();
            this._layers = new List<List<SpikingNode>>();
            this._outputLayer = new List<SpikingNode>();

            foreach (var layerNbNodes in layersNbNodes)
            {
                List<SpikingNode> layer = new List<SpikingNode>();
                for (int i = 0; i < layerNbNodes; i++)
                    layer.Add(new SpikingNode(_rand.NextDouble()));
                this._layers.Add(layer);
            }

            for (int i = 0; i < this._layers.Count - 1; i++)
                foreach (var nodeA in this._layers[i])
                    foreach (var nodeB in this._layers[i + 1])
                        nodeA.Links.Add(nodeB, GenerateWeight());
        }

        public SpikingNode AddInput()
        {
            SpikingNode node = new SpikingNode(0d);
            this._inputLayer.Add(node);
            foreach (var nodeB in this._layers.First())
                node.Links.Add(nodeB, GenerateWeight());
            return node;
        }

        public SpikingNode AddOutput()
        {
            SpikingNode node = new SpikingNode(0d);
            this._outputLayer.Add(node);
            foreach (var nodeA in this._layers.Last())
                nodeA.Links.Add(node, GenerateWeight());
            return node;
        }

        public void Tick()
        {
            foreach (var node in this._inputLayer)
                node.Tick();
            foreach (var layer in this._layers)
                foreach (var node in layer)
                    node.Tick();
        }

        public string PrintNetwork()
        {
            int maxHeight = System.Math.Max(System.Math.Max(this._inputLayer.Count, (from layer in this._layers select layer).Max(x => x.Count)), this._outputLayer.Count);
            int maxWidth = 1 + this._layers.Count + 1;

            double?[,] nodes = new double?[maxWidth, maxHeight];
            for (int i = 0; i < this._inputLayer.Count; i++)
                nodes[0, i] = this._inputLayer[i].Potential;
            for (int iLayer = 0; iLayer < this._layers.Count; iLayer++)
                for (int iNode = 0; iNode < this._layers[iLayer].Count; iNode++)
                    nodes[iLayer + 1, iNode] = this._layers[iLayer][iNode].Potential;
            for (int i = 0; i < this._outputLayer.Count; i++)
                nodes[this._layers.Count + 1, i] = this._outputLayer[i].Potential;

            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                for (int x = 0; x < nodes.GetLength(0); x++)
                {
                    double? val = nodes[x, y];
                    if (val.HasValue)
                        sb.AppendFormat("{0:000%}", nodes[x, y]);
                    else
                        sb.Append("---%");
                    sb.Append(" / ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string GetGeneticCode()
        {
            List<string> layers = new List<string>();
            layers.Add(this.GetLayerGC(this._inputLayer));
            foreach (var layer in this._layers)
                layers.Add(this.GetLayerGC(layer));
            return string.Join(SpikingNetwork.LayerSeparator.ToString(), layers);
        }

        private string GetLayerGC(List<SpikingNode> layer)
        {
            List<string> nodes = new List<string>();
            foreach (var node in layer)
                nodes.Add(this.GetNodeGC(node));
            return string.Join(SpikingNetwork.NodeSeparator.ToString(), nodes);
        }

        private string GetNodeGC(SpikingNode node)
        {
            List<string> links = new List<string>();
            foreach (var link in node.Links)
                links.Add(link.Value.ToString());
            return string.Join(SpikingNetwork.LinkSeparator.ToString(), links);
        }

        public void SetGeneticCode(string geneticCode)
        {
            string[] layers = geneticCode.Split(SpikingNetwork.LayerSeparator);
            this.SetLayerGC(this._inputLayer, layers[0]);
            for (int i = 0; i < this._layers.Count; i++)
                this.SetLayerGC(this._layers[i], layers[i + 1]);
        }

        private void SetLayerGC(List<SpikingNode> layer, string gc)
        {
            string[] nodes = gc.Split(SpikingNetwork.NodeSeparator);
            for (int i = 0; i < layer.Count; i++)
                this.SetNodeGC(layer[i], nodes[i]);
        }

        private void SetNodeGC(SpikingNode node, string gc)
        {
            string[] links = gc.Split(SpikingNetwork.LinkSeparator);
            for (int i = 0; i < node.Links.Keys.Count; i++)
                node.Links[node.Links.Keys.ElementAt(i)] = double.Parse(links[i]);
        }

        public static string MutateGeneticCode(string geneticCode, double mutateChance)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(geneticCode, @"[0-9\.]+");
            foreach (System.Text.RegularExpressions.Match match in matches)
                if (_rand.NextDouble() < mutateChance)
                    geneticCode.Replace(match.Value, _rand.NextDouble().ToString());
            return geneticCode;
        }
    }
}
