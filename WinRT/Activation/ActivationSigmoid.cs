using MyToolkit.Recognition.WinRT.Utilities;

namespace MyToolkit.Recognition.WinRT.Activation
{
    public class ActivationSigmoid : ActivationFunction
    {
        public override double Function(double input)
        {
            return 1.0 / (1 + BoundNumbers.Exp(-1.0 * input));
        }

		public override double DerivativeFunction(double input)
        {
			return input * (1.0 - input);
        }
    }
}
