using System;
using System.Collections.Generic;

namespace MyToolkit.Recognition.WinRT.Feedforward.Training.Backpropagation
{
    public class Backpropagation : ITrainable
    {
	    public FeedforwardNetwork Network { get; private set; }
		public double Error { get; private set; }

		private readonly IDictionary<FeedforwardLayer, BackpropagationLayer> layerMapping =
			new Dictionary<FeedforwardLayer, BackpropagationLayer>();

        private readonly double learnRate;
		private readonly double momentum;

		private readonly double[][] input;
		private readonly double[][] ideal;

        public Backpropagation(FeedforwardNetwork network,
                 double[][] input, double[][] ideal,
                 double learnRate, double momentum)
        {
            Network = network;

            this.learnRate = learnRate;
            this.momentum = momentum;
            this.input = input;
            this.ideal = ideal;

            foreach (var layer in network.Layers)
                layerMapping.Add(layer, new BackpropagationLayer(this, layer));
        }

        public void CalculateError(double[] ideal)
        {
			if (ideal.Length != Network.OutputLayer.NeuronCount)
				throw new ArgumentException("ideal size mismatch");

            foreach (var layer in Network.Layers)
                GetBackpropagationLayer(layer).ClearError();

            for (var i = Network.Layers.Count - 1; i >= 0; i--)
            {
                var layer = Network.Layers[i];
                if (layer.IsOutputLayer)
                    GetBackpropagationLayer(layer).CalculateError(ideal);
                else
                    GetBackpropagationLayer(layer).CalculateError();
            }
        }

        public BackpropagationLayer GetBackpropagationLayer(FeedforwardLayer layer)
        {
            return layerMapping[layer];
        }

        public void Iteration()
        {
			for (var j = 0; j < input.Length; j++)
            {
                Network.ComputeOutputs(input[j]);
                CalculateError(ideal[j]);
            }
			Error = Learn();
        }

        public double Learn()
        {
			foreach (var layer in Network.Layers)
			{
				var bLayer = GetBackpropagationLayer(layer);
				bLayer.Learn(learnRate, momentum);
			}

			return Network.CalculateError(input, ideal);
        }
    }
}
