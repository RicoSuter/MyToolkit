using System.Xml.Serialization;

namespace MyToolkit.MachineLearning.WinRT.Activation
{
	[XmlInclude(typeof(ActivationLinear))]
	[XmlInclude(typeof(ActivationSigmoid))]
	[XmlInclude(typeof(ActivationTanh))]
	public abstract class ActivationFunction
    {
        public abstract double Function(double input);
		public abstract double DerivativeFunction(double input);
    }
}
