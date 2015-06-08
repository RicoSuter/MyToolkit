using System;
using System.Linq;
using MyToolkit.Mathematics;

namespace MyToolkit.MachineLearning.WinRT.SOM
{
	public class SelfOrganizingMapTrainer
    {
		public double MinValue = Math.Pow(10, -30);

		public double ReductionFactor { get; set; }
		public double BestError { get; private set; }
	    public double TotalError { get; protected set; }
		public LearningMethod LearningMethod { get; private set; }
		public double LearningRate { get; private set; }

		public SelfOrganizingMap Network { get; private set; }
		public SelfOrganizingMap BestNetwork { get; private set; }
		public double[][] TrainingSet { get; private set; }

		private double _globalError;

		private readonly int[] _neuronWinCounts;
        private readonly int _outputNeuronCount;
        private readonly int _inputNeuronCount;

		private readonly Matrix _workMatrix;
        private readonly Matrix _correctionMatrix;

		public SelfOrganizingMapTrainer(SelfOrganizingMap network, double[][] trainingSet, LearningMethod learningMethod, double learningRate)
        {
	        ReductionFactor = 0.99;
			TotalError = 1.0;

            Network = network;
            TrainingSet = trainingSet;
			LearningMethod = learningMethod;
			LearningRate = learningRate;

            TotalError = 1.0;

			_outputNeuronCount = network.OutputNeuronCount;
			_inputNeuronCount = network.InputNeuronCount;

            for (int i = 0; i < trainingSet.Length; i++)
            {
                if (Matrix.CreateColumnMatrix(trainingSet[i]).VectorLength() < MinValue)
                    throw new Exception("Multiplicative normalization has null training case");
            }

			BestNetwork = new SelfOrganizingMap(_inputNeuronCount, _outputNeuronCount, network.NormalizationType);

            _neuronWinCounts = new int[_outputNeuronCount];
            _correctionMatrix = new Matrix(_outputNeuronCount, _inputNeuronCount + 1);

            _workMatrix = LearningMethod == LearningMethod.Additive ? new Matrix(1, _inputNeuronCount + 1) : null;

            Initialize();
            BestError = Double.MaxValue;
        }

        protected void AdjustWeights()
        {
            for (var i = 0; i < _outputNeuronCount; i++)
            {
                if (_neuronWinCounts[i] == 0)
                    continue;

                var f = 1.0 / _neuronWinCounts[i];
                if (LearningMethod == LearningMethod.Subtractive)
                    f *= LearningRate;

                for (var j = 0; j <= _inputNeuronCount; j++)
                {
                    var corr = f * _correctionMatrix[i, j];
                    Network.OutputWeights.Add(i, j, corr);
                }
            }
        }

		public void EvaluateErrors()
        {
			_correctionMatrix.Clear();
			for (var i = 0; i < _neuronWinCounts.Length; i++)
                _neuronWinCounts[i] = 0;

            _globalError = 0.0;
           
			// loop through all training sets to determine correction
            for (var tset = 0; tset < TrainingSet.Length; tset++)
            {
                var input = new NormalizeInput(TrainingSet[tset], Network.NormalizationType);
                var best = Network.GetWinner(input);

                _neuronWinCounts[best]++;
                var bestRow = Network.OutputWeights.GetRow(best);

				double diff;
				var length = 0.0;
                for (var i = 0; i < _inputNeuronCount; i++)
                {
                    diff = TrainingSet[tset][i] * input.NormalizationFactor - bestRow[0, i];
                    length += diff * diff;
                    if (LearningMethod == LearningMethod.Subtractive)
                        _correctionMatrix.Add(best, i, diff);
                    else
                        _workMatrix[0, i] = LearningRate * TrainingSet[tset][i] * input.NormalizationFactor + bestRow[0, i];
                }

                diff = input.SyntheticInput - bestRow[0, _inputNeuronCount];
                length += diff * diff;

				if (LearningMethod == LearningMethod.Subtractive)
					_correctionMatrix.Add(best, _inputNeuronCount, diff);
                else
                    _workMatrix[0, _inputNeuronCount] =  LearningRate * input.SyntheticInput + bestRow[0, _inputNeuronCount];

                if (length > _globalError)
                    _globalError = length;

				if (LearningMethod == LearningMethod.Additive)
                {
                    NormalizeRowWeight(_workMatrix, 0);
                    for (var i = 0; i <= _inputNeuronCount; i++)
                        _correctionMatrix.Add(best, i, _workMatrix[0, i] - bestRow[0, i]);
                }
            }
			_globalError = Math.Sqrt(_globalError);
        }

        protected void ForceWin()
        {
	        int which = 0;
			var outputWeights = Network.OutputWeights;
			var dist = Double.MaxValue;
            for (var tset = 0; tset < TrainingSet.Length; tset++)
            {
                var best = Network.GetWinner(TrainingSet[tset]);
                
				var output = Network.Output;
                if (output[best] < dist)
                {
                    dist = output[best];
                    which = tset;
                }
            }

            var input = new NormalizeInput(TrainingSet[which], Network.NormalizationType);
			Network.GetWinner(input);
            var output2 = Network.Output;

            dist = Double.MinValue;
            var i = _outputNeuronCount;
            while (i-- > 0)
            {
                if (_neuronWinCounts[i] != 0)
                    continue;

				if (output2[i] > dist)
                {
                    dist = output2[i];
                    which = i;
                }
            }

            for (var j = 0; j < input.InputMatrix.Columns; j++)
                outputWeights[which, j] =  input.InputMatrix[0, j];

            NormalizeRowWeight(outputWeights, which);
        }

        public void Initialize()
        {
			Network.OutputWeights.Ramdomize(-1, 1);
            for (var i = 0; i < _outputNeuronCount; i++)
                NormalizeRowWeight(Network.OutputWeights, i);
        }

        public void Iteration()
        {
			EvaluateErrors();
			
			TotalError = _globalError;
            if (TotalError < BestError)
            {
                BestError = TotalError;
				Network.OutputWeights.Copy(BestNetwork.OutputWeights);
            }

            var winners = _neuronWinCounts.Count(t => t != 0);
	        if (winners < _outputNeuronCount && winners < TrainingSet.Length)
            {
                ForceWin();
                return;
            }

            AdjustWeights();

            if (LearningRate > 0.01)
				LearningRate *= ReductionFactor;
        }

        protected void NormalizeRowWeight(Matrix matrix, int row)
        {
            var len = matrix.GetRow(row).VectorLength();
            len = Math.Max(len, MinValue);
			len = 1.0 / len;

            for (var i = 0; i < _inputNeuronCount; i++)
                matrix[row, i] =  matrix[row, i] * len;
            matrix[row, _inputNeuronCount] =  0;
        }
    }
}
