namespace MyToolkit.Recognition.WinRT.Feedforward.Training.Genetic
{
    public class TrainingSetNeuralGeneticAlgorithm : NeuralGeneticAlgorithm
    {
	    public double[][] Input { get; protected set; }
	    public double[][] Ideal { get; protected set; }

        public new double Error
        {
            get { return Network.CalculateError(Input, Ideal); }
        }

        public TrainingSetNeuralGeneticAlgorithm(FeedforwardNetwork network, bool reset, double[][] input, double[][] ideal, 
			int populationSize, double mutationPercent, double allowedPercentageToMate)
        {
			MutationPercent = mutationPercent;
            MatingPopulation = allowedPercentageToMate * 2;
            PopulationSize = populationSize;
            PercentToMate = allowedPercentageToMate;

            Input = input;
            Ideal = ideal;

            Chromosomes = new TrainingSetNeuralChromosome[PopulationSize];
            for (var i = 0; i < Chromosomes.Length; i++)
            {
                var chromosomeNetwork = (FeedforwardNetwork)network.Clone();
                if (reset)
                    chromosomeNetwork.Reset();

                var c = new TrainingSetNeuralChromosome(this, chromosomeNetwork);
                c.UpdateGenes();
                SetChromosome(i, c);
            }
            SortChromosomes();
        }
    }
}
