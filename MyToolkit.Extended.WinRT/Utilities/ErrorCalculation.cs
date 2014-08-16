using System;

namespace MyToolkit.MachineLearning.WinRT.Utilities
{
    public class ErrorCalculation
    {
        private double _globalError;
        private int _setSize;

        public double CalculateRootMeanSquare()
        {
            return Math.Sqrt(_globalError / _setSize);
        }

        public void Reset()
        {
            _globalError = 0;
            _setSize = 0;
        }

        public void UpdateError(double[] actual, double[] ideal)
        {
            for (var i = 0; i < actual.Length; i++)
            {
                var delta = ideal[i] - actual[i];
                _globalError += delta * delta;
            }
            _setSize += ideal.Length;
        }
    }
}
