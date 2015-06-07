using System;
using MyToolkit.MachineLearning.WinRT.Utilities;

namespace MyToolkit.MachineLearning.WinRT.Activation
{
    public class ActivationTanh : ActivationFunction
    {
        public override double Activate(double input)
        {
            return (BoundNumbers.Exp(input * 2.0) - 1.0) / (BoundNumbers.Exp(input * 2.0) + 1.0);
        }

		public override double DerivativeActivate(double input)
        {
			return 1.0 - Math.Pow(Activate(input), 2.0);
        }
    }
}
