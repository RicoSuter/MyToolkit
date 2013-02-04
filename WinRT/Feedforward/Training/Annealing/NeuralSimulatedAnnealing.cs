using System;
using MyToolkit.MachineLearning.WinRT.Learning.Annealing;

namespace MyToolkit.MachineLearning.WinRT.Feedforward.Training.Annealing
{
    public class NeuralSimulatedAnnealing : SimulatedAnnealingBase<double>, ITrainable
    {
		private readonly double[][] input;
		private readonly double[][] ideal;
		
		public FeedforwardNetwork Network { get; set; }
		
		public NeuralSimulatedAnnealing(FeedforwardNetwork network, double[][] input, double[][] ideal, 
			double startTemperature, double stopTemperature, int cyclesPerIteration)
        {
            Network = network;

            this.input = input;
            this.ideal = ideal;
            
			Temperature = startTemperature;
            StartTemperature = startTemperature;
            StopTemperature = stopTemperature;
            CyclesPerIteration = cyclesPerIteration;
        }

        override public double DetermineError()
        {
            return Network.CalculateError(input, ideal);
        }

        override public void PutArray(double[] array)
        {
            FeedforwardNetwork.ArrayToNetwork(array, Network);
        }

        override public void Randomize()
        {
            var rand = new Random();
			var array = Network.ToArray();

            for (var i = 0; i < array.Length; i++)
            {
                var add = 0.5 - (rand.NextDouble());
                add = add / StartTemperature;
                add = add * Temperature;
                array[i] = array[i] + add;
            }

            FeedforwardNetwork.ArrayToNetwork(array, Network);
        }

        override public double[] GetArray()
        {
			return Network.ToArray();
        }

        override public double[] GetArrayCopy()
        {
            return GetArray();
        }
    }
}
