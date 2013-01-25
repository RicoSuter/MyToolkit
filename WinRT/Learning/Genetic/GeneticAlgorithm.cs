using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyToolkit.Recognition.WinRT.Learning.Genetic
{
    public abstract class GeneticAlgorithm<T>
    {
		public Chromosome<T>[] Chromosomes { get; set; }
		
		public int PopulationSize { get; set; }
	    public double MutationPercent { get; set; }
	    public double PercentToMate { get; set; }
	    public double MatingPopulation { get; set; }
	    public bool PreventRepeat { get; set; }
	    public int CutLength { get; set; }

		public double Error
		{
			get { return Chromosomes[0].Cost; }
		}

        public Chromosome<T> GetChromosome(int i)
        {
            return Chromosomes[i];
        }

        public void Iteration()
        {
			var countToMate = (int)(PopulationSize * PercentToMate);
			var offspringCount = countToMate * 2;
			var offspringIndex = PopulationSize - offspringCount;
			var matingPopulationSize = (int)(PopulationSize * MatingPopulation);

	        var tasks = new List<Task>();
            var rand = new Random();
            for (var i = 0; i < countToMate; i++)
            {
                var mother = Chromosomes[i];
                var fatherInt = (int)(rand.NextDouble() * matingPopulationSize);
                var father = Chromosomes[fatherInt];
                var child1 = Chromosomes[offspringIndex];
                var child2 = Chromosomes[offspringIndex + 1];

				tasks.Add(Task.Run(() => mother.Mate(father, child1, child2)));
				offspringIndex += 2;
            }

			Task.WaitAll(tasks.ToArray());
			SortChromosomes();
        }

        public void SetChromosome(int i, Chromosome<T> value)
        {
            Chromosomes[i] = value;
        }

        public void SortChromosomes()
        {
            Array.Sort(Chromosomes);
        }
    }
}
