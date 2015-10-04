namespace MyToolkit.MachineLearning.WinRT.Feedforward.Training.Genetic
{
    class TrainingSetNeuralChromosome : NeuralChromosome
    {
        public override double[] Genes
        {
            get { return base.Genes; }
            set
            {
				base.Genes = value;
				CalculateCost();
            }
        }

        public TrainingSetNeuralChromosome(TrainingSetNeuralGeneticAlgorithm genetic, FeedforwardNetwork network)
        {
            GeneticAlgorithm =  genetic;
            Network = network;

            InitGenes(network.MatrixSize);
            UpdateGenes();
        }

        override public void CalculateCost()
        {
			UpdateNetwork();

			var input = ((TrainingSetNeuralGeneticAlgorithm)GeneticAlgorithm).Input;
			var ideal = ((TrainingSetNeuralGeneticAlgorithm)GeneticAlgorithm).Ideal;
			
			Cost  = Network.CalculateError(input, ideal);
        }
    }
}
