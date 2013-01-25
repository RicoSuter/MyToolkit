using System;
using System.Collections.Generic;

namespace MyToolkit.Recognition.WinRT.Learning.Genetic
{
	public abstract class Chromosome<T> : IComparable<Chromosome<T>>
	{
		private T[] genes; 
		public virtual T[] Genes
		{
			get { return genes; }
			set { genes = value; }
		}
		
		public double Cost { get; set; }
        public int Size { get { return Genes.Length; } }

		public GeneticAlgorithm<T> GeneticAlgorithm { get; set; }

		abstract public void CalculateCost();

        public int CompareTo(Chromosome<T> other)
        {
	        return Cost.CompareTo(other.Cost);
        }
		
		public T GetGene(int gene)
        {
            return Genes[gene];
        }

        private T GetNotTaken(Chromosome<T> source, IList<T> taken)
        {
            var geneLength = source.Size;
            for (var i = 0; i < geneLength; i++)
            {
                var trial = source.GetGene(i);
                if (!taken.Contains(trial))
                {
                    taken.Add(trial);
                    return trial;
                }
            }
			return default(T);
        }

        public void Mate(Chromosome<T> father, Chromosome<T> offspring1, Chromosome<T> offspring2)
        {
            var geneLength = Genes.Length; 
            var rand = new Random();

			var cutpoint1 = (int)(rand.NextDouble() * (geneLength - GeneticAlgorithm.CutLength));
            var cutpoint2 = cutpoint1 + GeneticAlgorithm.CutLength;

			var taken1 = new List<T>();
            var taken2 = new List<T>();

            for (int i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {

                }
                else
                {
                    offspring1.SetGene(i, father.GetGene(i));
                    offspring2.SetGene(i, GetGene(i));
                    taken1.Add(offspring1.GetGene(i));
                    taken2.Add(offspring2.GetGene(i));
                }
            }

            for (var i = 0; i < geneLength; i++)
            {
                if ((i < cutpoint1) || (i > cutpoint2))
                {
                    if (GeneticAlgorithm.PreventRepeat)
                    {
                        offspring1.SetGene(i, GetNotTaken(this, taken1));
                        offspring2.SetGene(i, GetNotTaken(father, taken2));
                    }
                    else
                    {
                        offspring1.SetGene(i, GetGene(i));
                        offspring2.SetGene(i, father.GetGene(i));
                    }
                }
            }
        
            if (rand.NextDouble() < GeneticAlgorithm.MutationPercent)
                offspring1.Mutate();

			if (rand.NextDouble() < GeneticAlgorithm.MutationPercent)
                offspring2.Mutate();

            offspring1.CalculateCost();
            offspring2.CalculateCost();
        }

        abstract public void Mutate();

		public void SetGene(int gene, T value)
        {
            Genes[gene] = value;
        }

        public void SetGenesDirect(T[] genes)
        {
            this.genes = genes;
        }

        override public String ToString()
        {
	        return "Chromosome: " + Cost;
        }
    }
}
