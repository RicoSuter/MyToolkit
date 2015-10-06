using System;
using MyToolkit.Mathematics;

namespace MyToolkit.MachineLearning.WinRT.SOM
{
    public class NormalizeInput
    {
        public Matrix InputMatrix { get; private set; }

        public double NormalizationFactor { get; private set; }

        public double SyntheticInput { get; private set; }

        public NormalizationType Type { get; private set; }

        public double MinimumAllowedPatternSize = Math.Pow(10, -30);

        public NormalizeInput(double[] input, NormalizationType type)
        {
            Type = type;
            CalculateFactors(input);
            InputMatrix = CreateInputMatrix(input, SyntheticInput);
        }
        
        protected Matrix CreateInputMatrix(double[] pattern, double extra)
        {
            var result = new Matrix(1, pattern.Length + 1);
            for (var i = 0; i < pattern.Length; i++)
                result[0, i] = pattern[i];

            result[0, pattern.Length] = extra;
            return result;
        }

        protected void CalculateFactors(double[] input)
        {
            var inputMatrix = Matrix.CreateColumnMatrix(input);
            
            var length = inputMatrix.VectorLength();
            length = Math.Max(length, MinimumAllowedPatternSize);
            
            if (Type == NormalizationType.Multiplicative)
            {
                NormalizationFactor = 1.0 / length;
                SyntheticInput = 0.0;
            }
            else
            {
                NormalizationFactor = 1.0 / Math.Sqrt(input.Length);

                var d = input.Length - Math.Pow(length, 2);
                SyntheticInput = d > 0.0 ? Math.Sqrt(d) * NormalizationFactor : 0;
            }
        }
    }
}
