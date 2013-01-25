using MyToolkit.Recognition.WinRT.Learning.Genetic;

namespace MyToolkit.Recognition.WinRT.Feedforward.Training.Genetic
{
    public class NeuralGeneticAlgorithm : GeneticAlgorithm<double>, ITrainable
    {
        public FeedforwardNetwork Network
        {
            get
            {
                var c = (NeuralChromosome)GetChromosome(0);
                c.UpdateNetwork();
                return c.Network;
            }
        }
    }
}
