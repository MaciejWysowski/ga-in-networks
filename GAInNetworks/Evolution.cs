using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GAInNetworks
{
    public struct Offspring
    {
        public Individual Child1 { get; set; }
        public Individual Child2 { get; set; }
    }

    public struct Parents
    {
        public Individual Parent1 { get; set; }
        public Individual Parent2 { get; set; }
    }

    public class Evolution
    {
        public static Offspring OnePointCrossover(Individual Parent1, Individual Parent2)
        {
            var child1 = new Individual();
            var child2 = new Individual();
            var rangemin = 0;
            var minInd = Parent1;
            var maxInd = Parent2;

            if (Parent1.Genes.Count > Parent2.Genes.Count)
            {
                rangemin = Parent2.Genes.Count;
                maxInd = Parent1;
                minInd = Parent2;
            }
            else
            {
                rangemin = Parent1.Genes.Count;
                minInd = Parent1;
                maxInd = Parent2;
            }
                

            int crosspoint1 = Program.r.Next(1, rangemin -1);

            for (int i = 0; i < crosspoint1; i++)
            {
                child1.Genes.Add(maxInd.Genes[i]);
            }

            for (int i = crosspoint1; i < minInd.Genes.Count; i++)
            {
                child1.Genes.Add(minInd.Genes[i]);
            }

            for (int i = 0; i < crosspoint1; i++)
            {
                child2.Genes.Add(minInd.Genes[i]);
            }

            for (int i = crosspoint1; i < maxInd.Genes.Count; i++)
            {
                child2.Genes.Add(maxInd.Genes[i]);
            }


            return new Offspring { Child1 = child1, Child2 = child2 };
        }

        public static Offspring UniformCrossover(Individual Parent1, Individual Parent2)
        {
            bool isSwapped = false;
            var child1 = new Individual();
            var child2 = new Individual();
            int range = 0;

            if (Parent1.Genes.Count > Parent2.Genes.Count)
                range = Parent2.Genes.Count;
            else
                range = Parent1.Genes.Count;

            for (int i = 0; i < range; i++)
            {
                isSwapped = Convert.ToBoolean(Program.r.Next(0, 2));
                if (isSwapped)
                {
                    child1.Genes.Add(Parent1.Genes[i]);
                    child2.Genes.Add(Parent2.Genes[i]);
                }
                else
                {
                    child1.Genes.Add(Parent2.Genes[i]);
                    child2.Genes.Add(Parent1.Genes[i]);
                }
                    
            }

            if(child1.Genes[child1.Genes.Count -1] != 19)
            {
                child1.Genes.Add(19);
            }

            if (child2.Genes[child2.Genes.Count - 1] != 19)
            {
                child2.Genes.Add(19);
            }

            return new Offspring { Child1 = child1, Child2 = child2 };
        }

        public static Individual swapMutation(Individual chromosome, double mutationRate)
        {
            Individual tmp = new Individual(chromosome.Genes);
            

            if (Program.r.NextDouble() < mutationRate)
            {
                int i = Program.r.Next(1, chromosome.Genes.Count -1);
                int j = Program.r.Next(1, chromosome.Genes.Count -1);
                int v = tmp.Genes[i];
                tmp.Genes[i] = tmp.Genes[j];
                tmp.Genes[j] = v;
            }

            return new Individual(tmp.Genes);
        }

        public static Individual inversionMutation(Individual chromosome)
        {
            int i = Program.r.Next(1, chromosome.Genes.Count -1);
            int j = Program.r.Next(i, chromosome.Genes.Count -1);
            List<int> s = chromosome.Genes.GetRange(i, j - i + 1);

            List<int> tmp = new List<int>(chromosome.Genes);

            for (int index = j, idx = 0; index >= i; index--, idx++)
            {
                tmp[index] = s[idx];
            }

            return new Individual(tmp);
        }

        public static Individual scrumbleMutation(Individual chromosome)
        {
            int i = Program.r.Next(1, chromosome.Genes.Count - 1);
            int j = Program.r.Next(i, chromosome.Genes.Count - 1);
            List<int> tmp = new List<int>(chromosome.Genes);
            List<int> s = chromosome.Genes.GetRange(i, j - i + 1);
            s = s.OrderBy(x => Program.r.Next()).ToList();

            tmp.RemoveRange(i, j - i + 1);
            tmp.InsertRange(i, s);

            return new Individual(tmp);
        }

        public static Offspring TwoPointCrossover(Individual Parent1, Individual Parent2)
        {
            var child1 = new Individual();
            var child2 = new Individual();
            var range = 0;
            if (Parent1.Genes.Count > Parent2.Genes.Count)
                range = Parent2.Genes.Count;
            else
                range = Parent1.Genes.Count;

            int crosspoint1 = Program.r.Next(0, range -1);
            int crosspoint2 = Program.r.Next(0, range -1);

            // Ensure crosspoints are different...
            if (crosspoint1 == crosspoint2)
            {
                if (crosspoint1 == 0)
                {
                    crosspoint2++;
                }
                else
                {
                    crosspoint1--;
                }
            }
            // .. and crosspoint1 is lower than crosspoint2
            if (crosspoint2 < crosspoint1)
            {
                int temp = crosspoint1;
                crosspoint1 = crosspoint2;
                crosspoint2 = temp;
            }

            for (int i = 0; i <range; i++)
            {
 
                if (i < crosspoint1 || i > crosspoint2)
                {
                    child1.Genes.Add(Parent1.Genes[i]);
                    child2.Genes.Add(Parent2.Genes[i]);
                }               
                else
                {
                    child1.Genes.Add(Parent2.Genes[i]);
                    child2.Genes.Add(Parent1.Genes[i]);
                }
            }

            if (child1.Genes[child1.Genes.Count - 1] != 19)
            {
                child1.Genes.Add(19);
            }

            if (child2.Genes[child2.Genes.Count - 1] != 19)
            {
                child2.Genes.Add(19);
            }

            return new Offspring { Child1 = child1, Child2 = child2 };
        }

        public static Individual tournamentSelection(List<Individual> pop, int tournamentSize)
        {
            var tournament = new List<Individual>();

            for (int i = 0; i < tournamentSize; i++)
            {
                int randomId = Program.r.Next(0, pop.Count);
                tournament.Add(pop[randomId]);
            }

            var fittest = tournament.Min(x => x.Fitness);
            return tournament.FirstOrDefault(x => x.Fitness == fittest);
        }

        public static Individual rouletteSelection(List<Individual> pop)
        {
            double sumFitness = pop.Sum(x => (1/(double)x.Fitness));
            double randPick = Program.r.NextDouble() * sumFitness;

            double currentValue = 0;
            Individual result = pop[0];

            for (int i = 0; i < pop.Count; i++)
            {
                currentValue += (1/ (double)pop[i].Fitness);
                if (currentValue > randPick)
                {
                    result = pop[i];
                    break;
                }
            }

            return result;
        }

        public static Parents StochasticUniversalSampling(List<Individual> pop)
        {
            double sumFitness = pop.Sum(x => (1 / (double)x.Fitness));
            double randPick1 = Program.r.NextDouble() * sumFitness;
            double randPick2 = Program.r.NextDouble() * sumFitness;

            double currentValue = 0;
            Individual result1 = pop[0];
            Individual result2 = pop[0];

            for (int i = 0; i < pop.Count; i++)
            {
                currentValue += (1 / (double)pop[i].Fitness);
                if (currentValue > randPick1)
                {
                    result1 = pop[i];
                    break;
                }

                if (currentValue > randPick2)
                {
                    result2 = pop[i];
                    break;
                }
            }

            return new Parents { Parent1 = result1, Parent2 = result2 };
        }
    }
}
