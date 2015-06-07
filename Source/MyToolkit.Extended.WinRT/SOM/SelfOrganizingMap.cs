using System;
using System.Xml.Serialization;
using MyToolkit.Mathematics;

namespace MyToolkit.MachineLearning.WinRT.SOM
{
    public class SelfOrganizingMap
    {
        public NormalizationType NormalizationType { get; set; }

		[XmlIgnore]
		public double[] Output { get; set; }
		public Matrix OutputWeights { get; set; }

		public int InputNeuronCount { get; set; }
		public int OutputNeuronCount { get; set; }

		internal SelfOrganizingMap() { } // serialization only
		public SelfOrganizingMap(int inputCount, int outputCount, NormalizationType normalizationType) : this()
        {
			InputNeuronCount = inputCount;
            OutputNeuronCount = outputCount;
			
			OutputWeights = new Matrix(OutputNeuronCount, InputNeuronCount + 1);
			Output = new double[OutputNeuronCount];

			NormalizationType = normalizationType;
        }

		public int GetWinner(double[] input)
        {
            return GetWinner(new NormalizeInput(input, NormalizationType));
        }

		public int GetWinner(NormalizeInput input)
        {
            var winner = 0;
			if (Output == null)
				Output = new double[OutputNeuronCount];

            var biggest = Double.MinValue;
            for (var i = 0; i < OutputNeuronCount; i++)
            {
				Output[i] = MatrixMath.DotProduct(input.InputMatrix, OutputWeights.GetRow(i)) * input.NormalizationFactor;
				Output[i] = (Output[i] + 1.0) / 2.0;

				if (Output[i] > biggest)
                {
					biggest = Output[i];
                    winner = i;
                }

				if (Output[i] < 0)
					Output[i] = 0;
				if (Output[i] > 1)
					Output[i] = 1;
            }
			return winner;
        }
    }
}
