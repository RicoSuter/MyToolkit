using MyToolkit.MachineLearning.WinRT.Utilities;
using MyToolkit.Mathematics;

namespace MyToolkit.MachineLearning.WinRT.Feedforward.Training.Backpropagation
{
    public class BackpropagationLayer
    {
		private readonly Backpropagation _parent;
		private readonly FeedforwardLayer _layer;
		
		private readonly double[] _error;
        private readonly double[] _errorDelta;
		
		private readonly int _biasRow;
		
        private readonly Matrix _accumulatedMatrixDelta;
		private Matrix _previousMatrixDelta;

        public BackpropagationLayer(Backpropagation parent, FeedforwardLayer layer)
        {
            _parent = parent;
            _layer = layer;

            _error = new double[layer.NeuronCount];
            _errorDelta = new double[layer.NeuronCount];

            if (layer.Next != null)
            {
                _accumulatedMatrixDelta = new Matrix(layer.NeuronCount + 1, layer.Next.NeuronCount);
                _previousMatrixDelta = new Matrix(layer.NeuronCount + 1, layer.Next.NeuronCount);
                _biasRow = layer.NeuronCount;
            }
        }

        public void AccumulateThresholdDelta(int index, double value)
        {
            _accumulatedMatrixDelta.Add(_biasRow, index, value);
        }

        public void CalculateError()
        {
			var next = _parent.GetBackpropagationLayer(_layer.Next);
			for (var i = 0; i < _layer.Next.NeuronCount; i++)
            {
                for (var j = 0; j < _layer.NeuronCount; j++)
                {
					_accumulatedMatrixDelta.Add(j, i, next.GetErrorDelta(i) * _layer.Fire[j]);
                    SetError(j, GetError(j) + _layer.LayerMatrix[j, i] * next.GetErrorDelta(i));
                }
                AccumulateThresholdDelta(i, next.GetErrorDelta(i));
            }

            if (_layer.IsHiddenLayer)
            {
				for (var i = 0; i < _layer.NeuronCount; i++) // hidden layer deltas
                    SetErrorDelta(i, BoundNumbers.Bound(CalculateDelta(i)));
            }
        }

        public void CalculateError(double[] ideal)
        {
            for (var i = 0; i < _layer.NeuronCount; i++)
            {
                SetError(i, ideal[i] - _layer.Fire[i]);
                SetErrorDelta(i, BoundNumbers.Bound(CalculateDelta(i)));
            }
        }

        private double CalculateDelta(int i)
        {
            return GetError(i) * _layer.LayerActivationFunction.
				DerivativeActivate(_layer.Fire[i]);
        }

        public void ClearError()
        {
            for (var i = 0; i < _layer.NeuronCount; i++)
                _error[i] = 0;
        }

        public double GetError(int index)
        {
            return _error[index];
        }

        public double GetErrorDelta(int index)
        {
            return _errorDelta[index];
        }

		public void Learn(double learnRate, double momentum)
        {
            if (_layer.HasMatrix)
            {
                _previousMatrixDelta = (_accumulatedMatrixDelta * learnRate) + (_previousMatrixDelta * momentum);
                _layer.LayerMatrix = _layer.LayerMatrix.Add(_previousMatrixDelta);

				_accumulatedMatrixDelta.Clear();
			}
        }

		public void SetError(int index, double e)
        {
            _error[index] = BoundNumbers.Bound(e);
        }

        public void SetErrorDelta(int index, double d)
        {
            _errorDelta[index] = d;
        }
	}
}
