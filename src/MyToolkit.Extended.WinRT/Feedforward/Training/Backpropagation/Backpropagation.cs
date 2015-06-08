using System;
using System.Collections.Generic;

namespace MyToolkit.MachineLearning.WinRT.Feedforward.Training.Backpropagation
{
    public class Backpropagation : ITrainable
    {
	    public FeedforwardNetwork Network { get; private set; }
		public double Error { get; private set; }

		private readonly IDictionary<FeedforwardLayer, BackpropagationLayer> _layerMapping =
			new Dictionary<FeedforwardLayer, BackpropagationLayer>();

        private readonly double _learnRate;
		private readonly double _momentum;

		private readonly double[][] _input;
		private readonly double[][] _ideal;

        public Backpropagation(FeedforwardNetwork network,
                 double[][] input, double[][] ideal,
                 double learnRate, double momentum)
        {
            Network = network;

            _learnRate = learnRate;
            _momentum = momentum;
            _input = input;
            _ideal = ideal;

            foreach (var layer in network.Layers)
                _layerMapping.Add(layer, new BackpropagationLayer(this, layer));
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
            return _layerMapping[layer];
        }

        public void Iteration()
        {
			for (var j = 0; j < _input.Length; j++)
            {
                Network.ComputeOutputs(_input[j]);
                CalculateError(_ideal[j]);
            }
			Error = Learn();
        }

        public double Learn()
        {
			foreach (var layer in Network.Layers)
			{
				var bLayer = GetBackpropagationLayer(layer);
				bLayer.Learn(_learnRate, _momentum);
			}

			return Network.CalculateError(_input, _ideal);
        }
    }
}
