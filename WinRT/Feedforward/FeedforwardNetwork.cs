using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using MyToolkit.MachineLearning.WinRT.Utilities;

namespace MyToolkit.MachineLearning.WinRT.Feedforward
{
    public class FeedforwardNetwork
    {
		public List<FeedforwardLayer> Layers { get; set; }

		public FeedforwardNetwork()
		{
			Layers = new List<FeedforwardLayer>();
		}

		public void UpdateLayers()
		{
			for (var i = 0; i < Layers.Count; i++)
			{
				var layer = Layers[i];
				layer.Previous = i > 0 ? Layers[i - 1] : null;
				if (i + 1 < Layers.Count)
					layer.Next = Layers[i + 1];
			}
		}
		
		[XmlIgnore]
		public FeedforwardLayer InputLayer
		{
			get { return Layers.FirstOrDefault(); }
		}

		[XmlIgnore]
		public FeedforwardLayer OutputLayer
		{
			get { return Layers.LastOrDefault(); }
		}
        
		[XmlIgnore]
        public int HiddenLayerCount
        {
            get { return Layers.Count - 2; }
        }

        /// <summary>
        /// Get the sum of all matrix sizes of all layers
        /// </summary>
		[XmlIgnore]
		public int MatrixSize
        {
            get { return Layers.Sum(layer => layer.MatrixSize); }
        }

        [XmlIgnore]
        public ICollection<FeedforwardLayer> HiddenLayers
        {
            get { return Layers.Where(layer => layer.IsHiddenLayer).ToList(); }
        }

        public void AddLayer(FeedforwardLayer layer)
        {
            if (OutputLayer != null)
            {
				layer.Previous = OutputLayer;
				OutputLayer.Next = layer;
            }

			Layers.Add(layer);
        }

        public double CalculateError(double[][] input, double[][] ideal)
        {
            var errorCalculation = new ErrorCalculation();
            for (var i = 0; i < ideal.Length; i++)
            {
                ComputeOutputs(input[i]);
                errorCalculation.UpdateError(OutputLayer.Fire, ideal[i]);
            }

	        return errorCalculation.CalculateRootMeanSquare();
        }

		public FeedforwardNetwork Clone()
        {
            var result = CloneStructure();
            var copy = ToArray();
            ArrayToNetwork(copy, result);
            return result;
        }

		public FeedforwardNetwork CloneStructure()
        {
            var result = new FeedforwardNetwork();
			foreach (var layer in Layers)
            {
                var clonedLayer = new FeedforwardLayer(layer.NeuronCount);
                result.AddLayer(clonedLayer);
            }
			return result;
        }

        public double[] ComputeOutputs(double[] input)
        {
			if (input.Length != InputLayer.NeuronCount)
                throw new ArgumentException("Input size mismatch");

            foreach (var layer in Layers)
            {
                if (layer.IsInputLayer)
                    layer.ComputeOutputs(input);
                else if (layer.IsHiddenLayer)
                    layer.ComputeOutputs(null);
            }

            return OutputLayer.Fire;
        }

        public bool Equals(FeedforwardNetwork other)
        {
            var i = 0;
            foreach (var layer in Layers)
            {
                var otherLayer = other.Layers[i++];
                if (layer.NeuronCount != otherLayer.NeuronCount)
                    return false;

                // make sure they either both have or do not have matrix weight matrix.
                if ((layer.LayerMatrix == null) && (otherLayer.LayerMatrix != null))
                    return false;

                if ((layer.LayerMatrix != null) && (otherLayer.LayerMatrix == null))
                    return false;

                // if they both have matrix matrix, then compare the matrices
                if ((layer.LayerMatrix != null) && (otherLayer.LayerMatrix != null))
                {
                    if (!layer.LayerMatrix.Equals(otherLayer.LayerMatrix))
                        return false;
                }
            }
			return true;
        }

        public void Reset()
        {
            foreach (var layer in Layers)
                layer.Reset();
        }

	    public double[] ToArray()
	    {
		    var size = 0;
		    foreach (FeedforwardLayer layer in Layers.Where(l => l.HasMatrix))
			    size += layer.MatrixSize;

		    var result = new Double[size];
		    var index = 0;
		    foreach (var layer in Layers)
		    {
			    if (layer.Next != null)
			    {
				    for (var r = 0; r < layer.LayerMatrix.Rows; r++)
				    {
					    for (var c = 0; c < layer.LayerMatrix.Columns; c++)
						    result[index++] = layer.LayerMatrix.Data[r, c];
				    }
			    }
		    }
		    return result;
	    }

		public static void ArrayToNetwork(Double[] array, FeedforwardNetwork network)
		{
			var index = 0;
			foreach (var layer in network.Layers)
			{
				if (layer.Next != null)
					index = layer.LayerMatrix.FromPackedArray(array, index);
			}
		}
    }
}
