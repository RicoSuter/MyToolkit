using System;

namespace MyToolkit.MachineLearning.WinRT.Utilities
{
    public class ErrorCalculation
    {
        private double globalError;
        private int setSize;

        public double CalculateRootMeanSquare()
        {
            return Math.Sqrt(globalError / setSize);
        }

        public void Reset()
        {
            globalError = 0;
            setSize = 0;
        }

        public void UpdateError(double[] actual, double[] ideal)
        {
            for (var i = 0; i < actual.Length; i++)
            {
                var delta = ideal[i] - actual[i];
                globalError += delta * delta;
            }
            setSize += ideal.Length;
        }
    }
}
