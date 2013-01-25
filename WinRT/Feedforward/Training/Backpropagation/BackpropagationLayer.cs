using MyToolkit.Mathematics;
using MyToolkit.Recognition.WinRT.Utilities;

namespace MyToolkit.Recognition.WinRT.Feedforward.Training.Backpropagation
{
    public class BackpropagationLayer
    {
		private readonly Backpropagation parent;
		private readonly FeedforwardLayer layer;
		
		private readonly double[] error;
        private readonly double[] errorDelta;
		
		private readonly int biasRow;
		
        private readonly Matrix accumulatedMatrixDelta;
		private Matrix previousMatrixDelta;

        public BackpropagationLayer(Backpropagation parent, FeedforwardLayer layer)
        {
            this.parent = parent;
            this.layer = layer;

            error = new double[layer.NeuronCount];
            errorDelta = new double[layer.NeuronCount];

            if (layer.Next != null)
            {
                accumulatedMatrixDelta = new Matrix(layer.NeuronCount + 1, layer.Next.NeuronCount);
                previousMatrixDelta = new Matrix(layer.NeuronCount + 1, layer.Next.NeuronCount);
                biasRow = layer.NeuronCount;
            }
        }

        public void AccumulateThresholdDelta(int index, double value)
        {
            accumulatedMatrixDelta.Add(biasRow, index, value);
        }

        public void CalculateError()
        {
			var next = parent.GetBackpropagationLayer(layer.Next);
			for (var i = 0; i < layer.Next.NeuronCount; i++)
            {
                for (var j = 0; j < layer.NeuronCount; j++)
                {
					accumulatedMatrixDelta.Add(j, i, next.GetErrorDelta(i) * layer.Fire[j]);
                    SetError(j, GetError(j) + layer.LayerMatrix[j, i] * next.GetErrorDelta(i));
                }
                AccumulateThresholdDelta(i, next.GetErrorDelta(i));
            }

            if (layer.IsHiddenLayer)
            {
				for (var i = 0; i < layer.NeuronCount; i++) // hidden layer deltas
                    SetErrorDelta(i, BoundNumbers.Bound(CalculateDelta(i)));
            }
        }

        public void CalculateError(double[] ideal)
        {
            for (var i = 0; i < layer.NeuronCount; i++)
            {
                SetError(i, ideal[i] - layer.Fire[i]);
                SetErrorDelta(i, BoundNumbers.Bound(CalculateDelta(i)));
            }
        }

        private double CalculateDelta(int i)
        {
            return GetError(i) * layer.LayerActivationFunction.
				DerivativeFunction(layer.Fire[i]);
        }

        public void ClearError()
        {
            for (var i = 0; i < layer.NeuronCount; i++)
                error[i] = 0;
        }

        public double GetError(int index)
        {
            return error[index];
        }

        public double GetErrorDelta(int index)
        {
            return errorDelta[index];
        }

		public void Learn(double learnRate, double momentum)
        {
            if (layer.HasMatrix)
            {
                previousMatrixDelta = (accumulatedMatrixDelta * learnRate) + (previousMatrixDelta * momentum);
                layer.LayerMatrix = layer.LayerMatrix.Add(previousMatrixDelta);

				accumulatedMatrixDelta.Clear();
			}
        }

		public void SetError(int index, double e)
        {
            error[index] = BoundNumbers.Bound(e);
        }

        public void SetErrorDelta(int index, double d)
        {
            errorDelta[index] = d;
        }
	}
}
