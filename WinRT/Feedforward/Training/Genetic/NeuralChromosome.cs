using System;
using MyToolkit.Recognition.WinRT.Learning.Genetic;

namespace MyToolkit.Recognition.WinRT.Feedforward.Training.Genetic
{
    abstract public class NeuralChromosome : Chromosome<double>
    {
		private const double MutationRange = 20.0;
		public FeedforwardNetwork Network { get; set; }

        public override double[] Genes
        {
            set
            {
                base.Genes = value;
                CalculateCost();
            }
            get { return base.Genes; }
        }

        public void InitGenes(int length)
        {
            var result = new double[length];
            for (var i = 0; i < result.Length; i++)
                result[i] = 0;
            SetGenesDirect(result);
        }

        override public void Mutate()
        {
            var rand = new Random();
			var length = Genes.Length;
            for (var i = 0; i < length; i++)
            {
                var d = GetGene(i);
				d = d * (int)((MutationRange * rand.NextDouble()) - MutationRange);
                SetGene(i, d);
            }
        }

        public void UpdateGenes()
        {
			Genes = Network.ToArray();
        }

        public void UpdateNetwork()
        {
            FeedforwardNetwork.ArrayToNetwork(Genes, Network);
        }
    }
}
