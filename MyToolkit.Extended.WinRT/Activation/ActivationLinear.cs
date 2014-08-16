using System;

namespace MyToolkit.MachineLearning.WinRT.Activation
{
    public class ActivationLinear : ActivationFunction
    {
		public override double Activate(double input)
        {
			return input;
        }

		public override double DerivativeActivate(double input)
        {
			throw new NotSupportedException();
        }
    }
}
