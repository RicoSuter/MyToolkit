using MyToolkit.MachineLearning.WinRT.Utilities;

namespace MyToolkit.MachineLearning.WinRT.Activation
{
    public class ActivationSigmoid : ActivationFunction
    {
        public override double Activate(double input)
        {
            return 1.0 / (1 + BoundNumbers.Exp(-1.0 * input));
        }

		public override double DerivativeActivate(double input)
        {
			return input * (1.0 - input);
        }
    }
}
