using System;
using System.Xml.Serialization;
using MyToolkit.MachineLearning.WinRT.Activation;
using MyToolkit.Mathematics;

namespace MyToolkit.MachineLearning.WinRT.Feedforward
{
	public class FeedforwardLayer
	{
		internal FeedforwardLayer() { } // used for serialization
		public FeedforwardLayer(int neuronCount) : this(new ActivationSigmoid(), neuronCount) { }
		public FeedforwardLayer(ActivationFunction thresholdFunction, int neuronCount)
		{
			Fire = new double[neuronCount];
			LayerActivationFunction = thresholdFunction;
		}
		
		public ActivationFunction LayerActivationFunction { get; set; }

		private Matrix layerMatrix; 
		public Matrix LayerMatrix
        {
            get { return layerMatrix; }
            set
            {
				layerMatrix = value;
				if (layerMatrix != null)
				{
					if (layerMatrix.Rows < 2)
						throw new Exception("matrix must have at least 2 rows.");
					
					if (layerMatrix != null && Fire == null)
						Fire = new double[layerMatrix.Rows - 1];
				}
            }
        }

		public bool HasMatrix
		{
			get { return LayerMatrix != null; }
		}

		public bool IsHiddenLayer
		{
			get { return (next != null) && (Previous != null); }
		}

		public bool IsInputLayer
		{
			get { return Previous == null; }
		}

		public bool IsOutputLayer
		{
			get { return next == null; }
		}

        [XmlIgnore]
        public int NeuronCount
        {
            get { return Fire.Length; }
        }

		public double[] Fire { get; set; }

		[XmlIgnore]
		public FeedforwardLayer Previous { get; set; }

		private FeedforwardLayer next; 
		[XmlIgnore]
		public FeedforwardLayer Next
        {
            get { return next; }
            set
            {
                next = value;
				if (LayerMatrix == null) // add one to the neuron count to provide a threshold value in row 0
					LayerMatrix = new Matrix(NeuronCount + 1, next.NeuronCount);
            }
        }

		[XmlIgnore]
		public int MatrixSize
        {
            get { return layerMatrix == null ? 0 : layerMatrix.Size; }
        }

        public FeedforwardLayer CloneStructure()
        {
            return new FeedforwardLayer(LayerActivationFunction, NeuronCount);
        }

        public double[] ComputeOutputs(double[] pattern)
        {
            if (pattern != null)
            {
                for (var i = 0; i < NeuronCount; i++)
                    Fire[i] = pattern[i];
            }

            for (var i = 0; i < next.NeuronCount; i++)
            {
				var sum = GetDotProductOfColumnWithOneFakeColumn(LayerMatrix, i, Fire);
				next.Fire[i] = LayerActivationFunction.Function(sum);
            }

            return Fire;
        }

		private double GetDotProductOfColumnWithOneFakeColumn(Matrix matrix, int col, double[] b)
		{
			var result = 0.0;
			for (var row = 0; row < matrix.Rows - 1; row++)
				result += matrix.Data[row, col] * b[row];
			result += matrix.Data[matrix.Rows - 1, col] * 1; // fake column = 1
			return result;
		}

        public void Prune(int neuron)
        {
			if (layerMatrix != null)
				LayerMatrix = LayerMatrix.DeleteRow(neuron); // delete a row on this matrix

			if (Previous != null)
            {
				if (Previous.LayerMatrix != null)
					Previous.LayerMatrix = Previous.LayerMatrix.DeleteColumn(neuron);  // delete a column on the previous
            }
        }

        public void Reset()
        {
			if (LayerMatrix != null)
				LayerMatrix.Ramdomize(-1, 1);
        }

		public override string ToString()
		{
			return "FeedforwardLayer: NeuronCount = " + NeuronCount;
		}
    }
}
