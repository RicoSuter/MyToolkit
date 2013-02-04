using System;

namespace MyToolkit.MachineLearning.WinRT.Learning.Annealing
{
    abstract public class SimulatedAnnealingBase<T>
    {
		public abstract T[] GetArrayCopy();
		public abstract void PutArray(T[] array);
		public abstract T[] GetArray();

		public abstract void Randomize();
		public abstract double DetermineError();

	    public double StartTemperature { get; set; }
	    public double StopTemperature { get; set; }

	    public int CyclesPerIteration { get; set; }

		public double Error { get; set; }
	    public double Temperature { get; protected set; }

        public void Iteration()
        {
	        Error = DetermineError();
			var bestArray = GetArrayCopy();

            Temperature = StartTemperature;
			for (var i = 0; i < CyclesPerIteration; i++)
            {
                Randomize();

				var currentError = DetermineError();
				if (currentError < Error)
                {
                    Error = currentError;
					bestArray = GetArrayCopy();
				}

                PutArray(bestArray);
				Temperature *= Math.Exp(Math.Log(StopTemperature / StartTemperature) / (CyclesPerIteration - 1));
            }
        }
    }
}
